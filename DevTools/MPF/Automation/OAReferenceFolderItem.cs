// OAReferenceFolderItem.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;

namespace Microsoft.VisualStudio.Project.Automation
{
    /// <summary>
    ///     Contains OAReferenceItem objects
    /// </summary>
    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    [ComVisible(true), CLSCompliant(false)]
    public class OAReferenceFolderItem : OAProjectItem<ReferenceContainerNode>
    {
        #region ctors

        public OAReferenceFolderItem(OAProject project, ReferenceContainerNode node)
            : base(project, node)
        {
        }

        #endregion

        #region overridden methods

        /// <summary>
        ///     Returns the project items collection of all the references defined for this project.
        /// </summary>
        public override ProjectItems ProjectItems
        {
            get { return new OANavigableProjectItems(Project, GetListOfProjectItems(), Node); }
        }

        #endregion

        #region Helper methods

        private List<ProjectItem> GetListOfProjectItems()
        {
            List<ProjectItem> list = new List<ProjectItem>();
            for (HierarchyNode child = Node.FirstChild; child != null; child = child.NextSibling)
            {
                ReferenceNode node = child as ReferenceNode;

                if (node != null)
                {
                    list.Add(new OAReferenceItem(Project, node));
                }
            }

            return list;
        }

        #endregion
    }
}
