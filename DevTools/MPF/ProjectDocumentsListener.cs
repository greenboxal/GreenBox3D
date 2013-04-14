// ProjectDocumentsListener.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Project
{
    [CLSCompliant(false)]
    public abstract class ProjectDocumentsListener : IVsTrackProjectDocumentsEvents2, IDisposable
    {
        #region fields

        /// <summary>
        ///     Defines an object that will be a mutex for this object for synchronizing thread calls.
        /// </summary>
        private static volatile object Mutex = new object();

        private readonly IVsTrackProjectDocuments2 projectDocTracker;
        private readonly ServiceProvider serviceProvider;
        private uint eventsCookie;
        private bool isDisposed;

        #endregion

        #region ctors

        protected ProjectDocumentsListener(ServiceProvider serviceProviderParameter)
        {
            if (serviceProviderParameter == null)
            {
                throw new ArgumentNullException("serviceProviderParameter");
            }

            serviceProvider = serviceProviderParameter;
            projectDocTracker =
                serviceProvider.GetService(typeof(SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;

            Debug.Assert(projectDocTracker != null,
                         "Could not get the IVsTrackProjectDocuments2 object from the services exposed by this project");

            if (projectDocTracker == null)
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

        protected IVsTrackProjectDocuments2 ProjectDocumentTracker2
        {
            get { return projectDocTracker; }
        }

        protected ServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
        }

        #endregion

        #region IVsTrackProjectDocumentsEvents2 Members

        public virtual int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects,
                                                   int[] rgFirstIndices, string[] rgpszMkDocuments,
                                                   VSADDDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices,
                                             string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects,
                                                    int[] rgFirstIndices, string[] rgpszMkDocuments,
                                                    VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices,
                                              string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects,
                                                    int[] rgFirstIndices, string[] rgszMkOldNames,
                                                    string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices,
                                              string[] rgszMkOldNames, string[] rgszMkNewNames,
                                              VSRENAMEFILEFLAGS[] rgFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects,
                                                   int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments,
                                                 VSQUERYADDDIRECTORYFLAGS[] rgFlags,
                                                 VSQUERYADDDIRECTORYRESULTS[] pSummaryResult,
                                                 VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments,
                                           VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult,
                                           VSQUERYADDFILERESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments,
                                                    VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags,
                                                    VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult,
                                                    VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments,
                                              VSQUERYREMOVEFILEFLAGS[] rgFlags,
                                              VSQUERYREMOVEFILERESULTS[] pSummaryResult,
                                              VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames,
                                                    string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags,
                                                    VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult,
                                                    VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames,
                                              string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags,
                                              VSQUERYRENAMEFILERESULTS[] pSummaryResult,
                                              VSQUERYRENAMEFILERESULTS[] rgResults)
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
            if (ProjectDocumentTracker2 != null)
            {
                ErrorHandler.ThrowOnFailure(ProjectDocumentTracker2.AdviseTrackProjectDocumentsEvents(this,
                                                                                                      out eventsCookie));
            }
        }

        /// <summary>
        ///     The method that does the cleanup.
        /// </summary>
        /// <param name="disposing"></param>
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults",
            MessageId =
                "Microsoft.VisualStudio.Shell.Interop.IVsTrackProjectDocuments2.UnadviseTrackProjectDocumentsEvents(System.UInt32)"
            )]
        protected virtual void Dispose(bool disposing)
        {
            // Everybody can go here.
            if (!isDisposed)
            {
                // Synchronize calls to the Dispose simulteniously.
                lock (Mutex)
                {
                    if (disposing && eventsCookie != (uint)ShellConstants.VSCOOKIE_NIL &&
                        ProjectDocumentTracker2 != null)
                    {
                        ProjectDocumentTracker2.UnadviseTrackProjectDocumentsEvents(eventsCookie);
                        eventsCookie = (uint)ShellConstants.VSCOOKIE_NIL;
                    }

                    isDisposed = true;
                }
            }
        }

        #endregion
    }
}
