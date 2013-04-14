// BuildDependency.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project
{
    public class BuildDependency : IVsBuildDependency
    {
        private readonly ProjectNode projectMgr;
        private readonly Guid referencedProjectGuid = Guid.Empty;

        [CLSCompliant(false)]
        public BuildDependency(ProjectNode projectMgr, Guid projectReference)
        {
            referencedProjectGuid = projectReference;
            this.projectMgr = projectMgr;
        }

        #region IVsBuildDependency methods

        public int get_CanonicalName(out string canonicalName)
        {
            canonicalName = null;
            return VSConstants.S_OK;
        }

        public int get_Type(out Guid guidType)
        {
            // All our dependencies are build projects
            guidType = VSConstants.GUID_VS_DEPTYPE_BUILD_PROJECT;
            return VSConstants.S_OK;
        }

        public int get_Description(out string description)
        {
            description = null;
            return VSConstants.S_OK;
        }

        [CLSCompliant(false)]
        public int get_HelpContext(out uint helpContext)
        {
            helpContext = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int get_HelpFile(out string helpFile)
        {
            helpFile = null;
            return VSConstants.E_NOTIMPL;
        }

        public int get_MustUpdateBefore(out int mustUpdateBefore)
        {
            // Must always update dependencies
            mustUpdateBefore = 1;

            return VSConstants.S_OK;
        }

        public int get_ReferredProject(out object unknownProject)
        {
            unknownProject = null;

            unknownProject = GetReferencedHierarchy();

            // If we cannot find the referenced hierarchy return S_FALSE.
            return (unknownProject == null) ? VSConstants.S_FALSE : VSConstants.S_OK;
        }

        #endregion

        #region helper methods

        private IVsHierarchy GetReferencedHierarchy()
        {
            IVsHierarchy hierarchy = null;

            if (referencedProjectGuid == Guid.Empty || projectMgr == null || projectMgr.IsClosed)
            {
                return hierarchy;
            }

            return VsShellUtilities.GetHierarchy(projectMgr.Site, referencedProjectGuid);
        }

        #endregion
    }
}
