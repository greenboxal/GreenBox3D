// NestedProjectNode.cs
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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Project.Automation;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj80;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;

namespace Microsoft.VisualStudio.Project
{
    [CLSCompliant(false), ComVisible(true)]
    public class NestedProjectNode : HierarchyNode, IPropertyNotifySink
    {
        #region fields

        /// <summary>
        ///     Defines an object that will be a mutex for this object for synchronizing thread calls.
        /// </summary>
        private static volatile object Mutex = new object();

        private ImageHandler imageHandler;

        /// <summary>
        ///     Sets the dispose flag on the object.
        /// </summary>
        private bool isDisposed;

        private IVsHierarchy nestedHierarchy;

        private Guid projectInstanceGuid = Guid.Empty;

        private string projectName = String.Empty;

        private string projectPath = String.Empty;

        // A cooike retrieved when advising on property chnanged events.
        private uint projectPropertyNotifySinkCookie;

        #endregion

        #region properties

        internal IVsHierarchy NestedHierarchy
        {
            get { return nestedHierarchy; }
        }

        #endregion

        #region virtual properties

        /// <summary>
        ///     Returns the __VSADDVPFLAGS that will be passed in when calling AddVirtualProjectEx
        /// </summary>
        protected virtual uint VirtualProjectFlags
        {
            get { return 0; }
        }

        #endregion

        #region overridden properties

        /// <summary>
        ///     The path of the nested project.
        /// </summary>
        public override string Url
        {
            get { return projectPath; }
        }

        /// <summary>
        ///     The Caption of the nested project.
        /// </summary>
        public override string Caption
        {
            get { return Path.GetFileNameWithoutExtension(projectName); }
        }

        public override Guid ItemTypeGuid
        {
            get { return VSConstants.GUID_ItemType_SubProject; }
        }

        /// <summary>
        ///     Defines whether a node can execute a command if in selection.
        ///     We do this in order to let the nested project to handle the execution of its own commands.
        /// </summary>
        public override bool CanExecuteCommand
        {
            get { return false; }
        }

        public override int SortPriority
        {
            get { return DefaultSortOrderNode.NestedProjectNode; }
        }

        protected bool IsDisposed
        {
            get { return isDisposed; }
            set { isDisposed = value; }
        }

        #endregion

        #region ctor

        protected NestedProjectNode()
        {
        }

        public NestedProjectNode(ProjectNode root, ProjectElement element)
            : base(root, element)
        {
            IsExpanded = true;
        }

        #endregion

        #region IPropertyNotifySink Members

        /// <summary>
        ///     Notifies a sink that the [bindable] property specified by dispID has changed.
        ///     If dispID is DISPID_UNKNOWN, then multiple properties have changed together.
        ///     The client (owner of the sink) should then retrieve the current value of each property of interest from the object that generated the notification.
        ///     In our case we will care about the  VSLangProj80.VsProjPropId.VBPROJPROPID_FileName and update the changes in the parent project file.
        /// </summary>
        /// <param name="dispid">Dispatch identifier of the property that is about to change or DISPID_UNKNOWN if multiple properties are about to change.</param>
        public virtual void OnChanged(int dispid)
        {
            if (dispid == (int)VsProjPropId.VBPROJPROPID_FileName)
            {
                // Get the filename of the nested project. Inetead of asking the label on the nested we ask the filename, since the label might not yet been set.
                IVsProject3 nestedProject = nestedHierarchy as IVsProject3;

                if (nestedProject != null)
                {
                    string document;
                    ErrorHandler.ThrowOnFailure(nestedProject.GetMkDocument(VSConstants.VSITEMID_ROOT, out document));
                    RenameNestedProjectInParentProject(Path.GetFileNameWithoutExtension(document));

                    // We need to redraw the caption since for some reason, by intervining to the OnChanged event the Caption is not updated.
                    ReDraw(UIHierarchyElement.Caption);
                }
            }
        }

        /// <summary>
        ///     Notifies a sink that a [requestedit] property is about to change and that the object is asking the sink how to proceed.
        /// </summary>
        /// <param name="dispid">Dispatch identifier of the property that is about to change or DISPID_UNKNOWN if multiple properties are about to change.</param>
        public virtual void OnRequestEdit(int dispid)
        {
        }

        #endregion

        #region public methods

        #endregion

        #region overridden methods

        /// <summary>
        ///     Get the automation object for the NestedProjectNode
        /// </summary>
        /// <returns>An instance of the Automation.OANestedProjectItem type if succeded</returns>
        public override object GetAutomationObject()
        {
            //Validate that we are not disposed or the project is closing
            if (isDisposed || ProjectMgr == null || ProjectMgr.IsClosed)
            {
                return null;
            }

            return new OANestedProjectItem(ProjectMgr.GetAutomationObject() as OAProject, this);
        }

