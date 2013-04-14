// OAFolderItem.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;

namespace Microsoft.VisualStudio.Project.Automation
{
    /// <summary>
    ///     Represents an automation object for a folder in a project
    /// </summary>
    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    [ComVisible(true), CLSCompliant(false)]
    public class OAFolderItem : OAProjectItem<FolderNode>
    {
        #region ctors

        public OAFolderItem(OAProject project, FolderNode node)
            : base(project, node)
        {
        }

        #endregion

        #region overridden methods

        public override ProjectItems Collection
        {
            get
            {
                return UIThread.DoOnUIThread(delegate
                {
                    ProjectItems items = new OAProjectItems(Project, Node);
                    return items;
                });
            }
        }

        public override ProjectItems ProjectItems
        {
            get { return Collection; }
        }

        #endregion
    }
}
