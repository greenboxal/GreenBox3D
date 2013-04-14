// OASolutionFolder.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project.Automation
{
    [ComVisible(true), CLSCompliant(false)]
    public class OASolutionFolder<T> : SolutionFolder
        where T : HierarchyNode
    {
        private readonly T node;
        private bool hidden;

        public OASolutionFolder(T associatedNode)
        {
            if (associatedNode == null)
            {
                throw new ArgumentNullException("associatedNode");
            }

            Debug.Assert(associatedNode.ProjectMgr is ProjectContainerNode,
                         "Expecting obejct of type" + typeof(ProjectContainerNode).Name);

            if (!(associatedNode.ProjectMgr is ProjectContainerNode))
                throw new ArgumentException(SR.GetString(SR.InvalidParameter, CultureInfo.CurrentUICulture),
                                            "associatedNode");

            node = associatedNode;
        }

        #region SolutionFolder Members

        public virtual EnvDTE.Project AddFromFile(string fileName)
        {
            ProjectContainerNode projectContainer = (ProjectContainerNode)node.ProjectMgr;
            ProjectElement newElement = new ProjectElement(projectContainer, fileName, ProjectFileConstants.SubProject);
            NestedProjectNode newNode = projectContainer.AddExistingNestedProject(newElement,
                                                                                  __VSCREATEPROJFLAGS.CPF_NOTINSLNEXPLR |
                                                                                  __VSCREATEPROJFLAGS.CPF_SILENT |
                                                                                  __VSCREATEPROJFLAGS.CPF_OPENFILE);
            if (newNode == null)
                return null;
            // Now that the sub project was created, get its extensibility object so we can return it
            object newProject = null;
            if (
                ErrorHandler.Succeeded(newNode.NestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT,
                                                                           (int)__VSHPROPID.VSHPROPID_ExtObject,
                                                                           out newProject)))
                return newProject as EnvDTE.Project;
            else
                return null;
        }

        public virtual EnvDTE.Project AddFromTemplate(string fileName, string destination, string projectName)
        {
            bool isVSTemplate = Utilities.IsTemplateFile(fileName);

            NestedProjectNode newNode = null;
            if (isVSTemplate)
            {
                // Get the wizard to run, we will get called again and use the alternate code path
                ProjectElement newElement = new ProjectElement(node.ProjectMgr, Path.Combine(destination, projectName),
                                                               ProjectFileConstants.SubProject);
                newElement.SetMetadata(ProjectFileConstants.Template, fileName);
                ((ProjectContainerNode)node.ProjectMgr).RunVsTemplateWizard(newElement, false);
            }
            else
            {
                if ((String.IsNullOrEmpty(Path.GetExtension(projectName))))
                {
                    string targetExtension = Path.GetExtension(fileName);
                    projectName = Path.ChangeExtension(projectName, targetExtension);
                }

                ProjectContainerNode projectContainer = (ProjectContainerNode)node.ProjectMgr;
                newNode = projectContainer.AddNestedProjectFromTemplate(fileName, destination, projectName, null,
                                                                        __VSCREATEPROJFLAGS.CPF_NOTINSLNEXPLR |
                                                                        __VSCREATEPROJFLAGS.CPF_SILENT |
                                                                        __VSCREATEPROJFLAGS.CPF_CLONEFILE);
            }
            if (newNode == null)
                return null;

            // Now that the sub project was created, get its extensibility object so we can return it
            object newProject = null;
            if (
                ErrorHandler.Succeeded(newNode.NestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT,
                                                                           (int)__VSHPROPID.VSHPROPID_ExtObject,
                                                                           out newProject)))
                return newProject as EnvDTE.Project;
            else
                return null;
        }

        public virtual EnvDTE.Project AddSolutionFolder(string Name)
        {
            throw new NotImplementedException();
        }

        public virtual EnvDTE.Project Parent
        {
            get { throw new NotImplementedException(); }
        }

        public virtual bool Hidden
        {
            get { return hidden; }
            set { hidden = value; }
        }

        public virtual DTE DTE
        {
            get { return (DTE)node.ProjectMgr.Site.GetService(typeof(DTE)); }
        }

        #endregion
    }
}