        /// <summary>
        ///     Gets properties of a given node or of the hierarchy.
        /// </summary>
        /// <param name="propId">Identifier of the hierarchy property</param>
        /// <returns>It return an object which type is dependent on the propid.</returns>
        public override object GetProperty(int propId)
        {
            __VSHPROPID vshPropId = (__VSHPROPID)propId;
            switch (vshPropId)
            {
                default:
                    return base.GetProperty(propId);

                case __VSHPROPID.VSHPROPID_Expandable:
                    return true;

                case __VSHPROPID.VSHPROPID_BrowseObject:
                case __VSHPROPID.VSHPROPID_HandlesOwnReload:
                    return DelegateGetPropertyToNested(propId);
            }
        }

        /// <summary>
        ///     Gets properties whose values are GUIDs.
        /// </summary>
        /// <param name="propid">Identifier of the hierarchy property</param>
        /// <param name="guid"> Pointer to a GUID property specified in propid</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public override int GetGuidProperty(int propid, out Guid guid)
        {
            guid = Guid.Empty;
            switch ((__VSHPROPID)propid)
            {
                case __VSHPROPID.VSHPROPID_ProjectIDGuid:
                    guid = projectInstanceGuid;
                    break;

                default:
                    return base.GetGuidProperty(propid, out guid);
            }

            CCITracing.TraceCall(String.Format(CultureInfo.CurrentCulture, "Guid for {0} property", propid));
            if (guid.CompareTo(Guid.Empty) == 0)
            {
                return VSConstants.DISP_E_MEMBERNOTFOUND;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Determines whether the hierarchy item changed.
        /// </summary>
        /// <param name="itemId">Item identifier of the hierarchy item contained in VSITEMID</param>
        /// <param name="punkDocData">Pointer to the IUnknown interface of the hierarchy item. </param>
        /// <param name="pfDirty">TRUE if the hierarchy item changed.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public override int IsItemDirty(uint itemId, IntPtr punkDocData, out int pfDirty)
        {
            Debug.Assert(nestedHierarchy != null,
                         "The nested hierarchy object must be created before calling this method");
            Debug.Assert(punkDocData != IntPtr.Zero, "docData intptr was zero");

            // Get an IPersistFileFormat object from docData object 
            IPersistFileFormat persistFileFormat =
                Marshal.GetTypedObjectForIUnknown(punkDocData, typeof(IPersistFileFormat)) as IPersistFileFormat;
            Debug.Assert(persistFileFormat != null,
                         "The docData object does not implement the IPersistFileFormat interface");

            // Call IsDirty on the IPersistFileFormat interface
            ErrorHandler.ThrowOnFailure(persistFileFormat.IsDirty(out pfDirty));

            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Saves the hierarchy item to disk.
        /// </summary>
        /// <param name="dwSave">Flags whose values are taken from the VSSAVEFLAGS enumeration.</param>
        /// <param name="silentSaveAsName">File name to be applied when dwSave is set to VSSAVE_SilentSave. </param>
        /// <param name="itemid">Item identifier of the hierarchy item saved from VSITEMID. </param>
        /// <param name="punkDocData">Pointer to the IUnknown interface of the hierarchy item saved.</param>
        /// <param name="pfCancelled">TRUE if the save action was canceled. </param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public override int SaveItem(VSSAVEFLAGS dwSave, string silentSaveAsName, uint itemid, IntPtr punkDocData,
                                     out int pfCancelled)
        {
            // Don't ignore/unignore file changes 
            // Use Advise/Unadvise to work around rename situations
            try
            {
                StopObservingNestedProjectFile();
                Debug.Assert(nestedHierarchy != null,
                             "The nested hierarchy object must be created before calling this method");
                Debug.Assert(punkDocData != IntPtr.Zero, "docData intptr was zero");

                // Get an IPersistFileFormat object from docData object (we don't call release on punkDocData since did not increment its ref count)
                IPersistFileFormat persistFileFormat =
                    Marshal.GetTypedObjectForIUnknown(punkDocData, typeof(IPersistFileFormat)) as IPersistFileFormat;
                Debug.Assert(persistFileFormat != null,
                             "The docData object does not implement the IPersistFileFormat interface");

                IVsUIShell uiShell = GetService(typeof(SVsUIShell)) as IVsUIShell;
                string newName;
                ErrorHandler.ThrowOnFailure(uiShell.SaveDocDataToFile(dwSave, persistFileFormat, silentSaveAsName,
                                                                      out newName, out pfCancelled));

                // When supported do a rename of the nested project here 
            }
            finally
            {
                // Succeeded or not we must hook to the file change events
                // Don't ignore/unignore file changes 
                // Use Advise/Unadvise to work around rename situations
                ObserveNestedProjectFile();
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Gets the icon handle. It tries first the nested to get the icon handle. If that is not supported it will get it from
        ///     the image list of the nested if that is supported. If neither of these is supported a default image will be shown.
        /// </summary>
        /// <returns>An object representing the icon.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults",
            MessageId =
                "Microsoft.VisualStudio.Shell.Interop.IVsHierarchy.GetProperty(System.UInt32,System.Int32,System.Object@)"
            )]
        public override object GetIconHandle(bool open)
        {
            Debug.Assert(nestedHierarchy != null,
                         "The nested hierarchy object must be created before calling this method");

            object iconHandle = null;
            nestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_IconHandle, out iconHandle);
            if (iconHandle == null)
            {
                if (null == imageHandler)
                {
                    InitImageHandler();
                }
                // Try to get an icon from the nested hierrachy image list.
                if (imageHandler.ImageList != null)
                {
                    object imageIndexAsObject = null;
                    if (
                        nestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_IconIndex,
                                                    out imageIndexAsObject) == VSConstants.S_OK &&
                        imageIndexAsObject != null)
                    {
                        int imageIndex = (int)imageIndexAsObject;
                        if (imageIndex < imageHandler.ImageList.Images.Count)
                        {
                            iconHandle = imageHandler.GetIconHandle(imageIndex);
                        }
                    }
                }

                if (null == iconHandle)
                {
                    iconHandle = ProjectMgr.ImageHandler.GetIconHandle((int)ProjectNode.ImageName.Application);
                }
            }

            return iconHandle;
        }

