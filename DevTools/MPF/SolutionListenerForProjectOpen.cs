// SolutionListenerForProjectOpen.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider = System.IServiceProvider;

namespace Microsoft.VisualStudio.Project
{
    [CLSCompliant(false)]
    public class SolutionListenerForProjectOpen : SolutionListener
    {
        public SolutionListenerForProjectOpen(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override int OnAfterOpenProject(IVsHierarchy hierarchy, int added)
        {
            // If this is a new project and our project. We use here that it is only our project that will implemnet the "internal"  IBuildDependencyOnProjectContainer.
            if (added != 0 && hierarchy is IBuildDependencyUpdate)
            {
                IVsUIHierarchy uiHierarchy = hierarchy as IVsUIHierarchy;
                Debug.Assert(uiHierarchy != null, "The ProjectNode should implement IVsUIHierarchy");
                // Expand and select project node
                IVsUIHierarchyWindow uiWindow = UIHierarchyUtilities.GetUIHierarchyWindow(ServiceProvider,
                                                                                          HierarchyNode.SolutionExplorer);
                if (uiWindow != null)
                {
                    __VSHIERARCHYITEMSTATE state;
                    uint stateAsInt;
                    if (
                        uiWindow.GetItemState(uiHierarchy, VSConstants.VSITEMID_ROOT,
                                              (uint)__VSHIERARCHYITEMSTATE.HIS_Expanded, out stateAsInt) ==
                        VSConstants.S_OK)
                    {
                        state = (__VSHIERARCHYITEMSTATE)stateAsInt;
                        if (state != __VSHIERARCHYITEMSTATE.HIS_Expanded)
                        {
                            int hr;
                            hr = uiWindow.ExpandItem(uiHierarchy, VSConstants.VSITEMID_ROOT,
                                                     EXPANDFLAGS.EXPF_ExpandParentsToShowItem);
                            if (ErrorHandler.Failed(hr))
                                Trace.WriteLine("Failed to expand project node");
                            hr = uiWindow.ExpandItem(uiHierarchy, VSConstants.VSITEMID_ROOT, EXPANDFLAGS.EXPF_SelectItem);
                            if (ErrorHandler.Failed(hr))
                                Trace.WriteLine("Failed to select project node");

                            return hr;
                        }
                    }
                }
            }
            return VSConstants.S_OK;
        }
    }
}
