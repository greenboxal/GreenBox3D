// SelectionListener.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;

namespace Microsoft.VisualStudio.Project
{
    [CLSCompliant(false)]
    public abstract class SelectionListener : IVsSelectionEvents, IDisposable
    {
        #region fields

        /// <summary>
        ///     Defines an object that will be a mutex for this object for synchronizing thread calls.
        /// </summary>
        private static volatile object Mutex = new object();

        private readonly IVsMonitorSelection monSel;
        private readonly ServiceProvider serviceProvider;
        private uint eventsCookie;
        private bool isDisposed;

        #endregion

        #region ctors

        protected SelectionListener(ServiceProvider serviceProviderParameter)
        {
            if (serviceProviderParameter == null)
            {
                throw new ArgumentNullException("serviceProviderParameter");
            }

            serviceProvider = serviceProviderParameter;
            monSel = serviceProvider.GetService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;

            if (monSel == null)
            {
                throw new InvalidOperationException();
            }
        }

        #endregion

        #region properties

        protected uint EventsCookie
        {
            get { return eventsCookie; }
        }

        protected IVsMonitorSelection SelectionMonitor
        {
            get { return monSel; }
        }

        protected ServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
        }

        #endregion

        #region IVsSelectionEvents Members

        public virtual int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld,
                                              ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew,
                                              IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     The IDispose interface Dispose method for disposing the object determinastically.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region methods

        public void Init()
        {
            if (SelectionMonitor != null)
            {
                ErrorHandler.ThrowOnFailure(SelectionMonitor.AdviseSelectionEvents(this, out eventsCookie));
            }
        }

        /// <summary>
        ///     The method that does the cleanup.
        /// </summary>
        /// <param name="disposing"></param>
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults",
            MessageId =
                "Microsoft.VisualStudio.Shell.Interop.IVsMonitorSelection.UnadviseSelectionEvents(System.UInt32)")]
        protected virtual void Dispose(bool disposing)
        {
            // Everybody can go here.
            if (!isDisposed)
            {
                // Synchronize calls to the Dispose simulteniously.
                lock (Mutex)
                {
                    if (disposing && eventsCookie != (uint)ShellConstants.VSCOOKIE_NIL && SelectionMonitor != null)
                    {
                        SelectionMonitor.UnadviseSelectionEvents(eventsCookie);
                        eventsCookie = (uint)ShellConstants.VSCOOKIE_NIL;
                    }

                    isDisposed = true;
                }
            }
        }

        #endregion
    }
}