        /// <summary>
        ///     Return S_OK. Implementation of Closing a nested project is done in CloseNestedProject which is called by CloseChildren.
        /// </summary>
        /// <returns>S_OK</returns>
        public override int Close()
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Returns the moniker of the nested project.
        /// </summary>
        /// <returns></returns>
        public override string GetMkDocument()
        {
            Debug.Assert(nestedHierarchy != null,
                         "The nested hierarchy object must be created before calling this method");
            if (isDisposed || ProjectMgr == null || ProjectMgr.IsClosed)
            {
                return String.Empty;
            }

            return projectPath;
        }

        /// <summary>
        ///     Called by the shell when a node has been renamed from the GUI
        /// </summary>
        /// <param name="label">The name of the new label.</param>
        /// <returns>A success or failure value.</returns>
        public override int SetEditLabel(string label)
        {
            int result = DelegateSetPropertyToNested((int)__VSHPROPID.VSHPROPID_EditLabel, label);
            if (ErrorHandler.Succeeded(result))
            {
                RenameNestedProjectInParentProject(label);
            }

            return result;
        }

        /// <summary>
        ///     Called by the shell to get the node caption when the user tries to rename from the GUI
        /// </summary>
        /// <returns>the node cation</returns>
        public override string GetEditLabel()
        {
            return (string)DelegateGetPropertyToNested((int)__VSHPROPID.VSHPROPID_EditLabel);
        }

        /// <summary>
        ///     This is temporary until we have support for re-adding a nested item
        /// </summary>
        protected override bool CanDeleteItem(__VSDELETEITEMOPERATION deleteOperation)
        {
            return false;
        }

        /// <summary>
        ///     Delegates the call to the inner hierarchy.
        /// </summary>
        /// <param name="reserved">Reserved parameter defined at the IVsPersistHierarchyItem2::ReloadItem parameter.</param>
        protected internal override void ReloadItem(uint reserved)
        {
            #region precondition

            if (isDisposed || ProjectMgr == null || ProjectMgr.IsClosed)
            {
                throw new InvalidOperationException();
            }

            Debug.Assert(nestedHierarchy != null,
                         "The nested hierarchy object must be created before calling this method");

            #endregion

            IVsPersistHierarchyItem2 persistHierachyItem = nestedHierarchy as IVsPersistHierarchyItem2;

            // We are expecting that if we get called then the nestedhierarchy supports IVsPersistHierarchyItem2, since then hierrachy should support handling its own reload.
            // There should be no errormessage to the user since this is an internal error, that it cannot be fixed at user level.
            if (persistHierachyItem == null)
            {
                throw new InvalidOperationException();
            }

            ErrorHandler.ThrowOnFailure(persistHierachyItem.ReloadItem(VSConstants.VSITEMID_ROOT, reserved));
        }

        /// <summary>
        ///     Flag indicating that changes to a file can be ignored when item is saved or reloaded.
        /// </summary>
        /// <param name="ignoreFlag">Flag indicating whether or not to ignore changes (1 to ignore, 0 to stop ignoring).</param>
        protected internal override void IgnoreItemFileChanges(bool ignoreFlag)
        {
            #region precondition

            if (isDisposed || ProjectMgr == null || ProjectMgr.IsClosed)
            {
                throw new InvalidOperationException();
            }

            Debug.Assert(nestedHierarchy != null,
                         "The nested hierarchy object must be created before calling this method");

            #endregion

            IgnoreNestedProjectFile(ignoreFlag);

            IVsPersistHierarchyItem2 persistHierachyItem = nestedHierarchy as IVsPersistHierarchyItem2;

            // If the IVsPersistHierarchyItem2 is not implemented by the nested just return
            if (persistHierachyItem == null)
            {
                return;
            }

            ErrorHandler.ThrowOnFailure(persistHierachyItem.IgnoreItemFileChanges(VSConstants.VSITEMID_ROOT,
                                                                                  ignoreFlag ? 1 : 0));
        }

