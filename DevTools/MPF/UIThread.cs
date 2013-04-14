// UIThread.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Project
{
    using Shell;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;

    internal sealed class UIThread : IDisposable
    {
        private WindowsFormsSynchronizationContext synchronizationContext;
#if DEBUG
        /// <summary>
        ///     Stack trace when synchronizationContext was captured
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private StackTrace captureStackTrace;
#endif

        private Thread uithread;

        /// <summary>
        ///     RunSync puts orignal exception stacktrace to Exception.Data by this key if action throws on UI thread
        /// </summary>
        /// WrappedStacktraceKey is a string to keep exception serializable.
        private const string WrappedStacktraceKey = "$$Microsoft.VisualStudio.Package.UIThread.WrappedStacktraceKey$$";

        /// <summary>
        ///     The singleton instance.
        /// </summary>
        private static volatile UIThread instance = new UIThread();

        internal UIThread()
        {
            Initialize();
        }

        /// <summary>
        ///     Gets the singleton instance
        /// </summary>
        public static UIThread Instance
        {
            get { return instance; }
        }

        /// <summary>
        ///     Checks whether this is the UI thread.
        /// </summary>
        public bool IsUIThread
        {
            get { return uithread == Thread.CurrentThread; }
        }

        /// <summary>
        ///     Gets a value indicating whether unit tests are running.
        /// </summary>
        internal static bool IsUnitTest { get; set; }

        #region IDisposable Members

        /// <summary>
        ///     Dispose implementation.
        /// </summary>
        public void Dispose()
        {
            if (synchronizationContext != null)
            {
                synchronizationContext.Dispose();
            }
        }

        #endregion

        /// <summary>
        ///     Initializes unit testing mode for this object
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal void InitUnitTestingMode()
        {
            Debug.Assert(synchronizationContext == null,
                         "Context has already been captured; too late to InitUnitTestingMode");
            IsUnitTest = true;
        }

        [Conditional("DEBUG")]
        internal void MustBeCalledFromUIThread()
        {
            Debug.Assert(uithread == Thread.CurrentThread || IsUnitTest, "This must be called from the GUI thread");
        }

        /// <summary>
        ///     Runs an action asynchronously on an associated forms synchronization context.
        /// </summary>
        /// <param name="a">The action to run</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal void Run(Action a)
        {
            if (IsUnitTest)
            {
                a();
                return;
            }
            Debug.Assert(synchronizationContext != null,
                         "The SynchronizationContext must be captured before calling this method");
#if DEBUG
            StackTrace stackTrace = new StackTrace(true);
#endif
            synchronizationContext.Post(delegate
            {
                try
                {
                    MustBeCalledFromUIThread();
                    a();
                }
#if DEBUG
                catch (Exception e)
                {
                    // swallow, random exceptions should not kill process
                    Debug.Assert(false,
                                 string.Format(CultureInfo.InvariantCulture,
                                               "UIThread.Run caught and swallowed exception: {0}\n\noriginally invoked from stack:\n{1}",
                                               e, stackTrace));
                }
#else
                catch (Exception)
                {
                    // swallow, random exceptions should not kill process
                }
#endif
            }, null);
        }

        /// <summary>
        ///     Runs an action synchronously on an associated forms synchronization context
        /// </summary>
        /// <param name="a">The action to run.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"),
         SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal void RunSync(Action a)
        {
            if (IsUnitTest)
            {
                a();
                return;
            }
            Exception exn = null;
            ;
            Debug.Assert(synchronizationContext != null,
                         "The SynchronizationContext must be captured before calling this method");

            // Send on UI thread will execute immediately.
            synchronizationContext.Send(ignore =>
            {
                try
                {
                    MustBeCalledFromUIThread();
                    a();
                }
                catch (Exception e)
                {
                    exn = e;
                }
            }, null
                );
            if (exn != null)
            {
                // throw exception on calling thread, preserve stacktrace
                if (!exn.Data.Contains(WrappedStacktraceKey))
                    exn.Data[WrappedStacktraceKey] = exn.StackTrace;
                throw exn;
            }
        }

        /// <summary>
        ///     Performs a callback on the UI thread, blocking until the action completes.  Uses the VS mechanism
        ///     of marshalling back to the main STA thread via COM RPC.
        /// </summary>
        internal static T DoOnUIThread<T>(Func<T> callback)
        {
            return ThreadHelper.Generic.Invoke(callback);
        }

        /// <summary>
        ///     Performs a callback on the UI thread, blocking until the action completes.  Uses the VS mechanism
        ///     of marshalling back to the main STA thread via COM RPC.
        /// </summary>
        internal static void DoOnUIThread(Action callback)
        {
            ThreadHelper.Generic.Invoke(callback);
        }

        /// <summary>
        ///     Initializes this object.
        /// </summary>
        private void Initialize()
        {
            if (IsUnitTest)
                return;
            uithread = Thread.CurrentThread;

            if (synchronizationContext == null)
            {
#if DEBUG
                // This is a handy place to do this, since the product and all interesting unit tests
                // must go through this code path.
                AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs args)
                {
                    if (args.IsTerminating)
                    {
                        string s = String.Format(CultureInfo.InvariantCulture,
                                                 "An unhandled exception is about to terminate the process.  Exception info:\n{0}",
                                                 args.ExceptionObject);
                        Debug.Assert(false, s);
                    }
                };

                captureStackTrace = new StackTrace(true);
#endif
                synchronizationContext = new WindowsFormsSynchronizationContext();
            }
            else
            {
                // Make sure we are always capturing the same thread.
                Debug.Assert(uithread == Thread.CurrentThread);
            }
        }
    }
}
