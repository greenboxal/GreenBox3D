// OAReferences.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;

namespace Microsoft.VisualStudio.Project.Automation
{
    /// <summary>
    ///     Represents the automation object for the equivalent ReferenceContainerNode object
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [ComVisible(true)]
    public class OAReferences : ConnectionPointContainer,
                                IEventSource<_dispReferencesEvents>,
                                References,
                                ReferencesEvents
    {
        private readonly ReferenceContainerNode container;

        public OAReferences(ReferenceContainerNode containerNode)
        {
            container = containerNode;
            AddEventSource(this);
            container.OnChildAdded += OnReferenceAdded;
            container.OnChildRemoved += OnReferenceRemoved;
        }

        #region Private Members

        private Reference AddFromSelectorData(VSCOMPONENTSELECTORDATA selector, string wrapperTool = null)
        {
            ReferenceNode refNode = container.AddReferenceFromSelectorData(selector, wrapperTool);
            if (null == refNode)
            {
                return null;
            }

            return refNode.Object as Reference;
        }

        private Reference FindByName(string stringIndex)
        {
            foreach (Reference refNode in this)
            {
                if (0 == string.Compare(refNode.Name, stringIndex, StringComparison.Ordinal))
                {
                    return refNode;
                }
            }
            return null;
        }

        #endregion

        #region References Members

        public Reference Add(string bstrPath)
        {
            if (string.IsNullOrEmpty(bstrPath))
            {
                return null;
            }
            VSCOMPONENTSELECTORDATA selector = new VSCOMPONENTSELECTORDATA();
            selector.type = VSCOMPONENTTYPE.VSCOMPONENTTYPE_File;
            selector.bstrFile = bstrPath;

            return AddFromSelectorData(selector);
        }

        public Reference AddActiveX(string bstrTypeLibGuid, int lMajorVer, int lMinorVer, int lLocaleId,
                                    string bstrWrapperTool)
        {
            VSCOMPONENTSELECTORDATA selector = new VSCOMPONENTSELECTORDATA();
            selector.type = VSCOMPONENTTYPE.VSCOMPONENTTYPE_Com2;
            selector.guidTypeLibrary = new Guid(bstrTypeLibGuid);
            selector.lcidTypeLibrary = (uint)lLocaleId;
            selector.wTypeLibraryMajorVersion = (ushort)lMajorVer;
            selector.wTypeLibraryMinorVersion = (ushort)lMinorVer;

            return AddFromSelectorData(selector, bstrWrapperTool);
        }

        public Reference AddProject(EnvDTE.Project project)
        {
            if (null == project)
            {
                return null;
            }
            // Get the soulution.
            IVsSolution solution = container.ProjectMgr.Site.GetService(typeof(SVsSolution)) as IVsSolution;
            if (null == solution)
            {
                return null;
            }

            // Get the hierarchy for this project.
            IVsHierarchy projectHierarchy;
            ErrorHandler.ThrowOnFailure(solution.GetProjectOfUniqueName(project.UniqueName, out projectHierarchy));

            // Create the selector data.
            VSCOMPONENTSELECTORDATA selector = new VSCOMPONENTSELECTORDATA();
            selector.type = VSCOMPONENTTYPE.VSCOMPONENTTYPE_Project;

            // Get the project reference string.
            ErrorHandler.ThrowOnFailure(solution.GetProjrefOfProject(projectHierarchy, out selector.bstrProjRef));

            selector.bstrTitle = project.Name;
            selector.bstrFile = Path.GetDirectoryName(project.FullName);

            return AddFromSelectorData(selector);
        }

        public EnvDTE.Project ContainingProject
        {
            get { return container.ProjectMgr.GetAutomationObject() as EnvDTE.Project; }
        }

        public int Count
        {
            get { return container.EnumReferences().Count; }
        }

        public DTE DTE
        {
            get { return container.ProjectMgr.Site.GetService(typeof(DTE)) as DTE; }
        }

        public Reference Find(string bstrIdentity)
        {
            if (string.IsNullOrEmpty(bstrIdentity))
            {
                return null;
            }
            foreach (Reference refNode in this)
            {
                if (null != refNode)
                {
                    if (0 == string.Compare(bstrIdentity, refNode.Identity, StringComparison.Ordinal))
                    {
                        return refNode;
                    }
                }
            }
            return null;
        }

        public IEnumerator GetEnumerator()
        {
            List<Reference> references = new List<Reference>();
            IEnumerator baseEnum = container.EnumReferences().GetEnumerator();
            if (null == baseEnum)
            {
                return references.GetEnumerator();
            }
            while (baseEnum.MoveNext())
            {
                ReferenceNode refNode = baseEnum.Current as ReferenceNode;
                if (null == refNode)
                {
                    continue;
                }
                Reference reference = refNode.Object as Reference;
                if (null != reference)
                {
                    references.Add(reference);
                }
            }
            return references.GetEnumerator();
        }

        public Reference Item(object index)
        {
            string stringIndex = index as string;
            if (null != stringIndex)
            {
                return FindByName(stringIndex);
            }
            // Note that this cast will throw if the index is not convertible to int.
            int intIndex = (int)index;
            IList<ReferenceNode> refs = container.EnumReferences();
            if (null == refs)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if ((intIndex <= 0) || (intIndex > refs.Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            // Let the implementation of IList<> throw in case of index not correct.
            return refs[intIndex - 1].Object as Reference;
        }

        public object Parent
        {
            get { return container.Parent.Object; }
        }

        #endregion

        #region _dispReferencesEvents_Event Members

        public event _dispReferencesEvents_ReferenceAddedEventHandler ReferenceAdded;
        public event _dispReferencesEvents_ReferenceChangedEventHandler ReferenceChanged;
        public event _dispReferencesEvents_ReferenceRemovedEventHandler ReferenceRemoved;

        #endregion

        #region Callbacks for the HierarchyNode events

        private void OnReferenceAdded(object sender, HierarchyNodeEventArgs args)
        {
            // Validate the parameters.
            if ((container != sender as ReferenceContainerNode) ||
                (null == args) || (null == args.Child))
            {
                return;
            }

            // Check if there is any sink for this event.
            if (null == ReferenceAdded)
            {
                return;
            }

            // Check that the removed item implements the Reference interface.
            Reference reference = args.Child.Object as Reference;
            if (null != reference)
            {
                ReferenceAdded(reference);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Support for this has not yet been added")]
        private void OnReferenceChanged(object sender, HierarchyNodeEventArgs args)
        {
            // Validate the parameters.
            if ((container != sender as ReferenceContainerNode) ||
                (null == args) || (null == args.Child))
            {
                return;
            }

            // Check if there is any sink for this event.
            if (null == ReferenceChanged)
            {
                return;
            }

            // Check that the removed item implements the Reference interface.
            Reference reference = args.Child.Object as Reference;
            if (null != reference)
            {
                ReferenceChanged(reference);
            }
        }

        private void OnReferenceRemoved(object sender, HierarchyNodeEventArgs args)
        {
            // Validate the parameters.
            if ((container != sender as ReferenceContainerNode) ||
                (null == args) || (null == args.Child))
            {
                return;
            }

            // Check if there is any sink for this event.
            if (null == ReferenceRemoved)
            {
                return;
            }

            // Check that the removed item implements the Reference interface.
            Reference reference = args.Child.Object as Reference;
            if (null != reference)
            {
                ReferenceRemoved(reference);
            }
        }

        #endregion

        #region IEventSource<_dispReferencesEvents> Members

        void IEventSource<_dispReferencesEvents>.OnSinkAdded(_dispReferencesEvents sink)
        {
            ReferenceAdded += sink.ReferenceAdded;
            ReferenceChanged += sink.ReferenceChanged;
            ReferenceRemoved += sink.ReferenceRemoved;
        }

        void IEventSource<_dispReferencesEvents>.OnSinkRemoved(_dispReferencesEvents sink)
        {
            ReferenceAdded -= sink.ReferenceAdded;
            ReferenceChanged -= sink.ReferenceChanged;
            ReferenceRemoved -= sink.ReferenceRemoved;
        }

        #endregion
    }
}