        /// <summary>
        ///     Sets the VSADDFILEFLAGS that will be used to call the  IVsTrackProjectDocumentsEvents2 OnAddFiles
        /// </summary>
        /// <param name="files">The files to which an array of VSADDFILEFLAGS has to be specified.</param>
        /// <returns></returns>
        protected internal override VSADDFILEFLAGS[] GetAddFileFlags(string[] files)
        {
            if (files == null || files.Length == 0)
            {
                return new VSADDFILEFLAGS[1] { VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags };
            }

            VSADDFILEFLAGS[] addFileFlags = new VSADDFILEFLAGS[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                addFileFlags[i] = VSADDFILEFLAGS.VSADDFILEFLAGS_IsNestedProjectFile;
            }

            return addFileFlags;
        }

        /// <summary>
        ///     Sets the VSQUERYADDFILEFLAGS that will be used to call the  IVsTrackProjectDocumentsEvents2 OnQueryAddFiles
        /// </summary>
        /// <param name="files">The files to which an array of VSADDFILEFLAGS has to be specified.</param>
        /// <returns></returns>
        protected internal override VSQUERYADDFILEFLAGS[] GetQueryAddFileFlags(string[] files)
        {
            if (files == null || files.Length == 0)
            {
                return new VSQUERYADDFILEFLAGS[1] { VSQUERYADDFILEFLAGS.VSQUERYADDFILEFLAGS_NoFlags };
            }

            VSQUERYADDFILEFLAGS[] queryAddFileFlags = new VSQUERYADDFILEFLAGS[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                queryAddFileFlags[i] = VSQUERYADDFILEFLAGS.VSQUERYADDFILEFLAGS_IsNestedProjectFile;
            }

            return queryAddFileFlags;
        }

        /// <summary>
        ///     Sets the VSREMOVEFILEFLAGS that will be used to call the  IVsTrackProjectDocumentsEvents2 OnRemoveFiles
        /// </summary>
        /// <param name="files">The files to which an array of VSREMOVEFILEFLAGS has to be specified.</param>
        /// <returns></returns>
        protected internal override VSREMOVEFILEFLAGS[] GetRemoveFileFlags(string[] files)
        {
            if (files == null || files.Length == 0)
            {
                return new VSREMOVEFILEFLAGS[1] { VSREMOVEFILEFLAGS.VSREMOVEFILEFLAGS_NoFlags };
            }

            VSREMOVEFILEFLAGS[] removeFileFlags = new VSREMOVEFILEFLAGS[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                removeFileFlags[i] = VSREMOVEFILEFLAGS.VSREMOVEFILEFLAGS_IsNestedProjectFile;
            }

            return removeFileFlags;
        }

        /// <summary>
        ///     Sets the VSQUERYREMOVEFILEFLAGS that will be used to call the  IVsTrackProjectDocumentsEvents2 OnQueryRemoveFiles
        /// </summary>
        /// <param name="files">The files to which an array of VSQUERYREMOVEFILEFLAGS has to be specified.</param>
        /// <returns></returns>
        protected internal override VSQUERYREMOVEFILEFLAGS[] GetQueryRemoveFileFlags(string[] files)
        {
            if (files == null || files.Length == 0)
            {
                return new VSQUERYREMOVEFILEFLAGS[1] { VSQUERYREMOVEFILEFLAGS.VSQUERYREMOVEFILEFLAGS_NoFlags };
            }

            VSQUERYREMOVEFILEFLAGS[] queryRemoveFileFlags = new VSQUERYREMOVEFILEFLAGS[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                queryRemoveFileFlags[i] = VSQUERYREMOVEFILEFLAGS.VSQUERYREMOVEFILEFLAGS_IsNestedProjectFile;
            }

            return queryRemoveFileFlags;
        }

        #endregion

        #region virtual methods

        /// <summary>
        ///     Initialize the nested hierarhy node.
        /// </summary>
        /// <param name="fileName">The file name of the nested project.</param>
        /// <param name="destination">The location of the nested project.</param>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="createFlags">The nested project creation flags </param>
        /// <remarks>This methos should be called just after a NestedProjectNode object is created.</remarks>
        public virtual void Init(string fileName, string destination, string projectName,
                                 __VSCREATEPROJFLAGS createFlags)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException(
                    SR.GetString(SR.ParameterCannotBeNullOrEmpty, CultureInfo.CurrentUICulture), "fileName");
            }

            if (String.IsNullOrEmpty(destination))
            {
                throw new ArgumentException(
                    SR.GetString(SR.ParameterCannotBeNullOrEmpty, CultureInfo.CurrentUICulture), "destination");
            }

            this.projectName = Path.GetFileName(fileName);
            projectPath = Path.Combine(destination, this.projectName);

            // get the IVsSolution interface from the global service provider
            IVsSolution solution = GetService(typeof(IVsSolution)) as IVsSolution;
            Debug.Assert(solution != null,
                         "Could not get the IVsSolution object from the services exposed by this project");
            if (solution == null)
            {
                throw new InvalidOperationException();
            }

