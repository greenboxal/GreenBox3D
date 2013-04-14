// IDEBuildLogger.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.Win32;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.Project
{
    /// <summary>
    ///     This class implements an MSBuild logger that output events to VS outputwindow and tasklist.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IDE")]
    internal class IDEBuildLogger : Logger
    {
        #region fields

        // TODO: Remove these constants when we have a version that supports getting the verbosity using automation.
        private const string buildVerbosityRegistrySubKey = @"General";
        private const string buildVerbosityRegistryKey = "MSBuildLoggerVerbosity";
        private readonly Dispatcher dispatcher;

        private readonly IVsHierarchy hierarchy;
        private readonly IServiceProvider serviceProvider;
        private readonly TaskProvider taskProvider;
        private string buildVerbosityRegistryRoot = @"Software\Microsoft\VisualStudio\10.0";
        private int currentIndent;
        private string errorString = SR.GetString(SR.Error, CultureInfo.CurrentUICulture);
        private bool haveCachedVerbosity;

        // Queues to manage Tasks and Error output plus message logging
        private ConcurrentQueue<string> outputQueue;
        private IVsOutputWindowPane outputWindowPane;
        private ConcurrentQueue<Func<ErrorTask>> taskQueue;
        private string warningString = SR.GetString(SR.Warning, CultureInfo.CurrentUICulture);

        #endregion

        #region properties

        public IServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
        }

        public string WarningString
        {
            get { return warningString; }
            set { warningString = value; }
        }

        public string ErrorString
        {
            get { return errorString; }
            set { errorString = value; }
        }

        /// <summary>
        ///     When the build is not a "design time" (background or secondary) build this is True
        /// </summary>
        /// <remarks>
        ///     The only known way to detect an interactive build is to check this.outputWindowPane for null.
        /// </remarks>
        protected bool InteractiveBuild
        {
            get { return outputWindowPane != null; }
        }

        /// <summary>
        ///     When building from within VS, setting this will
        ///     enable the logger to retrive the verbosity from
        ///     the correct registry hive.
        /// </summary>
        internal string BuildVerbosityRegistryRoot
        {
            get { return buildVerbosityRegistryRoot; }
            set { buildVerbosityRegistryRoot = value; }
        }

        /// <summary>
        ///     Set to null to avoid writing to the output window
        /// </summary>
        internal IVsOutputWindowPane OutputWindowPane
        {
            get { return outputWindowPane; }
            set { outputWindowPane = value; }
        }

        #endregion

        #region ctors

        /// <summary>
        ///     Constructor.  Inititialize member data.
        /// </summary>
        public IDEBuildLogger(IVsOutputWindowPane output, TaskProvider taskProvider, IVsHierarchy hierarchy)
        {
            if (taskProvider == null)
                throw new ArgumentNullException("taskProvider");
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            Trace.WriteLineIf(Thread.CurrentThread.GetApartmentState() != ApartmentState.STA,
                              "WARNING: IDEBuildLogger constructor running on the wrong thread.");

            IOleServiceProvider site;
            VisualStudio.ErrorHandler.ThrowOnFailure(hierarchy.GetSite(out site));

            this.taskProvider = taskProvider;
            outputWindowPane = output;
            this.hierarchy = hierarchy;
            serviceProvider = new ServiceProvider(site);
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        #endregion

        #region overridden methods

        /// <summary>
        ///     Overridden from the Logger class.
        /// </summary>
        public override void Initialize(IEventSource eventSource)
        {
            if (null == eventSource)
            {
                throw new ArgumentNullException("eventSource");
            }

            taskQueue = new ConcurrentQueue<Func<ErrorTask>>();
            outputQueue = new ConcurrentQueue<string>();

            eventSource.BuildStarted += BuildStartedHandler;
            eventSource.BuildFinished += BuildFinishedHandler;
            eventSource.ProjectStarted += ProjectStartedHandler;
            eventSource.ProjectFinished += ProjectFinishedHandler;
            eventSource.TargetStarted += TargetStartedHandler;
            eventSource.TargetFinished += TargetFinishedHandler;
            eventSource.TaskStarted += TaskStartedHandler;
            eventSource.TaskFinished += TaskFinishedHandler;
            eventSource.CustomEventRaised += CustomHandler;
            eventSource.ErrorRaised += ErrorHandler;
            eventSource.WarningRaised += WarningHandler;
            eventSource.MessageRaised += MessageHandler;
        }

        #endregion

        #region event delegates

        /// <summary>
        ///     This is the delegate for BuildStartedHandler events.
        /// </summary>
        protected virtual void BuildStartedHandler(object sender, BuildStartedEventArgs buildEvent)
        {
            // NOTE: This may run on a background thread!
            ClearCachedVerbosity();
            ClearQueuedOutput();
            ClearQueuedTasks();

            QueueOutputEvent(MessageImportance.Low, buildEvent);
        }

        /// <summary>
        ///     This is the delegate for BuildFinishedHandler events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="buildEvent"></param>
        protected virtual void BuildFinishedHandler(object sender, BuildFinishedEventArgs buildEvent)
        {
            // NOTE: This may run on a background thread!
            MessageImportance importance = buildEvent.Succeeded ? MessageImportance.Low : MessageImportance.High;
            QueueOutputText(importance, Environment.NewLine);
            QueueOutputEvent(importance, buildEvent);

            // flush output and error queues
            ReportQueuedOutput();
            ReportQueuedTasks();
        }

        /// <summary>
        ///     This is the delegate for ProjectStartedHandler events.
        /// </summary>
        protected virtual void ProjectStartedHandler(object sender, ProjectStartedEventArgs buildEvent)
        {
            // NOTE: This may run on a background thread!
            QueueOutputEvent(MessageImportance.Low, buildEvent);
        }

        /// <summary>
        ///     This is the delegate for ProjectFinishedHandler events.
        /// </summary>
        protected virtual void ProjectFinishedHandler(object sender, ProjectFinishedEventArgs buildEvent)
        {
            // NOTE: This may run on a background thread!
            QueueOutputEvent(buildEvent.Succeeded ? MessageImportance.Low : MessageImportance.High, buildEvent);
        }

        /// <summary>
        ///     This is the delegate for TargetStartedHandler events.
        /// </summary>
        protected virtual void TargetStartedHandler(object sender, TargetStartedEventArgs buildEvent)
        {
            // NOTE: This may run on a background thread!
            QueueOutputEvent(MessageImportance.Low, buildEvent);
            IndentOutput();
        }

        /// <summary>
        ///     This is the delegate for TargetFinishedHandler events.
        /// </summary>
        protected virtual void TargetFinishedHandler(object sender, TargetFinishedEventArgs buildEvent)
        {
            // NOTE: This may run on a background thread!
            UnindentOutput();
            QueueOutputEvent(MessageImportance.Low, buildEvent);
        }

        /// <summary>
        ///     This is the delegate for TaskStartedHandler events.
        /// </summary>
        protected virtual void TaskStartedHandler(object sender, TaskStartedEventArgs buildEvent)
        {
            // NOTE: This may run on a background thread!
            QueueOutputEvent(MessageImportance.Low, buildEvent);
            IndentOutput();
        }

        /// <summary>
        ///     This is the delegate for TaskFinishedHandler events.
        /// </summary>
        protected virtual void TaskFinishedHandler(object sender, TaskFinishedEventArgs buildEvent)
        {
            // NOTE: This may run on a background thread!
            UnindentOutput();
            QueueOutputEvent(MessageImportance.Low, buildEvent);
        }

        /// <summary>
        ///     This is the delegate for CustomHandler events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="buildEvent"></param>
        protected virtual void CustomHandler(object sender, CustomBuildEventArgs buildEvent)
        {
            // NOTE: This may run on a background thread!
            QueueOutputEvent(MessageImportance.High, buildEvent);
        }

        /// <summary>
        ///     This is the delegate for error events.
        /// </summary>
        protected virtual void ErrorHandler(object sender, BuildErrorEventArgs errorEvent)
        {
            // NOTE: This may run on a background thread!
            QueueOutputText(GetFormattedErrorMessage(errorEvent.File, errorEvent.LineNumber, errorEvent.ColumnNumber,
                                                     false, errorEvent.Code, errorEvent.Message));
            QueueTaskEvent(errorEvent);
        }

        /// <summary>
        ///     This is the delegate for warning events.
        /// </summary>
        protected virtual void WarningHandler(object sender, BuildWarningEventArgs warningEvent)
        {
            // NOTE: This may run on a background thread!
            QueueOutputText(MessageImportance.High,
                            GetFormattedErrorMessage(warningEvent.File, warningEvent.LineNumber,
                                                     warningEvent.ColumnNumber, true, warningEvent.Code,
                                                     warningEvent.Message));
            QueueTaskEvent(warningEvent);
        }

        /// <summary>
        ///     This is the delegate for Message event types
        /// </summary>
        protected virtual void MessageHandler(object sender, BuildMessageEventArgs messageEvent)
        {
            // NOTE: This may run on a background thread!
            QueueOutputEvent(messageEvent.Importance, messageEvent);
        }

        #endregion

        #region output queue

        protected void QueueOutputEvent(MessageImportance importance, BuildEventArgs buildEvent)
        {
            // NOTE: This may run on a background thread!
            if (LogAtImportance(importance) && !string.IsNullOrEmpty(buildEvent.Message))
            {
                StringBuilder message = new StringBuilder(currentIndent + buildEvent.Message.Length);
                if (currentIndent > 0)
                {
                    message.Append('\t', currentIndent);
                }
                message.AppendLine(buildEvent.Message);

                QueueOutputText(message.ToString());
            }
        }

        protected void QueueOutputText(MessageImportance importance, string text)
        {
            // NOTE: This may run on a background thread!
            if (LogAtImportance(importance))
            {
                QueueOutputText(text);
            }
        }

        protected void QueueOutputText(string text)
        {
            // NOTE: This may run on a background thread!
            if (OutputWindowPane != null)
            {
                // Enqueue the output text
                outputQueue.Enqueue(text);

                // We want to interactively report the output. But we dont want to dispatch
                // more than one at a time, otherwise we might overflow the main thread's
                // message queue. So, we only report the output if the queue was empty.
                if (outputQueue.Count == 1)
                {
                    ReportQueuedOutput();
                }
            }
        }

        private void IndentOutput()
        {
            // NOTE: This may run on a background thread!
            currentIndent++;
        }

        private void UnindentOutput()
        {
            // NOTE: This may run on a background thread!
            currentIndent--;
        }

        private void ReportQueuedOutput()
        {
            // NOTE: This may run on a background thread!
            // We need to output this on the main thread. We must use BeginInvoke because the main thread may not be pumping events yet.
            BeginInvokeWithErrorMessage(serviceProvider, dispatcher, () =>
            {
                if (OutputWindowPane != null)
                {
                    string outputString;

                    while (outputQueue.TryDequeue(out outputString))
                    {
                        VisualStudio.ErrorHandler.ThrowOnFailure(OutputWindowPane.OutputString(outputString));
                    }
                }
            });
        }

        private void ClearQueuedOutput()
        {
            // NOTE: This may run on a background thread!
            outputQueue = new ConcurrentQueue<string>();
        }

        #endregion output queue

        #region task queue

        protected void QueueTaskEvent(BuildEventArgs errorEvent)
        {
            taskQueue.Enqueue(() =>
            {
                ErrorTask task = new ErrorTask();

                if (errorEvent is BuildErrorEventArgs)
                {
                    BuildErrorEventArgs errorArgs = (BuildErrorEventArgs)errorEvent;
                    task.Document = errorArgs.File;
                    task.ErrorCategory = TaskErrorCategory.Error;
                    task.Line = errorArgs.LineNumber - 1; // The task list does +1 before showing this number.
                    task.Column = errorArgs.ColumnNumber;
                    task.Priority = TaskPriority.High;
                }
                else if (errorEvent is BuildWarningEventArgs)
                {
                    BuildWarningEventArgs warningArgs = (BuildWarningEventArgs)errorEvent;
                    task.Document = warningArgs.File;
                    task.ErrorCategory = TaskErrorCategory.Warning;
                    task.Line = warningArgs.LineNumber - 1; // The task list does +1 before showing this number.
                    task.Column = warningArgs.ColumnNumber;
                    task.Priority = TaskPriority.Normal;
                }

                task.Text = errorEvent.Message;
                task.Category = TaskCategory.BuildCompile;
                task.HierarchyItem = hierarchy;

                return task;
            });

            // NOTE: Unlike output we dont want to interactively report the tasks. So we never queue
            // call ReportQueuedTasks here. We do this when the build finishes.
        }

        private void ReportQueuedTasks()
        {
            // NOTE: This may run on a background thread!
            // We need to output this on the main thread. We must use BeginInvoke because the main thread may not be pumping events yet.
            BeginInvokeWithErrorMessage(serviceProvider, dispatcher, () =>
            {
                taskProvider.SuspendRefresh();
                try
                {
                    Func<ErrorTask> taskFunc;

                    while (taskQueue.TryDequeue(out taskFunc))
                    {
                        // Create the error task
                        ErrorTask task = taskFunc();

                        // Log the task
                        taskProvider.Tasks.Add(task);
                    }
                }
                finally
                {
                    taskProvider.ResumeRefresh();
                }
            });
        }

        private void ClearQueuedTasks()
        {
            // NOTE: This may run on a background thread!
            taskQueue = new ConcurrentQueue<Func<ErrorTask>>();

            if (InteractiveBuild)
            {
                // We need to clear this on the main thread. We must use BeginInvoke because the main thread may not be pumping events yet.
                BeginInvokeWithErrorMessage(serviceProvider, dispatcher, () => { taskProvider.Tasks.Clear(); });
            }
        }

        #endregion task queue

        #region helpers

        /// <summary>
        ///     This method takes a MessageImportance and returns true if messages
        ///     at importance i should be loggeed.  Otherwise return false.
        /// </summary>
        private bool LogAtImportance(MessageImportance importance)
        {
            // If importance is too low for current settings, ignore the event
            bool logIt = false;

            SetVerbosity();

            switch (Verbosity)
            {
                case LoggerVerbosity.Quiet:
                    logIt = false;
                    break;
                case LoggerVerbosity.Minimal:
                    logIt = (importance == MessageImportance.High);
                    break;
                case LoggerVerbosity.Normal:
                    // Falling through...
                case LoggerVerbosity.Detailed:
                    logIt = (importance != MessageImportance.Low);
                    break;
                case LoggerVerbosity.Diagnostic:
                    logIt = true;
                    break;
                default:
                    Debug.Fail("Unknown Verbosity level. Ignoring will cause everything to be logged");
                    break;
            }

            return logIt;
        }

        /// <summary>
        ///     Format error messages for the task list
        /// </summary>
        private string GetFormattedErrorMessage(
            string fileName,
            int line,
            int column,
            bool isWarning,
            string errorNumber,
            string errorText)
        {
            string errorCode = isWarning ? WarningString : ErrorString;

            StringBuilder message = new StringBuilder();
            if (!string.IsNullOrEmpty(fileName))
            {
                message.AppendFormat(CultureInfo.CurrentCulture, "{0}({1},{2}):", fileName, line, column);
            }
            message.AppendFormat(CultureInfo.CurrentCulture, " {0} {1}: {2}", errorCode, errorNumber, errorText);
            message.AppendLine();

            return message.ToString();
        }

        /// <summary>
        ///     Sets the verbosity level.
        /// </summary>
        private void SetVerbosity()
        {
            // TODO: This should be replaced when we have a version that supports automation.
            if (!haveCachedVerbosity)
            {
                string verbosityKey = String.Format(CultureInfo.InvariantCulture, @"{0}\{1}", BuildVerbosityRegistryRoot,
                                                    buildVerbosityRegistrySubKey);
                using (RegistryKey subKey = Registry.CurrentUser.OpenSubKey(verbosityKey))
                {
                    if (subKey != null)
                    {
                        object valueAsObject = subKey.GetValue(buildVerbosityRegistryKey);
                        if (valueAsObject != null)
                        {
                            Verbosity = (LoggerVerbosity)((int)valueAsObject);
                        }
                    }
                }

                haveCachedVerbosity = true;
            }
        }

        /// <summary>
        ///     Clear the cached verbosity, so that it will be re-evaluated from the build verbosity registry key.
        /// </summary>
        private void ClearCachedVerbosity()
        {
            haveCachedVerbosity = false;
        }

        #endregion helpers

        #region exception handling helpers

        /// <summary>
        ///     Call Dispatcher.BeginInvoke, showing an error message if there was a non-critical exception.
        /// </summary>
        /// <param name="serviceProvider">service provider</param>
        /// <param name="dispatcher">dispatcher</param>
        /// <param name="action">action to invoke</param>
        private static void BeginInvokeWithErrorMessage(IServiceProvider serviceProvider, Dispatcher dispatcher,
                                                        Action action)
        {
            dispatcher.BeginInvoke(new Action(() => CallWithErrorMessage(serviceProvider, action)));
        }

        /// <summary>
        ///     Show error message if exception is caught when invoking a method
        /// </summary>
        /// <param name="serviceProvider">service provider</param>
        /// <param name="action">action to invoke</param>
        private static void CallWithErrorMessage(IServiceProvider serviceProvider, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (VisualStudio.ErrorHandler.IsCriticalException(ex))
                {
                    throw;
                }

                ShowErrorMessage(serviceProvider, ex);
            }
        }

        /// <summary>
        ///     Show error window about the exception
        /// </summary>
        /// <param name="serviceProvider">service provider</param>
        /// <param name="exception">exception</param>
        private static void ShowErrorMessage(IServiceProvider serviceProvider, Exception exception)
        {
            IUIService UIservice = (IUIService)serviceProvider.GetService(typeof(IUIService));
            if (UIservice != null && exception != null)
            {
                UIservice.ShowError(exception);
            }
        }

        #endregion exception handling helpers
    }
}
