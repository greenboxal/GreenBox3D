// ProjectDesignerDocumentManager.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.Project
{
    internal class ProjectDesignerDocumentManager : DocumentManager
    {
        #region ctors

        public ProjectDesignerDocumentManager(ProjectNode node)
            : base(node)
        {
        }

        #endregion

        #region overriden methods

        public override int Open(ref Guid logicalView, IntPtr docDataExisting, out IVsWindowFrame windowFrame,
                                 WindowFrameShowAction windowFrameAction)
        {
            Guid editorGuid = VSConstants.GUID_ProjectDesignerEditor;
            return OpenWithSpecific(0, ref editorGuid, String.Empty, ref logicalView, docDataExisting, out windowFrame,
                                    windowFrameAction);
        }

        public override int OpenWithSpecific(uint editorFlags, ref Guid editorType, string physicalView,
                                             ref Guid logicalView, IntPtr docDataExisting, out IVsWindowFrame frame,
                                             WindowFrameShowAction windowFrameAction)
        {
            frame = null;
            Debug.Assert(editorType == VSConstants.GUID_ProjectDesignerEditor,
                         "Cannot open project designer with guid " + editorType.ToString());

            if (Node == null || Node.ProjectMgr == null || Node.ProjectMgr.IsClosed)
            {
                return VSConstants.E_FAIL;
            }

            IVsUIShellOpenDocument uiShellOpenDocument =
                Node.ProjectMgr.Site.GetService(typeof(SVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
            IOleServiceProvider serviceProvider =
                Node.ProjectMgr.Site.GetService(typeof(IOleServiceProvider)) as IOleServiceProvider;

            if (serviceProvider != null && uiShellOpenDocument != null)
            {
                string fullPath = GetFullPathForDocument();
                string caption = GetOwnerCaption();

                IVsUIHierarchy parentHierarchy =
                    Node.ProjectMgr.GetProperty((int)__VSHPROPID.VSHPROPID_ParentHierarchy) as IVsUIHierarchy;

                uint parentHierarchyItemId =
                    (uint)Node.ProjectMgr.GetProperty((int)__VSHPROPID.VSHPROPID_ParentHierarchyItemid);

                ErrorHandler.ThrowOnFailure(uiShellOpenDocument.OpenSpecificEditor(editorFlags, fullPath, ref editorType,
                                                                                   physicalView, ref logicalView,
                                                                                   caption, parentHierarchy,
                                                                                   parentHierarchyItemId,
                                                                                   docDataExisting, serviceProvider,
                                                                                   out frame));

                if (frame != null)
                {
                    if (windowFrameAction == WindowFrameShowAction.Show)
                    {
                        ErrorHandler.ThrowOnFailure(frame.Show());
                    }
                }
            }

            return VSConstants.S_OK;
        }

        #endregion
    }
}