            // Get the project type guid from project element				
            string typeGuidString = ItemNode.GetMetadataAndThrow(ProjectFileConstants.TypeGuid,
                                                                 new InvalidOperationException());
            Guid projectFactoryGuid = Guid.Empty;
            if (!String.IsNullOrEmpty(typeGuidString))
            {
                projectFactoryGuid = new Guid(typeGuidString);
            }

            // Get the project factory.
            IVsProjectFactory projectFactory;
            ErrorHandler.ThrowOnFailure(solution.GetProjectFactory(0, new[] { projectFactoryGuid }, fileName,
                                                                   out projectFactory));

            CreateProjectDirectory();

            //Create new project using factory
            int cancelled;
            Guid refiid = NativeMethods.IID_IUnknown;
            IntPtr projectPtr = IntPtr.Zero;

            try
            {
                ErrorHandler.ThrowOnFailure(projectFactory.CreateProject(fileName, destination, projectName,
                                                                         (uint)createFlags, ref refiid, out projectPtr,
                                                                         out cancelled));

                if (projectPtr != IntPtr.Zero)
                {
                    nestedHierarchy =
                        Marshal.GetTypedObjectForIUnknown(projectPtr, typeof(IVsHierarchy)) as IVsHierarchy;
                    Debug.Assert(nestedHierarchy != null, "Nested hierarchy could not be created");
                    Debug.Assert(cancelled == 0);
                }
            }
            finally
            {
                if (projectPtr != IntPtr.Zero)
                {
                    // We created a new instance of the project, we need to call release to decrement the ref count
                    // the RCW (this.nestedHierarchy) still has a reference to it which will keep it alive
                    Marshal.Release(projectPtr);
                }
            }

