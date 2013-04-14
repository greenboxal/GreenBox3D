// NodeProperties.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.Project
{
    /// <summary>
    ///     All public properties on Nodeproperties or derived classes are assumed to be used by Automation by default.
    ///     Set this attribute to false on Properties that should not be visible for Automation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class AutomationBrowsableAttribute : Attribute
    {
        private readonly bool browsable;

        public AutomationBrowsableAttribute(bool browsable)
        {
            this.browsable = browsable;
        }

        public bool Browsable
        {
            get { return browsable; }
        }
    }

    /// <summary>
    ///     To create your own localizable node properties, subclass this and add public properties
    ///     decorated with your own localized display name, category and description attributes.
    /// </summary>
    [CLSCompliant(false), ComVisible(true)]
    public class NodeProperties : LocalizableProperties,
                                  ISpecifyPropertyPages,
                                  IVsGetCfgProvider,
                                  IVsSpecifyProjectDesignerPages,
                                  IInternalExtenderProvider,
                                  IVsBrowseObject
    {
        #region fields

        private readonly HierarchyNode node;

        #endregion

        #region properties

        [Browsable(false)]
        [AutomationBrowsable(false)]
        public HierarchyNode Node
        {
            get { return node; }
        }

        /// <summary>
        ///     Used by Property Pages Frame to set it's title bar. The Caption of the Hierarchy Node is returned.
        /// </summary>
        [Browsable(false)]
        [AutomationBrowsable(false)]
        public virtual string Name
        {
            get { return node.Caption; }
        }

        #endregion

        #region ctors

        public NodeProperties(HierarchyNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            this.node = node;
        }

        #endregion

        #region ISpecifyPropertyPages methods

        public virtual void GetPages(CAUUID[] pages)
        {
            GetCommonPropertyPages(pages);
        }

        #endregion

        #region IVsSpecifyProjectDesignerPages

        /// <summary>
        ///     Implementation of the IVsSpecifyProjectDesignerPages. It will retun the pages that are configuration independent.
        /// </summary>
        /// <param name="pages">The pages to return.</param>
        /// <returns></returns>
        public virtual int GetProjectDesignerPages(CAUUID[] pages)
        {
            GetCommonPropertyPages(pages);
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsGetCfgProvider methods

        public virtual int GetCfgProvider(out IVsCfgProvider p)
        {
            p = null;
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsBrowseObject methods

        /// <summary>
        ///     Maps back to the hierarchy or project item object corresponding to the browse object.
        /// </summary>
        /// <param name="hier">Reference to the hierarchy object.</param>
        /// <param name="itemid">Reference to the project item.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int GetProjectItem(out IVsHierarchy hier, out uint itemid)
        {
            if (node == null)
            {
                throw new InvalidOperationException();
            }
            hier = node.ProjectMgr.InteropSafeIVsHierarchy;
            itemid = node.ID;
            return VSConstants.S_OK;
        }

        #endregion

        #region overridden methods

        /// <summary>
        ///     Get the Caption of the Hierarchy Node instance. If Caption is null or empty we delegate to base
        /// </summary>
        /// <returns>Caption of Hierarchy node instance</returns>
        public override string GetComponentName()
        {
            string caption = Node.Caption;
            if (string.IsNullOrEmpty(caption))
            {
                return base.GetComponentName();
            }
            else
            {
                return caption;
            }
        }

        #endregion

        #region helper methods

        protected string GetProperty(string name, string def)
        {
            string a = Node.ItemNode.GetMetadata(name);
            return (a == null) ? def : a;
        }

        protected void SetProperty(string name, string value)
        {
            Node.ItemNode.SetMetadata(name, value);
        }

        /// <summary>
        ///     Retrieves the common property pages. The NodeProperties is the BrowseObject and that will be called to support
        ///     configuration independent properties.
        /// </summary>
        /// <param name="pages">The pages to return.</param>
        private void GetCommonPropertyPages(CAUUID[] pages)
        {
            // We do not check whether the supportsProjectDesigner is set to false on the ProjectNode.
            // We rely that the caller knows what to call on us.
            if (pages == null)
            {
                throw new ArgumentNullException("pages");
            }

            if (pages.Length == 0)
            {
                throw new ArgumentException(SR.GetString(SR.InvalidParameter, CultureInfo.CurrentUICulture), "pages");
            }

            // Only the project should show the property page the rest should show the project properties.
            if (node != null && (node is ProjectNode))
            {
                // Retrieve the list of guids from hierarchy properties.
                // Because a flavor could modify that list we must make sure we are calling the outer most implementation of IVsHierarchy
                string guidsList = String.Empty;
                IVsHierarchy hierarchy = Node.ProjectMgr.InteropSafeIVsHierarchy;
                object variant = null;
                ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT,
                                                                  (int)__VSHPROPID2.VSHPROPID_PropertyPagesCLSIDList,
                                                                  out variant));
                guidsList = (string)variant;

                Guid[] guids = Utilities.GuidsArrayFromSemicolonDelimitedStringOfGuids(guidsList);
                if (guids == null || guids.Length == 0)
                {
                    pages[0] = new CAUUID();
                    pages[0].cElems = 0;
                }
                else
                {
                    pages[0] = PackageUtilities.CreateCAUUIDFromGuidArray(guids);
                }
            }
            else
            {
                pages[0] = new CAUUID();
                pages[0].cElems = 0;
            }
        }

        #endregion

        #region IInternalExtenderProvider Members

        bool IInternalExtenderProvider.CanExtend(string extenderCATID, string extenderName, object extendeeObject)
        {
            IInternalExtenderProvider outerHierarchy =
                Node.ProjectMgr.InteropSafeIVsHierarchy as IInternalExtenderProvider;

            if (outerHierarchy != null)
            {
                return outerHierarchy.CanExtend(extenderCATID, extenderName, extendeeObject);
            }
            return false;
        }

        object IInternalExtenderProvider.GetExtender(string extenderCATID, string extenderName, object extendeeObject,
                                                     IExtenderSite extenderSite, int cookie)
        {
            IInternalExtenderProvider outerHierarchy =
                Node.ProjectMgr.InteropSafeIVsHierarchy as IInternalExtenderProvider;

            if (outerHierarchy != null)
            {
                return outerHierarchy.GetExtender(extenderCATID, extenderName, extendeeObject, extenderSite, cookie);
            }

            return null;
        }

        object IInternalExtenderProvider.GetExtenderNames(string extenderCATID, object extendeeObject)
        {
            IInternalExtenderProvider outerHierarchy =
                Node.ProjectMgr.InteropSafeIVsHierarchy as IInternalExtenderProvider;

            if (outerHierarchy != null)
            {
                return outerHierarchy.GetExtenderNames(extenderCATID, extendeeObject);
            }

            return null;
        }

        #endregion

        #region ExtenderSupport

        [Browsable(false)]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CATID")]
        public virtual string ExtenderCATID
        {
            get
            {
                Guid catid = Node.ProjectMgr.GetCATIDForType(GetType());
                if (Guid.Empty.CompareTo(catid) == 0)
                {
                    return null;
                }
                return catid.ToString("B");
            }
        }

        [Browsable(false)]
        public object ExtenderNames()
        {
            ObjectExtenders extenderService = (ObjectExtenders)Node.GetService(typeof(ObjectExtenders));
            Debug.Assert(extenderService != null,
                         "Could not get the ObjectExtenders object from the services exposed by this property object");
            if (extenderService == null)
            {
                throw new InvalidOperationException();
            }
            return extenderService.GetExtenderNames(ExtenderCATID, this);
        }

        public object Extender(string extenderName)
        {
            ObjectExtenders extenderService = (ObjectExtenders)Node.GetService(typeof(ObjectExtenders));
            Debug.Assert(extenderService != null,
                         "Could not get the ObjectExtenders object from the services exposed by this property object");
            if (extenderService == null)
            {
                throw new InvalidOperationException();
            }
            return extenderService.GetExtender(ExtenderCATID, extenderName, this);
        }

        #endregion
    }

    [CLSCompliant(false), ComVisible(true)]
    public class FileNodeProperties : NodeProperties
    {
        #region properties

        [SRCategory(SR.Advanced)]
        [LocDisplayName(SR.BuildAction)]
        [SRDescription(SR.BuildActionDescription)]
        public virtual BuildAction BuildAction
        {
            get
            {
                string value = Node.ItemNode.ItemName;
                if (value == null || value.Length == 0)
                {
                    return BuildAction.None;
                }
                return (BuildAction)Enum.Parse(typeof(BuildAction), value);
            }
            set { Node.ItemNode.ItemName = value.ToString(); }
        }

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.FileName)]
        [SRDescription(SR.FileNameDescription)]
        public string FileName
        {
            get { return Node.Caption; }
            set { Node.SetEditLabel(value); }
        }

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.FullPath)]
        [SRDescription(SR.FullPathDescription)]
        public string FullPath
        {
            get { return Node.Url; }
        }

        #region non-browsable properties - used for automation only

        [Browsable(false)]
        public string Extension
        {
            get { return Path.GetExtension(Node.Caption); }
        }

        #endregion

        #endregion

        #region ctors

        public FileNodeProperties(HierarchyNode node)
            : base(node)
        {
        }

        #endregion

        #region overridden methods

        public override string GetClassName()
        {
            return SR.GetString(SR.FileProperties, CultureInfo.CurrentUICulture);
        }

        #endregion
    }

    [CLSCompliant(false), ComVisible(true)]
    public class DependentFileNodeProperties : NodeProperties
    {
        #region properties

        [SRCategory(SR.Advanced)]
        [LocDisplayName(SR.BuildAction)]
        [SRDescription(SR.BuildActionDescription)]
        public virtual BuildAction BuildAction
        {
            get
            {
                string value = Node.ItemNode.ItemName;
                if (value == null || value.Length == 0)
                {
                    return BuildAction.None;
                }
                return (BuildAction)Enum.Parse(typeof(BuildAction), value);
            }
            set { Node.ItemNode.ItemName = value.ToString(); }
        }

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.FileName)]
        [SRDescription(SR.FileNameDescription)]
        public virtual string FileName
        {
            get { return Node.Caption; }
        }

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.FullPath)]
        [SRDescription(SR.FullPathDescription)]
        public string FullPath
        {
            get { return Node.Url; }
        }

        #endregion

        #region ctors

        public DependentFileNodeProperties(HierarchyNode node)
            : base(node)
        {
        }

        #endregion

        #region overridden methods

        public override string GetClassName()
        {
            return SR.GetString(SR.FileProperties, CultureInfo.CurrentUICulture);
        }

        #endregion
    }

    [CLSCompliant(false), ComVisible(true)]
    public class SingleFileGeneratorNodeProperties : FileNodeProperties
    {
        #region fields

        private EventHandler<HierarchyNodeEventArgs> onCustomToolChanged;
        private EventHandler<HierarchyNodeEventArgs> onCustomToolNameSpaceChanged;

        #endregion

        #region custom tool events

        internal event EventHandler<HierarchyNodeEventArgs> OnCustomToolChanged
        {
            add { onCustomToolChanged += value; }
            remove { onCustomToolChanged -= value; }
        }

        internal event EventHandler<HierarchyNodeEventArgs> OnCustomToolNameSpaceChanged
        {
            add { onCustomToolNameSpaceChanged += value; }
            remove { onCustomToolNameSpaceChanged -= value; }
        }

        #endregion

        #region properties

        [SRCategory(SR.Advanced)]
        [LocDisplayName(SR.CustomTool)]
        [SRDescription(SR.CustomToolDescription)]
        public virtual string CustomTool
        {
            get { return Node.ItemNode.GetMetadata(ProjectFileConstants.Generator); }
            set
            {
                if (CustomTool != value)
                {
                    Node.ItemNode.SetMetadata(ProjectFileConstants.Generator, value != string.Empty ? value : null);
                    HierarchyNodeEventArgs args = new HierarchyNodeEventArgs(Node);
                    if (onCustomToolChanged != null)
                    {
                        onCustomToolChanged(Node, args);
                    }
                }
            }
        }

        [SRCategory(SR.Advanced)]
        [LocDisplayName(SR.CustomToolNamespace)]
        [SRDescription(SR.CustomToolNamespaceDescription)]
        public virtual string CustomToolNamespace
        {
            get { return Node.ItemNode.GetMetadata(ProjectFileConstants.CustomToolNamespace); }
            set
            {
                if (CustomToolNamespace != value)
                {
                    Node.ItemNode.SetMetadata(ProjectFileConstants.CustomToolNamespace,
                                              value != String.Empty ? value : null);
                    HierarchyNodeEventArgs args = new HierarchyNodeEventArgs(Node);
                    if (onCustomToolNameSpaceChanged != null)
                    {
                        onCustomToolNameSpaceChanged(Node, args);
                    }
                }
            }
        }

        #endregion

        #region ctors

        public SingleFileGeneratorNodeProperties(HierarchyNode node)
            : base(node)
        {
        }

        #endregion
    }

    [CLSCompliant(false), ComVisible(true)]
    public class ProjectNodeProperties : NodeProperties
    {
        #region properties

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.ProjectFolder)]
        [SRDescription(SR.ProjectFolderDescription)]
        [AutomationBrowsable(false)]
        public string ProjectFolder
        {
            get { return Node.ProjectMgr.ProjectFolder; }
        }

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.ProjectFile)]
        [SRDescription(SR.ProjectFileDescription)]
        [AutomationBrowsable(false)]
        public string ProjectFile
        {
            get { return Node.ProjectMgr.ProjectFile; }
            set { Node.ProjectMgr.ProjectFile = value; }
        }

        #region non-browsable properties - used for automation only

        [Browsable(false)]
        public string FileName
        {
            get { return Node.ProjectMgr.ProjectFile; }
            set { Node.ProjectMgr.ProjectFile = value; }
        }

        [Browsable(false)]
        public string FullPath
        {
            get
            {
                string fullPath = Node.ProjectMgr.ProjectFolder;
                if (!fullPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                {
                    return fullPath + Path.DirectorySeparatorChar;
                }
                else
                {
                    return fullPath;
                }
            }
        }

        #endregion

        #endregion

        #region ctors

        public ProjectNodeProperties(ProjectNode node)
            : base(node)
        {
        }

        #endregion

        #region overridden methods

        public override string GetClassName()
        {
            return SR.GetString(SR.ProjectProperties, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        ///     ICustomTypeDescriptor.GetEditor
        ///     To enable the "Property Pages" button on the properties browser
        ///     the browse object (project properties) need to be unmanaged
        ///     or it needs to provide an editor of type ComponentEditor.
        /// </summary>
        /// <param name="editorBaseType">Type of the editor</param>
        /// <returns>Editor</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The service provider is used by the PropertiesEditorLauncher")]
        public override object GetEditor(Type editorBaseType)
        {
            // Override the scenario where we are asked for a ComponentEditor
            // as this is how the Properties Browser calls us
            if (editorBaseType == typeof(ComponentEditor))
            {
                IOleServiceProvider sp;
                ErrorHandler.ThrowOnFailure(Node.GetSite(out sp));
                return new PropertiesEditorLauncher(new ServiceProvider(sp));
            }

            return base.GetEditor(editorBaseType);
        }

        public override int GetCfgProvider(out IVsCfgProvider p)
        {
            if (Node != null && Node.ProjectMgr != null)
            {
                return Node.ProjectMgr.GetCfgProvider(out p);
            }

            return base.GetCfgProvider(out p);
        }

        #endregion
    }

    [CLSCompliant(false), ComVisible(true)]
    public class FolderNodeProperties : NodeProperties
    {
        #region properties

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.FolderName)]
        [SRDescription(SR.FolderNameDescription)]
        [AutomationBrowsable(false)]
        public string FolderName
        {
            get { return Node.Caption; }
            set
            {
                Node.SetEditLabel(value);
                Node.ReDraw(UIHierarchyElement.Caption);
            }
        }

        #region properties - used for automation only

        [Browsable(false)]
        [AutomationBrowsable(true)]
        public string FileName
        {
            get { return Node.Caption; }
            set { Node.SetEditLabel(value); }
        }

        [Browsable(false)]
        [AutomationBrowsable(true)]
        public string FullPath
        {
            get
            {
                string fullPath = Node.GetMkDocument();
                if (!fullPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                {
                    return fullPath + Path.DirectorySeparatorChar;
                }
                else
                {
                    return fullPath;
                }
            }
        }

        #endregion

        #endregion

        #region ctors

        public FolderNodeProperties(HierarchyNode node)
            : base(node)
        {
        }

        #endregion

        #region overridden methods

        public override string GetClassName()
        {
            return SR.GetString(SR.FolderProperties, CultureInfo.CurrentUICulture);
        }

        #endregion
    }

    [CLSCompliant(false), ComVisible(true)]
    public class ReferenceNodeProperties : NodeProperties
    {
        #region properties

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.RefName)]
        [SRDescription(SR.RefNameDescription)]
        [Browsable(true)]
        [AutomationBrowsable(true)]
        public override string Name
        {
            get { return Node.Caption; }
        }

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.CopyToLocal)]
        [SRDescription(SR.CopyToLocalDescription)]
        public bool CopyToLocal
        {
            get
            {
                string copyLocal = GetProperty(ProjectFileConstants.Private, "False");
                if (copyLocal == null || copyLocal.Length == 0)
                    return true;
                return bool.Parse(copyLocal);
            }
            set { SetProperty(ProjectFileConstants.Private, value.ToString()); }
        }

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.FullPath)]
        [SRDescription(SR.FullPathDescription)]
        public virtual string FullPath
        {
            get { return Node.Url; }
        }

        #endregion

        #region ctors

        public ReferenceNodeProperties(HierarchyNode node)
            : base(node)
        {
        }

        #endregion

        #region overridden methods

        public override string GetClassName()
        {
            return SR.GetString(SR.ReferenceProperties, CultureInfo.CurrentUICulture);
        }

        #endregion
    }

    [ComVisible(true)]
    public class ProjectReferencesProperties : ReferenceNodeProperties
    {
        #region ctors

        public ProjectReferencesProperties(ProjectReferenceNode node)
            : base(node)
        {
        }

        #endregion

        #region overriden methods

        public override string FullPath
        {
            get { return ((ProjectReferenceNode)Node).ReferencedProjectOutputPath; }
        }

        #endregion
    }

    [ComVisible(true)]
    public class ComReferenceProperties : ReferenceNodeProperties
    {
        public ComReferenceProperties(ComReferenceNode node)
            : base(node)
        {
        }

        [SRCategory(SR.Misc)]
        [LocDisplayName(SR.EmbedInteropTypes)]
        [SRDescription(SR.EmbedInteropTypesDescription)]
        public virtual bool EmbedInteropTypes
        {
            get { return ((ComReferenceNode)Node).EmbedInteropTypes; }
            set { ((ComReferenceNode)Node).EmbedInteropTypes = value; }
        }
    }
}
