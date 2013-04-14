// OAReferenceItem.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;

namespace Microsoft.VisualStudio.Project.Automation
{
    /// <summary>
    ///     Represents the automation object equivalent to a ReferenceNode object
    /// </summary>
    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    [ComVisible(true), CLSCompliant(false)]
    public class OAReferenceItem : OAProjectItem<ReferenceNode>
    {
        #region ctors

        public OAReferenceItem(OAProject project, ReferenceNode node)
            : base(project, node)
        {
        }

        #endregion

        #region overridden methods

        /// <summary>
        ///     Gets or sets the name of the object.
        /// </summary>
        public override string Name
        {
            get { return base.Name; }
            set { throw new InvalidOperationException(); }
        }

        /// <summary>
        ///     Gets the ProjectItems collection containing the ProjectItem object supporting this property.
        /// </summary>
        public override ProjectItems Collection
        {
            get
            {
                // Get the parent node (ReferenceContainerNode)
                ReferenceContainerNode parentNode = Node.Parent as ReferenceContainerNode;
                Debug.Assert(parentNode != null, "Failed to get the parent node");

                // Get the ProjectItems object for the parent node
                if (parentNode != null)
                {
                    // The root node for the project
                    return ((OAReferenceFolderItem)parentNode.GetAutomationObject()).ProjectItems;
                }

                return null;
            }
        }

        /// <summary>
        ///     Not implemented. If called throws invalid operation exception.
        /// </summary>
        public override void Delete()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        ///     Not implemented. If called throws invalid operation exception.
        /// </summary>
        /// <param name="viewKind"> A Constants. vsViewKind indicating the type of view to use.</param>
        /// <returns></returns>
        public override Window Open(string viewKind)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}