            if (cancelled != 0 && nestedHierarchy == null)
            {
                ErrorHandler.ThrowOnFailure(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            // Link into the nested VS hierarchy.
            ErrorHandler.ThrowOnFailure(nestedHierarchy.SetProperty(VSConstants.VSITEMID_ROOT,
                                                                    (int)__VSHPROPID.VSHPROPID_ParentHierarchy,
                                                                    ProjectMgr));
            ErrorHandler.ThrowOnFailure(nestedHierarchy.SetProperty(VSConstants.VSITEMID_ROOT,
                                                                    (int)__VSHPROPID.VSHPROPID_ParentHierarchyItemid,
                                                                    (int)ID));

            LockRDTEntry();

            ConnectPropertyNotifySink();
        }

        /// <summary>
        ///     Links a nested project as a virtual project to the solution.
        /// </summary>
        protected internal virtual void AddVirtualProject()
        {
            // This is the second step in creating and adding a nested project. The inner hierarchy must have been
            // already initialized at this point. 

            #region precondition

            if (nestedHierarchy == null)
            {
                throw new InvalidOperationException();
            }

            #endregion

            // get the IVsSolution interface from the global service provider
            IVsSolution solution = GetService(typeof(IVsSolution)) as IVsSolution;
            Debug.Assert(solution != null,
                         "Could not get the IVsSolution object from the services exposed by this project");
            if (solution == null)
            {
                throw new InvalidOperationException();
            }

            InitializeInstanceGuid();

            // Add virtual project to solution.
            ErrorHandler.ThrowOnFailure(solution.AddVirtualProjectEx(nestedHierarchy, VirtualProjectFlags,
                                                                     ref projectInstanceGuid));

            // Now set up to listen on file changes on the nested project node.
            ObserveNestedProjectFile();
        }

        /// <summary>
        ///     The method that does the cleanup.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            // Everybody can go here.
            if (!isDisposed)
            {
                try
                {
                    // Synchronize calls to the Dispose simulteniously.
                    lock (Mutex)
                    {
                        if (disposing)
                        {
                            DisconnectPropertyNotifySink();
                            StopObservingNestedProjectFile();

                            // If a project cannot load it may happen that the imagehandler is not instantiated.
                            if (imageHandler != null)
                            {
                                imageHandler.Close();
                            }
                        }
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                    isDisposed = true;
                }
            }
        }

        /// <summary>
        ///     Creates the project directory if it does not exist.
        /// </summary>
        /// <returns></returns>
        protected virtual void CreateProjectDirectory()
        {
            string directoryName = Path.GetDirectoryName(projectPath);

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        /// <summary>
        ///     Lock the RDT Entry for the nested project.
        ///     By default this document is marked as "Dont Save as". That means the menu File->SaveAs is disabled for the
        ///     nested project node.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RDT")]
        protected virtual void LockRDTEntry()
        {
            // Define flags for the nested project document
            _VSRDTFLAGS flags = _VSRDTFLAGS.RDT_VirtualDocument | _VSRDTFLAGS.RDT_ProjSlnDocument;
            ;

            // Request the RDT service
            IVsRunningDocumentTable rdt = GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            Debug.Assert(rdt != null, " Could not get running document table from the services exposed by this project");
            if (rdt == null)
            {
                throw new InvalidOperationException();
            }

            // First we see if someone else has opened the requested view of the file.
            uint itemid;
            IntPtr docData = IntPtr.Zero;
            IVsHierarchy ivsHierarchy;
            uint docCookie;
            IntPtr projectPtr = IntPtr.Zero;

            try
            {
                ErrorHandler.ThrowOnFailure(rdt.FindAndLockDocument((uint)flags, projectPath, out ivsHierarchy,
                                                                    out itemid, out docData, out docCookie));
                flags |= _VSRDTFLAGS.RDT_EditLock;

                if (ivsHierarchy != null && docCookie != (uint)ShellConstants.VSDOCCOOKIE_NIL)
                {
                    if (docCookie != DocCookie)
                    {
                        DocCookie = docCookie;
                    }
                }
                else
                {
                    // get inptr for hierarchy
                    projectPtr = Marshal.GetIUnknownForObject(nestedHierarchy);
                    Debug.Assert(projectPtr != IntPtr.Zero,
                                 " Project pointer for the nested hierarchy has not been initialized");
                    ErrorHandler.ThrowOnFailure(rdt.RegisterAndLockDocument((uint)flags, projectPath,
                                                                            ProjectMgr.InteropSafeIVsHierarchy, ID,
                                                                            projectPtr, out docCookie));

                    DocCookie = docCookie;
                    Debug.Assert(DocCookie != (uint)ShellConstants.VSDOCCOOKIE_NIL,
                                 "Invalid cookie when registering document in the running document table.");

                    //we must also set the doc cookie on the nested hier
                    SetDocCookieOnNestedHier(DocCookie);
                }
            }
            finally
            {
                // Release all Inptr's that that were given as out pointers
                if (docData != IntPtr.Zero)
                {
                    Marshal.Release(docData);
                }
                if (projectPtr != IntPtr.Zero)
                {
                    Marshal.Release(projectPtr);
                }
            }
        }

        /// <summary>
        ///     Unlock the RDT entry for the nested project
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RDT")]
        protected virtual void UnlockRDTEntry()
        {
            if (isDisposed || ProjectMgr == null || ProjectMgr.IsClosed)
            {
                return;
            }
            // First we see if someone else has opened the requested view of the file.
            IVsRunningDocumentTable rdt = GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            if (rdt != null && DocCookie != (int)ShellConstants.VSDOCCOOKIE_NIL)
            {
                _VSRDTFLAGS flags = _VSRDTFLAGS.RDT_EditLock;

                ErrorHandler.ThrowOnFailure(rdt.UnlockDocument((uint)flags, DocCookie));
            }

            DocCookie = (int)ShellConstants.VSDOCCOOKIE_NIL;
        }

        /// <summary>
        ///     Renames the project file in the parent project structure.
        /// </summary>
        /// <param name="label">The new label.</param>
        protected virtual void RenameNestedProjectInParentProject(string label)
        {
            string existingLabel = Caption;

            if (String.Compare(existingLabel, label, StringComparison.Ordinal) == 0)
            {
                return;
            }

            string oldFileName = projectPath;
            string oldPath = Url;

            try
            {
                StopObservingNestedProjectFile();
                ProjectMgr.SuspendMSBuild();

                // Check out the project file if necessary.
                if (!ProjectMgr.QueryEditProjectFile(false))
                {
                    throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
                }

                string newFileName = label + Path.GetExtension(oldFileName);
                SaveNestedProjectItemInProjectFile(newFileName);

                string projectDirectory = Path.GetDirectoryName(oldFileName);

                // update state.
                projectName = newFileName;
                projectPath = Path.Combine(projectDirectory, projectName);

                // Unload and lock the RDT entries
                UnlockRDTEntry();
                LockRDTEntry();

                // Since actually this is a rename in our hierarchy notify the tracker that a rename has happened.
                ProjectMgr.Tracker.OnItemRenamed(oldPath, projectPath,
                                                 VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_IsNestedProjectFile);
            }
            finally
            {
                ObserveNestedProjectFile();
                ProjectMgr.ResumeMSBuild(ProjectMgr.ReEvaluateProjectFileTargetName);
            }
        }

        /// <summary>
        ///     Saves the nested project information in the project file.
        /// </summary>
        /// <param name="newFileName"></param>
        protected virtual void SaveNestedProjectItemInProjectFile(string newFileName)
        {
            string existingInclude = ItemNode.Item.EvaluatedInclude;
            string existingRelativePath = Path.GetDirectoryName(existingInclude);
            string newRelativePath = Path.Combine(existingRelativePath, newFileName);
            ItemNode.Rename(newRelativePath);
        }

        #endregion

        #region helper methods

        /// <summary>
        ///     Closes a nested project and releases the nested hierrachy pointer.
        /// </summary>
        internal void CloseNestedProjectNode()
        {
            if (isDisposed || ProjectMgr == null || ProjectMgr.IsClosed)
            {
                return;
            }

            uint itemid = VSConstants.VSITEMID_NIL;
            try
            {
                DisconnectPropertyNotifySink();

                IVsUIHierarchy hier;

                IVsWindowFrame windowFrame;
                VsShellUtilities.IsDocumentOpen(ProjectMgr.Site, projectPath, Guid.Empty, out hier, out itemid,
                                                out windowFrame);

                if (itemid == VSConstants.VSITEMID_NIL)
                {
                    UnlockRDTEntry();
                }

                IVsSolution solution = GetService(typeof(IVsSolution)) as IVsSolution;
                if (solution == null)
                {
                    throw new InvalidOperationException();
                }

                ErrorHandler.ThrowOnFailure(solution.RemoveVirtualProject(nestedHierarchy, 0));
            }
            finally
            {
                StopObservingNestedProjectFile();

                // if we haven't already release the RDT cookie, do so now.
                if (itemid == VSConstants.VSITEMID_NIL)
                {
                    UnlockRDTEntry();
                }

                Dispose(true);
            }
        }

        private void InitializeInstanceGuid()
        {
            if (projectInstanceGuid != Guid.Empty)
            {
                return;
            }

            Guid instanceGuid = Guid.Empty;

            Debug.Assert(nestedHierarchy != null,
                         "The nested hierarchy object must be created before calling this method");

            // This method should be called from the open children method, then we can safely use the IsNewProject property
            if (ProjectMgr.IsNewProject)
            {
                instanceGuid = Guid.NewGuid();
                ItemNode.SetMetadata(ProjectFileConstants.InstanceGuid, instanceGuid.ToString("B"));
                ErrorHandler.ThrowOnFailure(nestedHierarchy.SetGuidProperty(VSConstants.VSITEMID_ROOT,
                                                                            (int)__VSHPROPID.VSHPROPID_ProjectIDGuid,
                                                                            ref instanceGuid));
            }
            else
            {
                // Get a guid from the nested hiererachy.
                Guid nestedHiererachyInstanceGuid;
                ErrorHandler.ThrowOnFailure(nestedHierarchy.GetGuidProperty(VSConstants.VSITEMID_ROOT,
                                                                            (int)__VSHPROPID.VSHPROPID_ProjectIDGuid,
                                                                            out nestedHiererachyInstanceGuid));

                // Get instance guid from the project file. If it does not exist then we create one.
                string instanceGuidAsString = ItemNode.GetMetadata(ProjectFileConstants.InstanceGuid);

                // 1. nestedHiererachyInstanceGuid is empty and instanceGuidAsString is empty then create a new one.
                // 2. nestedHiererachyInstanceGuid is empty and instanceGuidAsString not empty use instanceGuidAsString and update the nested project object by calling SetGuidProperty.
                // 3. nestedHiererachyInstanceGuid is not empty instanceGuidAsString is empty then use nestedHiererachyInstanceGuid and update the outer project element.
                // 4. nestedHiererachyInstanceGuid is not empty instanceGuidAsString is empty then use nestedHiererachyInstanceGuid and update the outer project element.

                if (nestedHiererachyInstanceGuid == Guid.Empty && String.IsNullOrEmpty(instanceGuidAsString))
                {
                    instanceGuid = Guid.NewGuid();
                }
                else if (nestedHiererachyInstanceGuid == Guid.Empty && !String.IsNullOrEmpty(instanceGuidAsString))
                {
                    instanceGuid = new Guid(instanceGuidAsString);

                    ErrorHandler.ThrowOnFailure(nestedHierarchy.SetGuidProperty(VSConstants.VSITEMID_ROOT,
                                                                                (int)__VSHPROPID.VSHPROPID_ProjectIDGuid,
                                                                                ref instanceGuid));
                }
                else if (nestedHiererachyInstanceGuid != Guid.Empty)
                {
                    instanceGuid = nestedHiererachyInstanceGuid;

                    // If the instanceGuidAsString is empty then creating a guid out of it would throw an exception.
                    if (String.IsNullOrEmpty(instanceGuidAsString) ||
                        nestedHiererachyInstanceGuid != new Guid(instanceGuidAsString))
                    {
                        ItemNode.SetMetadata(ProjectFileConstants.InstanceGuid, instanceGuid.ToString("B"));
                    }
                }
            }

            projectInstanceGuid = instanceGuid;
        }

        private void SetDocCookieOnNestedHier(uint itemDocCookie)
        {
            object docCookie = (int)itemDocCookie;

            try
            {
                ErrorHandler.ThrowOnFailure(nestedHierarchy.SetProperty(VSConstants.VSITEMID_ROOT,
                                                                        (int)__VSHPROPID.VSHPROPID_ItemDocCookie,
                                                                        docCookie));
            }
            catch (NotImplementedException)
            {
                //we swallow this exception on purpose
            }
        }

        private void InitImageHandler()
        {
            Debug.Assert(nestedHierarchy != null,
                         "The nested hierarchy object must be created before calling this method");

            if (null == imageHandler)
            {
                imageHandler = new ImageHandler();
            }
            object imageListAsPointer = null;
            ErrorHandler.ThrowOnFailure(nestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT,
                                                                    (int)__VSHPROPID.VSHPROPID_IconImgList,
                                                                    out imageListAsPointer));
            if (imageListAsPointer != null)
            {
                imageHandler.ImageList = Utilities.GetImageList(imageListAsPointer);
            }
        }

        /// <summary>
        ///     Delegates Getproperty calls to the inner nested.
        /// </summary>
        /// <param name="propID">The property to delegate.</param>
        /// <returns>The return of the GetProperty from nested.</returns>
        private object DelegateGetPropertyToNested(int propID)
        {
            if (!ProjectMgr.IsClosed)
            {
                Debug.Assert(nestedHierarchy != null,
                             "The nested hierarchy object must be created before calling this method");

                object returnValue;

                // Do not throw since some project types will return E_FAIL if they do not support a property.
                int result = nestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, propID, out returnValue);
                if (ErrorHandler.Succeeded(result))
                {
                    return returnValue;
                }
            }

            return null;
        }

