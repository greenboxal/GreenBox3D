// AssemblyReferenceNode.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Project.Automation;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using MSBuild = Microsoft.Build.Evaluation;
using MSBuildExecution = Microsoft.Build.Execution;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;

namespace Microsoft.VisualStudio.Project
{
    [CLSCompliant(false)]
    [ComVisible(true)]
    public class AssemblyReferenceNode : ReferenceNode
    {
        #region fieds

        /// <summary>
        ///     The name of the assembly this refernce represents
        /// </summary>
        private AssemblyName assemblyName;

        private string assemblyPath = String.Empty;

        /// <summary>
        ///     Defines the listener that would listen on file changes on the nested project node.
        /// </summary>
        private FileChangeManager fileChangeListener;

        /// <summary>
        ///     A flag for specifying if the object was disposed.
        /// </summary>
        private bool isDisposed;

        private AssemblyName resolvedAssemblyName;

        #endregion

        #region properties

        private OAAssemblyReference assemblyRef;

        /// <summary>
        ///     The name of the assembly this reference represents.
        /// </summary>
        /// <value></value>
        internal AssemblyName AssemblyName
        {
            get { return assemblyName; }
        }

        /// <summary>
        ///     Returns the name of the assembly this reference refers to on this specific
        ///     machine. It can be different from the AssemblyName property because it can
        ///     be more specific.
        /// </summary>
        internal AssemblyName ResolvedAssembly
        {
            get { return resolvedAssemblyName; }
        }

        public override string Url
        {
            get { return assemblyPath; }
        }

        public override string Caption
        {
            get { return assemblyName.Name; }
        }

        internal override object Object
        {
            get
            {
                if (null == assemblyRef)
                {
                    assemblyRef = new OAAssemblyReference(this);
                }
                return assemblyRef;
            }
        }

        #endregion

        #region ctors

        /// <summary>
        ///     Constructor for the ReferenceNode
        /// </summary>
        public AssemblyReferenceNode(ProjectNode root, ProjectElement element)
            : base(root, element)
        {
            GetPathNameFromProjectFile();

            InitializeFileChangeEvents();

            string include = ItemNode.GetMetadata(ProjectFileConstants.Include);

            CreateFromAssemblyName(new AssemblyName(include));
        }

        /// <summary>
        ///     Constructor for the AssemblyReferenceNode
        /// </summary>
        public AssemblyReferenceNode(ProjectNode root, string assemblyPath)
            : base(root)
        {
            // Validate the input parameters.
            if (null == root)
            {
                throw new ArgumentNullException("root");
            }
            if (string.IsNullOrEmpty(assemblyPath))
            {
                throw new ArgumentNullException("assemblyPath");
            }

            InitializeFileChangeEvents();

            // The assemblyPath variable can be an actual path on disk or a generic assembly name.
            if (File.Exists(assemblyPath))
            {
                // The assemblyPath parameter is an actual file on disk; try to load it.
                assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                this.assemblyPath = assemblyPath;

                // We register with listeningto chnages onteh path here. The rest of teh cases will call into resolving the assembly and registration is done there.
                fileChangeListener.ObserveItem(this.assemblyPath);
            }
            else
            {
                // The file does not exist on disk. This can be because the file / path is not
                // correct or because this is not a path, but an assembly name.
                // Try to resolve the reference as an assembly name.
                CreateFromAssemblyName(new AssemblyName(assemblyPath));
            }
        }

        #endregion

        #region methods

        /// <summary>
        ///     Closes the node.
        /// </summary>
        /// <returns></returns>
        public override int Close()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                base.Close();
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Links a reference node to the project and hierarchy.
        /// </summary>
        protected override void BindReferenceData()
        {
            Debug.Assert(assemblyName != null, "The AssemblyName field has not been initialized");

            // If the item has not been set correctly like in case of a new reference added it now.
            // The constructor for the AssemblyReference node will create a default project item. In that case the Item is null.
            // We need to specify here the correct project element. 
            if (ItemNode == null || ItemNode.Item == null)
            {
                ItemNode = new ProjectElement(ProjectMgr, assemblyName.FullName, ProjectFileConstants.Reference);
            }

            // Set the basic information we know about
            ItemNode.SetMetadata(ProjectFileConstants.Name, assemblyName.Name);
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                ItemNode.SetMetadata(ProjectFileConstants.AssemblyName, Path.GetFileName(assemblyPath));
            }
            else
            {
                ItemNode.SetMetadata(ProjectFileConstants.AssemblyName, null);
            }

