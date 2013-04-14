// ProjectReferenceNode.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Project.Automation;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Constants = EnvDTE.Constants;

namespace Microsoft.VisualStudio.Project
{
    [CLSCompliant(false), ComVisible(true)]
    public class ProjectReferenceNode : ReferenceNode
    {
        #region fieds

        private readonly BuildDependency buildDependency;
        private readonly string referencedProjectFullPath = String.Empty;
        private readonly string referencedProjectName = String.Empty;

        private readonly string referencedProjectRelativePath = String.Empty;

        /// <summary>
        ///     This state is controlled by the solution events.
        ///     The state is set to false by OnBeforeUnloadProject.
        ///     The state is set to true by OnBeforeCloseProject event.
        /// </summary>
        private bool canRemoveReference = true;

        /// <summary>
        ///     Possibility for solution listener to update the state on the dangling reference.
        ///     It will be set in OnBeforeUnloadProject then the nopde is invalidated then it is reset to false.
        /// </summary>
        private bool isNodeValid;

        /// <summary>
        ///     This is a reference to the automation object for the referenced project.
        /// </summary>
        private EnvDTE.Project referencedProject;

        /// <summary>
        ///     The name of the assembly this refernce represents
        /// </summary>
        private Guid referencedProjectGuid;

        #endregion

        #region properties

        private OAProjectReference projectReference;

        public override string Url
        {
            get { return referencedProjectFullPath; }
        }

        public override string Caption
        {
            get { return referencedProjectName; }
        }

        internal Guid ReferencedProjectGuid
        {
            get { return referencedProjectGuid; }
        }

        /// <summary>
        ///     Possiblity to shortcut and set the dangling project reference icon.
        ///     It is ussually manipulated by solution listsneres who handle reference updates.
        /// </summary>
        protected internal bool IsNodeValid
        {
            get { return isNodeValid; }
            set { isNodeValid = value; }
        }

        /// <summary>
        ///     Controls the state whether this reference can be removed or not. Think of the project unload scenario where the project reference should not be deleted.
        /// </summary>
        internal bool CanRemoveReference
        {
            get { return canRemoveReference; }
            set { canRemoveReference = value; }
        }

        internal string ReferencedProjectName
        {
            get { return referencedProjectName; }
        }