        /// <summary>
        ///     Delegates Setproperty calls to the inner nested.
        /// </summary>
        /// <param name="propID">The property to delegate.</param>
        /// <param name="value">The property to set.</param>
        /// <returns>The return of the SetProperty from nested.</returns>
        private int DelegateSetPropertyToNested(int propID, object value)
        {
            if (ProjectMgr.IsClosed)
            {
                return VSConstants.E_FAIL;
            }

            Debug.Assert(nestedHierarchy != null,
                         "The nested hierarchy object must be created before calling this method");

            // Do not throw since some project types will return E_FAIL if they do not support a property.
            return nestedHierarchy.SetProperty(VSConstants.VSITEMID_ROOT, propID, value);
        }

        /// <summary>
        ///     Starts observing changes on this file.
        /// </summary>
        private void ObserveNestedProjectFile()
        {
            ProjectContainerNode parent = ProjectMgr as ProjectContainerNode;
            Debug.Assert(parent != null,
                         "The parent project for nested projects should be subclassed from ProjectContainerNode");
            parent.NestedProjectNodeReloader.ObserveItem(GetMkDocument(), ID);
        }

        /// <summary>
        ///     Stops observing changes on this file.
        /// </summary>
        private void StopObservingNestedProjectFile()
        {
            ProjectContainerNode parent = ProjectMgr as ProjectContainerNode;
            Debug.Assert(parent != null,
                         "The parent project for nested projects should be subclassed from ProjectContainerNode");
            parent.NestedProjectNodeReloader.StopObservingItem(GetMkDocument());
        }

