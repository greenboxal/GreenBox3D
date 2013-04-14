// OANavigableProjectItems.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;

namespace Microsoft.VisualStudio.Project.Automation
{
    /// <summary>
    ///     This can navigate a collection object only (partial implementation of ProjectItems interface)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [ComVisible(true), CLSCompliant(false)]
    public class OANavigableProjectItems : ProjectItems
    {
        #region fields

        private readonly IList<ProjectItem> items;
        private readonly HierarchyNode nodeWithItems;
        private readonly OAProject project;

        #endregion

        #region properties

        /// <summary>
        ///     Defines an internal list of project items
        /// </summary>
        internal IList<ProjectItem> Items
        {
            get { return items; }
        }

        /// <summary>
        ///     Defines a relationship to the associated project.
        /// </summary>
        internal OAProject Project
        {
            get { return project; }
        }

        /// <summary>
        ///     Defines the node that contains the items
        /// </summary>
        internal HierarchyNode NodeWithItems
        {
            get { return nodeWithItems; }
        }

        #endregion

        #region ctor

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="project">The associated project.</param>
        /// <param name="nodeWithItems">The node that defines the items.</param>
        public OANavigableProjectItems(OAProject project, HierarchyNode nodeWithItems)
        {
            this.project = project;
            this.nodeWithItems = nodeWithItems;
            items = GetListOfProjectItems();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="project">The associated project.</param>
        /// <param name="items">A list of items that will make up the items defined by this object.</param>
        /// <param name="nodeWithItems">The node that defines the items.</param>
        public OANavigableProjectItems(OAProject project, IList<ProjectItem> items, HierarchyNode nodeWithItems)
        {
            this.items = items;
            this.project = project;
            this.nodeWithItems = nodeWithItems;
        }

        #endregion

        #region EnvDTE.ProjectItems

        /// <summary>
        ///     Gets a value indicating the number of objects in the collection.
        /// </summary>
        public virtual int Count
        {
            get { return items.Count; }
        }

        /// <summary>
        ///     Gets the immediate parent object of a ProjectItems collection.
        /// </summary>
        public virtual object Parent
        {
            get { return nodeWithItems.GetAutomationObject(); }
        }

        /// <summary>
        ///     Gets an enumeration indicating the type of object.
        /// </summary>
        public virtual string Kind
        {
            get
            {
                // TODO:  Add OAProjectItems.Kind getter implementation
                return null;
            }
        }

        /// <summary>
        ///     Gets the top-level extensibility object.
        /// </summary>
        public virtual DTE DTE
        {
            get { return project.DTE; }
        }

        /// <summary>
        ///     Gets the project hosting the project item or items.
        /// </summary>
        public virtual EnvDTE.Project ContainingProject
        {
            get { return project; }
        }

        /// <summary>
        ///     Adds one or more ProjectItem objects from a directory to the ProjectItems collection.
        /// </summary>
        /// <param name="directory">The directory from which to add the project item.</param>
        /// <returns>A ProjectItem object.</returns>
        public virtual ProjectItem AddFromDirectory(string directory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Creates a new project item from an existing item template file and adds it to the project.
        /// </summary>
        /// <param name="fileName">The full path and file name of the template project file.</param>
        /// <param name="name">The file name to use for the new project item.</param>
        /// <returns>A ProjectItem object. </returns>
        public virtual ProjectItem AddFromTemplate(string fileName, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Creates a new folder in Solution Explorer.
        /// </summary>
        /// <param name="name">The name of the folder node in Solution Explorer.</param>
        /// <param name="kind">The type of folder to add. The available values are based on vsProjectItemsKindConstants and vsProjectItemKindConstants</param>
        /// <returns>A ProjectItem object.</returns>
        public virtual ProjectItem AddFolder(string name, string kind)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Copies a source file and adds it to the project.
        /// </summary>
        /// <param name="filePath">The path and file name of the project item to be added.</param>
        /// <returns>A ProjectItem object. </returns>
        public virtual ProjectItem AddFromFileCopy(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Adds a project item from a file that is installed in a project directory structure.
        /// </summary>
        /// <param name="fileName">The file name of the item to add as a project item. </param>
        /// <returns>A ProjectItem object. </returns>
        public virtual ProjectItem AddFromFile(string fileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Get Project Item from index
        /// </summary>
        /// <param name="index">Either index by number (1-based) or by name can be used to get the item</param>
        /// <returns>Project Item. null is return if invalid index is specified</returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public virtual ProjectItem Item(object index)
        {
            if (index is int)
            {
                int realIndex = (int)index - 1;
                if (realIndex >= 0 && realIndex < items.Count)
                {
                    return items[realIndex];
                }
                return null;
            }
            else if (index is string)
            {
                string name = (string)index;
                foreach (ProjectItem item in items)
                {
                    if (String.Compare(item.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Returns an enumeration for items in a collection.
        /// </summary>
        /// <returns>An IEnumerator for this object.</returns>
        public virtual IEnumerator GetEnumerator()
        {
            if (items == null)
            {
                yield return null;
            }

            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                yield return items[i];
            }
        }

        #endregion

        #region virtual methods

        /// <summary>
        ///     Retrives a list of items associated with the current node.
        /// </summary>
        /// <returns>A List of project items</returns>
        protected IList<ProjectItem> GetListOfProjectItems()
        {
            return UIThread.DoOnUIThread(delegate
            {
                List<ProjectItem> list = new List<ProjectItem>();
                for (HierarchyNode child = NodeWithItems.FirstChild; child != null; child = child.NextSibling)
                {
                    ProjectItem item = child.GetAutomationObject() as ProjectItem;
                    if (null != item)
                    {
                        list.Add(item);
                    }
                }

                return list;
            });
        }

        #endregion
    }
}