            SetReferenceProperties();
        }

        /// <summary>
        ///     Disposes the node
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            try
            {
                UnregisterFromFileChangeService();
            }
            finally
            {
                base.Dispose(disposing);
                isDisposed = true;
            }
        }

        private void CreateFromAssemblyName(AssemblyName name)
        {
            assemblyName = name;

            // Use MsBuild to resolve the assemblyname 
            ResolveAssemblyReference();

            if (String.IsNullOrEmpty(assemblyPath) && (null != ItemNode.Item))
            {
                // Try to get the assembly name from the hintpath.
                GetPathNameFromProjectFile();
                if (assemblyPath == null)
                {
                    // Try to get the assembly name from the path
                    assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                }
            }
            if (null == resolvedAssemblyName)
            {
                resolvedAssemblyName = assemblyName;
            }
        }

        /// <summary>
        ///     Checks if an assembly is already added. The method parses all references and compares the full assemblynames, or the location of the assemblies to decide whether two assemblies are the same.
        /// </summary>
        /// <returns>true if the assembly has already been added.</returns>
        protected internal override bool IsAlreadyAdded(out ReferenceNode existingReference)
        {
            ReferenceContainerNode referencesFolder =
                ProjectMgr.FindChild(ReferenceContainerNode.ReferencesNodeVirtualName) as ReferenceContainerNode;
            Debug.Assert(referencesFolder != null, "Could not find the References node");
            bool shouldCheckPath = !string.IsNullOrEmpty(Url);

            for (HierarchyNode n = referencesFolder.FirstChild; n != null; n = n.NextSibling)
            {
                AssemblyReferenceNode assemblyReferenceNode = n as AssemblyReferenceNode;
                if (null != assemblyReferenceNode)
                {
                    // We will check if the full assemblynames are the same or if the Url of the assemblies is the same.
                    if (
                        String.Compare(assemblyReferenceNode.AssemblyName.FullName, assemblyName.FullName,
                                       StringComparison.OrdinalIgnoreCase) == 0 ||
                        (shouldCheckPath && NativeMethods.IsSamePath(assemblyReferenceNode.Url, Url)))
                    {
                        existingReference = assemblyReferenceNode;
                        return true;
                    }
                }
            }

            existingReference = null;
            return false;
        }

        /// <summary>
        ///     Determines if this is node a valid node for painting the default reference icon.
        /// </summary>
        /// <returns></returns>
        protected override bool CanShowDefaultIcon()
        {
            if (String.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
            {
                return false;
            }

            return true;
        }

        private void GetPathNameFromProjectFile()
        {
            string result = ItemNode.GetMetadata(ProjectFileConstants.HintPath);
            if (String.IsNullOrEmpty(result))
            {
                result = ItemNode.GetMetadata(ProjectFileConstants.AssemblyName);
                if (String.IsNullOrEmpty(result))
                {
                    assemblyPath = String.Empty;
                }
                else if (!result.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    result += ".dll";
                    assemblyPath = result;
                }
            }
            else
            {
                assemblyPath = GetFullPathFromPath(result);
            }
        }

        private string GetFullPathFromPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                Uri uri = new Uri(ProjectMgr.BaseURI.Uri, path);

                if (uri != null)
                {
                    return Shell.Url.Unescape(uri.LocalPath, true);
                }
            }

            return String.Empty;
        }

        protected override void ResolveReference()
        {
            ResolveAssemblyReference();
        }

        private void SetHintPathAndPrivateValue()
        {
            // Remove the HintPath, we will re-add it below if it is needed
            if (!String.IsNullOrEmpty(assemblyPath))
            {
                ItemNode.SetMetadata(ProjectFileConstants.HintPath, null);
            }

            // Get the list of items which require HintPath
            IEnumerable<ProjectItem> references =
                ProjectMgr.BuildProject.GetItems(MsBuildGeneratedItemType.ReferenceCopyLocalPaths);

            // Now loop through the generated References to find the corresponding one
            foreach (ProjectItem reference in references)
            {
                string fileName = Path.GetFileNameWithoutExtension(reference.EvaluatedInclude);
                if (String.Compare(fileName, assemblyName.Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    // We found it, now set some properties based on this.
                    string hintPath = reference.GetMetadataValue(ProjectFileConstants.HintPath);
                    SetHintPathAndPrivateValue(hintPath);
                    break;
                }
            }
        }

        /// <summary>
        ///     Sets the hint path to the provided value.
        ///     It also sets the private value to true if it has not been already provided through the associated project element.
        /// </summary>
        /// <param name="hintPath">The hint path to set.</param>
        private void SetHintPathAndPrivateValue(string hintPath)
        {
            if (String.IsNullOrEmpty(hintPath))
            {
                return;
            }

            if (Path.IsPathRooted(hintPath))
            {
                hintPath = PackageUtilities.GetPathDistance(ProjectMgr.BaseURI.Uri, new Uri(hintPath));
            }

            ItemNode.SetMetadata(ProjectFileConstants.HintPath, hintPath);

            // Private means local copy; we want to know if it is already set to not override the default
            string privateValue = ItemNode != null ? ItemNode.GetMetadata(ProjectFileConstants.Private) : null;

            // If this is not already set, we default to true
            if (String.IsNullOrEmpty(privateValue))
            {
                ItemNode.SetMetadata(ProjectFileConstants.Private, true.ToString());
            }
        }

        /// <summary>
        ///     This function ensures that some properties of the reference are set.
        /// </summary>
        private void SetReferenceProperties()
        {
            // If there is an assembly path then just set the hint path
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                SetHintPathAndPrivateValue(assemblyPath);
                return;
            }

            // Set a default HintPath for msbuild to be able to resolve the reference.
            ItemNode.SetMetadata(ProjectFileConstants.HintPath, assemblyPath);

            // Resolve assembly referernces. This is needed to make sure that properties like the full path
            // to the assembly or the hint path are set.
            if (ProjectMgr.Build(MsBuildTarget.ResolveAssemblyReferences) != MSBuildResult.Successful)
            {
                return;
            }

            // Check if we have to resolve again the path to the assembly.			
            ResolveReference();

            // Make sure that the hint path if set (if needed).
            SetHintPathAndPrivateValue();
        }

        /// <summary>
        ///     Does the actual job of resolving an assembly reference. We need a private method that does not violate
        ///     calling virtual method from the constructor.
        /// </summary>
        private void ResolveAssemblyReference()
        {
            if (ProjectMgr == null || ProjectMgr.IsClosed)
            {
                return;
            }

            var group = ProjectMgr.CurrentConfig.GetItems(ProjectFileConstants.ReferencePath);
            foreach (var item in group)
            {
                string fullPath = GetFullPathFromPath(item.EvaluatedInclude);

                AssemblyName name = AssemblyName.GetAssemblyName(fullPath);

                // Try with full assembly name and then with weak assembly name.
                if (String.Equals(name.FullName, assemblyName.FullName, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(name.Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (!NativeMethods.IsSamePath(fullPath, assemblyPath))
                    {
                        // set the full path now.
                        assemblyPath = fullPath;

                        // We have a new item to listen too, since the assembly reference is resolved from a different place.
                        fileChangeListener.ObserveItem(assemblyPath);
                    }

                    resolvedAssemblyName = name;

                    // No hint path is needed since the assembly path will always be resolved.
                    return;
                }
            }
        }

        /// <summary>
        ///     Registers with File change events
        /// </summary>
        private void InitializeFileChangeEvents()
        {
            fileChangeListener = new FileChangeManager(ProjectMgr.Site);
            fileChangeListener.FileChangedOnDisk += OnAssemblyReferenceChangedOnDisk;
        }

        /// <summary>
        ///     Unregisters this node from file change notifications.
        /// </summary>
        private void UnregisterFromFileChangeService()
        {
            fileChangeListener.FileChangedOnDisk -= OnAssemblyReferenceChangedOnDisk;
            fileChangeListener.Dispose();
        }

        /// <summary>
        ///     Event callback. Called when one of the assembly file is changed.
        /// </summary>
        /// <param name="sender">The FileChangeManager object.</param>
        /// <param name="e">Event args containing the file name that was updated.</param>
        private void OnAssemblyReferenceChangedOnDisk(object sender, FileChangedOnDiskEventArgs e)
        {
            Debug.Assert(e != null, "No event args specified for the FileChangedOnDisk event");

            // We only care about file deletes, so check for one before enumerating references.			
            if ((e.FileChangeFlag & _VSFILECHANGEFLAGS.VSFILECHG_Del) == 0)
            {
                return;
            }

            if (NativeMethods.IsSamePath(e.FileName, assemblyPath))
            {
                OnInvalidateItems(Parent);
            }
        }

        #endregion
    }
}