        /// <summary>
        ///     Ignores observing changes on this file depending on the boolean flag.
        /// </summary>
        /// <param name="ignoreFlag">Flag indicating whether or not to ignore changes (1 to ignore, 0 to stop ignoring).</param>
        private void IgnoreNestedProjectFile(bool ignoreFlag)
        {
            ProjectContainerNode parent = ProjectMgr as ProjectContainerNode;
            Debug.Assert(parent != null,
                         "The parent project for nested projects should be subclassed from ProjectContainerNode");
            parent.NestedProjectNodeReloader.IgnoreItemChanges(GetMkDocument(), ignoreFlag);
        }

        /// <summary>
        ///     We need to advise property notify sink on project properties so that
        ///     we know when the project file is renamed through a property.
        /// </summary>
        private void ConnectPropertyNotifySink()
        {
            if (projectPropertyNotifySinkCookie != (uint)ShellConstants.VSCOOKIE_NIL)
            {
                return;
            }

            IConnectionPoint connectionPoint = GetConnectionPointFromPropertySink();
            if (connectionPoint != null)
            {
                connectionPoint.Advise(this, out projectPropertyNotifySinkCookie);
            }
        }

        /// <summary>
        ///     Disconnects the propertynotify sink
        /// </summary>
        private void DisconnectPropertyNotifySink()
        {
            if (projectPropertyNotifySinkCookie == (uint)ShellConstants.VSCOOKIE_NIL)
            {
                return;
            }

            IConnectionPoint connectionPoint = GetConnectionPointFromPropertySink();
            if (connectionPoint != null)
            {
                connectionPoint.Unadvise(projectPropertyNotifySinkCookie);
                projectPropertyNotifySinkCookie = (uint)ShellConstants.VSCOOKIE_NIL;
            }
        }

        /// <summary>
        ///     Gets a ConnectionPoint for the IPropertyNotifySink interface.
        /// </summary>
        /// <returns></returns>
        private IConnectionPoint GetConnectionPointFromPropertySink()
        {
            IConnectionPoint connectionPoint = null;
            object browseObject = GetProperty((int)__VSHPROPID.VSHPROPID_BrowseObject);
            IConnectionPointContainer connectionPointContainer = browseObject as IConnectionPointContainer;

            if (connectionPointContainer != null)
            {
                Guid guid = typeof(IPropertyNotifySink).GUID;
                connectionPointContainer.FindConnectionPoint(ref guid, out connectionPoint);
            }

            return connectionPoint;
        }

        #endregion
    }
}