        /// <summary>
        ///     Gets the automation object for the referenced project.
        /// </summary>
        internal EnvDTE.Project ReferencedProjectObject
        {
            get
            {
                // If the referenced project is null then re-read.
                if (referencedProject == null)
                {
                    // Search for the project in the collection of the projects in the
                    // current solution.
                    DTE dte = (DTE)ProjectMgr.GetService(typeof(DTE));
                    if ((null == dte) || (null == dte.Solution))
                    {
                        return null;
                    }
                    foreach (EnvDTE.Project prj in dte.Solution.Projects)
                    {
                        //Skip this project if it is an umodeled project (unloaded)
                        if (
                            string.Compare(Constants.vsProjectKindUnmodeled, prj.Kind,
                                           StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            continue;
                        }

                        // Get the full path of the current project.
                        Property pathProperty = null;
                        try
                        {
                            if (prj.Properties == null)
                            {
                                continue;
                            }

                            pathProperty = prj.Properties.Item("FullPath");
                            if (null == pathProperty)
                            {
                                // The full path should alway be availabe, but if this is not the
                                // case then we have to skip it.
                                continue;
                            }
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                        string prjPath = pathProperty.Value.ToString();
                        Property fileNameProperty = null;
                        // Get the name of the project file.
                        try
                        {
                            fileNameProperty = prj.Properties.Item("FileName");
                            if (null == fileNameProperty)
                            {
                                // Again, this should never be the case, but we handle it anyway.
                                continue;
                            }
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                        prjPath = Path.Combine(prjPath, fileNameProperty.Value.ToString());

                        // If the full path of this project is the same as the one of this
                        // reference, then we have found the right project.
                        if (NativeMethods.IsSamePath(prjPath, referencedProjectFullPath))
                        {
                            referencedProject = prj;
                            break;
                        }
                    }
                }

                return referencedProject;
            }
            set { referencedProject = value; }
        }

        /// <summary>
        ///     Gets the full path to the assembly generated by this project.
        /// </summary>
        internal string ReferencedProjectOutputPath
        {
            get
            {
                // Make sure that the referenced project implements the automation object.
                if (null == ReferencedProjectObject)
                {
                    return null;
                }

                // Get the configuration manager from the project.
                ConfigurationManager confManager = ReferencedProjectObject.ConfigurationManager;
                if (null == confManager)
                {
                    return null;
                }

                // Get the active configuration.
                Configuration config = confManager.ActiveConfiguration;
                if (null == config)
                {
                    return null;
                }

                // Get the output path for the current configuration.
                Property outputPathProperty = config.Properties.Item("OutputPath");
                if (null == outputPathProperty)
                {
                    return null;
                }

                string outputPath = outputPathProperty.Value.ToString();

                // Ususally the output path is relative to the project path, but it is possible
                // to set it as an absolute path. If it is not absolute, then evaluate its value
                // based on the project directory.
                if (!Path.IsPathRooted(outputPath))
                {
                    string projectDir = Path.GetDirectoryName(referencedProjectFullPath);
                    outputPath = Path.Combine(projectDir, outputPath);
                }

                // Now get the name of the assembly from the project.
                // Some project system throw if the property does not exist. We expect an ArgumentException.
                Property assemblyNameProperty = null;
                try
                {
                    assemblyNameProperty = ReferencedProjectObject.Properties.Item("OutputFileName");
                }
                catch (ArgumentException)
                {
                }

                if (null == assemblyNameProperty)
                {
                    return null;
                }
                // build the full path adding the name of the assembly to the output path.
                outputPath = Path.Combine(outputPath, assemblyNameProperty.Value.ToString());

                return outputPath;
            }
        }

        internal override object Object
        {
            get
            {
                if (null == projectReference)
                {
                    projectReference = new OAProjectReference(this);
                }
                return projectReference;
            }
        }

        #endregion

        #region ctors

        /// <summary>
        ///     Constructor for the ReferenceNode. It is called when the project is reloaded, when the project element representing the refernce exists.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public ProjectReferenceNode(ProjectNode root, ProjectElement element)
            : base(root, element)
        {
            referencedProjectRelativePath = ItemNode.GetMetadata(ProjectFileConstants.Include);
            Debug.Assert(!String.IsNullOrEmpty(referencedProjectRelativePath),
                         "Could not retrive referenced project path form project file");

            string guidString = ItemNode.GetMetadata(ProjectFileConstants.Project);

            // Continue even if project setttings cannot be read.
            try
            {
                referencedProjectGuid = new Guid(guidString);

                buildDependency = new BuildDependency(ProjectMgr, referencedProjectGuid);
                ProjectMgr.AddBuildDependency(buildDependency);
            }
            finally
            {
                Debug.Assert(referencedProjectGuid != Guid.Empty,
                             "Could not retrive referenced project guidproject file");

                referencedProjectName = ItemNode.GetMetadata(ProjectFileConstants.Name);

                Debug.Assert(!String.IsNullOrEmpty(referencedProjectName),
                             "Could not retrive referenced project name form project file");
            }

            Uri uri = new Uri(ProjectMgr.BaseURI.Uri, referencedProjectRelativePath);

            if (uri != null)
            {
                referencedProjectFullPath = Shell.Url.Unescape(uri.LocalPath, true);
            }
        }

        /// <summary>
        ///     constructor for the ProjectReferenceNode
        /// </summary>
        public ProjectReferenceNode(ProjectNode root, string referencedProjectName, string projectPath,
                                    string projectReference)
            : base(root)
        {
            Debug.Assert(
                root != null && !String.IsNullOrEmpty(referencedProjectName) && !String.IsNullOrEmpty(projectReference)
                && !String.IsNullOrEmpty(projectPath),
                "Can not add a reference because the input for adding one is invalid.");

            if (projectReference == null)
            {
                throw new ArgumentNullException("projectReference");
            }

            this.referencedProjectName = referencedProjectName;

            int indexOfSeparator = projectReference.IndexOf('|');

            string fileName = String.Empty;

            // Unfortunately we cannot use the path part of the projectReference string since it is not resolving correctly relative pathes.
            if (indexOfSeparator != -1)
            {
                string projectGuid = projectReference.Substring(0, indexOfSeparator);
                referencedProjectGuid = new Guid(projectGuid);
                if (indexOfSeparator + 1 < projectReference.Length)
                {
                    string remaining = projectReference.Substring(indexOfSeparator + 1);
                    indexOfSeparator = remaining.IndexOf('|');

                    if (indexOfSeparator == -1)
                    {
                        fileName = remaining;
                    }
                    else
                    {
                        fileName = remaining.Substring(0, indexOfSeparator);
                    }
                }
            }

            Debug.Assert(!String.IsNullOrEmpty(fileName),
                         "Can not add a project reference because the input for adding one is invalid.");

            // Did we get just a file or a relative path?
            Uri uri = new Uri(projectPath);

            string referenceDir = PackageUtilities.GetPathDistance(ProjectMgr.BaseURI.Uri, uri);

            Debug.Assert(!String.IsNullOrEmpty(referenceDir),
                         "Can not add a project reference because the input for adding one is invalid.");

            string justTheFileName = Path.GetFileName(fileName);
            referencedProjectRelativePath = Path.Combine(referenceDir, justTheFileName);

            referencedProjectFullPath = Path.Combine(projectPath, justTheFileName);

            buildDependency = new BuildDependency(ProjectMgr, referencedProjectGuid);
        }

        #endregion

        #region methods

        protected override NodeProperties CreatePropertiesObject()
        {
            return new ProjectReferencesProperties(this);
        }

        /// <summary>
        ///     The node is added to the hierarchy and then updates the build dependency list.
        /// </summary>
        public override void AddReference()
        {
            if (ProjectMgr == null)
            {
                return;
            }
            base.AddReference();
            ProjectMgr.AddBuildDependency(buildDependency);
            return;
        }

        /// <summary>
        ///     Overridden method. The method updates the build dependency list before removing the node from the hierarchy.
        /// </summary>
        public override void Remove(bool removeFromStorage)
        {
            if (ProjectMgr == null || !CanRemoveReference)
            {
                return;
            }
            ProjectMgr.RemoveBuildDependency(buildDependency);
            base.Remove(removeFromStorage);
            return;
        }

        /// <summary>
        ///     Links a reference node to the project file.
        /// </summary>
        protected override void BindReferenceData()
        {
            Debug.Assert(!String.IsNullOrEmpty(referencedProjectName),
                         "The referencedProjectName field has not been initialized");
            Debug.Assert(referencedProjectGuid != Guid.Empty, "The referencedProjectName field has not been initialized");

            ItemNode = new ProjectElement(ProjectMgr, referencedProjectRelativePath,
                                          ProjectFileConstants.ProjectReference);

            ItemNode.SetMetadata(ProjectFileConstants.Name, referencedProjectName);
            ItemNode.SetMetadata(ProjectFileConstants.Project, referencedProjectGuid.ToString("B"));
            ItemNode.SetMetadata(ProjectFileConstants.Private, true.ToString());
        }

        /// <summary>
        ///     Defines whether this node is valid node for painting the refererence icon.
        /// </summary>
        /// <returns></returns>
        protected override bool CanShowDefaultIcon()
        {
            if (referencedProjectGuid == Guid.Empty || ProjectMgr == null || ProjectMgr.IsClosed || isNodeValid)
            {
                return false;
            }

            IVsHierarchy hierarchy = null;

            hierarchy = VsShellUtilities.GetHierarchy(ProjectMgr.Site, referencedProjectGuid);

            if (hierarchy == null)
            {
                return false;
            }

            //If the Project is unloaded return false
            if (ReferencedProjectObject == null)
            {
                return false;
            }

            return (!String.IsNullOrEmpty(referencedProjectFullPath) && File.Exists(referencedProjectFullPath));
        }

        /// <summary>
        ///     Checks if a project reference can be added to the hierarchy. It calls base to see if the reference is not already there, then checks for circular references.
        /// </summary>
        /// <param name="errorHandler">The error handler delegate to return</param>
        /// <returns></returns>
        protected override bool CanAddReference(out CannotAddReferenceErrorMessage errorHandler)
        {
            // When this method is called this refererence has not yet been added to the hierarchy, only instantiated.
            if (!base.CanAddReference(out errorHandler))
            {
                return false;
            }

            errorHandler = null;
            if (IsThisProjectReferenceInCycle())
            {
                errorHandler = ShowCircularReferenceErrorMessage;
                return false;
            }

            return true;
        }

        private bool IsThisProjectReferenceInCycle()
        {
            return IsReferenceInCycle(referencedProjectGuid);
        }

        private void ShowCircularReferenceErrorMessage()
        {
            string message = String.Format(CultureInfo.CurrentCulture,
                                           SR.GetString(SR.ProjectContainsCircularReferences,
                                                        CultureInfo.CurrentUICulture), referencedProjectName);
            string title = string.Empty;
            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
            OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
            OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
            VsShellUtilities.ShowMessageBox(ProjectMgr.Site, title, message, icon, buttons, defaultButton);
        }

        /// <summary>
        ///     Checks whether a reference added to a given project would introduce a circular dependency.
        /// </summary>
        private bool IsReferenceInCycle(Guid projectGuid)
        {
            IVsHierarchy referencedHierarchy = VsShellUtilities.GetHierarchy(ProjectMgr.Site, projectGuid);

            var solutionBuildManager =
                ProjectMgr.Site.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager2;
            if (solutionBuildManager == null)
            {
                throw new InvalidOperationException("Cannot find the IVsSolutionBuildManager2 service.");
            }

            int circular;
            Marshal.ThrowExceptionForHR(solutionBuildManager.CalculateProjectDependencies());
            Marshal.ThrowExceptionForHR(solutionBuildManager.QueryProjectDependency(referencedHierarchy,
                                                                                    ProjectMgr.InteropSafeIVsHierarchy,
                                                                                    out circular));

            return circular != 0;
        }

        #endregion
    }
}
