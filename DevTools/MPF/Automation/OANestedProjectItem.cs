// OANestedProjectItem.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project.Automation
{
    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    [ComVisible(true), CLSCompliant(false)]
    public class OANestedProjectItem : OAProjectItem<NestedProjectNode>
    {
        #region fields

        private readonly EnvDTE.Project nestedProject;

        #endregion

        #region ctors

        public OANestedProjectItem(OAProject project, NestedProjectNode node)
            : base(project, node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            object nestedproject;
            if (
                ErrorHandler.Succeeded(node.NestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT,
                                                                        (int)__VSHPROPID.VSHPROPID_ExtObject,
                                                                        out nestedproject)))
            {
                nestedProject = nestedproject as EnvDTE.Project;
            }
        }

        #endregion

        #region overridden methods

        /// <summary>
        ///     Returns the collection of project items defined in the nested project
        /// </summary>
        public override ProjectItems ProjectItems
        {
            get
            {
                if (nestedProject != null)
                {
                    return nestedProject.ProjectItems;
                }
                return null;
            }
        }

        /// <summary>
        ///     Returns the nested project.
        /// </summary>
        public override EnvDTE.Project SubProject
        {
            get { return nestedProject; }
        }

        #endregion
    }
}
