// AutomationScope.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics.CodeAnalysis;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;

namespace Microsoft.VisualStudio.Project.Automation
{
    /// <summary>
    ///     Helper class that handle the scope of an automation function.
    ///     It should be used inside a "using" directive to define the scope of the
    ///     automation function and make sure that the ExitAutomation method is called.
    /// </summary>
    internal class AutomationScope : IDisposable
    {
        private static volatile object Mutex;
        private readonly IVsExtensibility3 extensibility;
        private bool inAutomation;
        private bool isDisposed;

        /// <summary>
        ///     Initializes the <see cref="AutomationScope" /> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static AutomationScope()
        {
            Mutex = new object();
        }

        /// <summary>
        ///     Defines the beginning of the scope of an automation function. This constuctor
        ///     calls EnterAutomationFunction to signal the Shell that the current function is
        ///     changing the status of the automation objects.
        /// </summary>
        public AutomationScope(IServiceProvider provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException("provider");
            }
            extensibility = provider.GetService(typeof(IVsExtensibility)) as IVsExtensibility3;
            if (null == extensibility)
            {
                throw new InvalidOperationException();
            }
            ErrorHandler.ThrowOnFailure(extensibility.EnterAutomationFunction());
            inAutomation = true;
        }

        /// <summary>
        ///     Gets the IVsExtensibility3 interface used in the automation function.
        /// </summary>
        public IVsExtensibility3 Extensibility
        {
            get { return extensibility; }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                lock (Mutex)
                {
                    if (disposing)
                    {
                        ExitAutomation();
                    }

                    isDisposed = true;
                }
            }
        }

        #endregion

        /// <summary>
        ///     Ends the scope of the automation function. This function is also called by the
        ///     Dispose method.
        /// </summary>
        public void ExitAutomation()
        {
            if (inAutomation)
            {
                ErrorHandler.ThrowOnFailure(extensibility.ExitAutomationFunction());
                inAutomation = false;
            }
        }
    }
}
