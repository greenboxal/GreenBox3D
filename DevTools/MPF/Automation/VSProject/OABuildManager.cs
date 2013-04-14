// OABuildManager.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics.CodeAnalysis;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;
using Microsoft.VisualStudio;

namespace Microsoft.VisualStudio.Project.Automation
{
    public class OABuildManager : ConnectionPointContainer,
                                  IEventSource<_dispBuildManagerEvents>,
                                  BuildManager,
                                  BuildManagerEvents
    {
        private readonly ProjectNode projectManager;

        public OABuildManager(ProjectNode project)
        {
            projectManager = project;
            AddEventSource(this);
        }

        #region BuildManager Members

        public virtual string BuildDesignTimeOutput(string bstrOutputMoniker)
        {
            throw new NotImplementedException();
        }

        public virtual EnvDTE.Project ContainingProject
        {
            get { return projectManager.GetAutomationObject() as EnvDTE.Project; }
        }

        public virtual DTE DTE
        {
            get { return projectManager.Site.GetService(typeof(DTE)) as DTE; }
        }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public virtual object DesignTimeOutputMonikers
        {
            get { throw new NotImplementedException(); }
        }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public virtual object Parent
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region _dispBuildManagerEvents_Event Members

        public event _dispBuildManagerEvents_DesignTimeOutputDeletedEventHandler DesignTimeOutputDeleted;

        public event _dispBuildManagerEvents_DesignTimeOutputDirtyEventHandler DesignTimeOutputDirty;

        #endregion

        #region IEventSource<_dispBuildManagerEvents> Members

        void IEventSource<_dispBuildManagerEvents>.OnSinkAdded(_dispBuildManagerEvents sink)
        {
            DesignTimeOutputDeleted += sink.DesignTimeOutputDeleted;
            DesignTimeOutputDirty += sink.DesignTimeOutputDirty;
        }

        void IEventSource<_dispBuildManagerEvents>.OnSinkRemoved(_dispBuildManagerEvents sink)
        {
            DesignTimeOutputDeleted -= sink.DesignTimeOutputDeleted;
            DesignTimeOutputDirty -= sink.DesignTimeOutputDirty;
        }

        #endregion

        protected virtual void OnDesignTimeOutputDeleted(string outputMoniker)
        {
            var handlers = DesignTimeOutputDeleted;
            if (handlers != null)
            {
                handlers(outputMoniker);
            }
        }

        protected virtual void OnDesignTimeOutputDirty(string outputMoniker)
        {
            var handlers = DesignTimeOutputDirty;
            if (handlers != null)
            {
                handlers(outputMoniker);
            }
        }
    }
}
