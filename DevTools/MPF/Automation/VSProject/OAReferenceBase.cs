// OAReferenceBase.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using VSLangProj;

namespace Microsoft.VisualStudio.Project.Automation
{
    /// <summary>
    ///     Represents the automation equivalent of ReferenceNode
    /// </summary>
    /// <typeparam name="RefType"></typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
    [ComVisible(true)]
    public abstract class OAReferenceBase<RefType> : Reference
        where RefType : ReferenceNode
    {
        #region fields

        private readonly RefType referenceNode;

        #endregion

        #region ctors

        protected OAReferenceBase(RefType referenceNode)
        {
            this.referenceNode = referenceNode;
        }

        #endregion

        #region properties

        protected RefType BaseReferenceNode
        {
            get { return referenceNode; }
        }

        #endregion

        #region Reference Members

        public virtual int BuildNumber
        {
            get { return 0; }
        }

        public virtual References Collection
        {
            get { return BaseReferenceNode.Parent.Object as References; }
        }

        public virtual EnvDTE.Project ContainingProject
        {
            get { return BaseReferenceNode.ProjectMgr.GetAutomationObject() as EnvDTE.Project; }
        }

        public virtual bool CopyLocal
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public virtual string Culture
        {
            get { throw new NotImplementedException(); }
        }

        public virtual DTE DTE
        {
            get { return BaseReferenceNode.ProjectMgr.Site.GetService(typeof(DTE)) as DTE; }
        }

        public virtual string Description
        {
            get { return Name; }
        }

        public virtual string ExtenderCATID
        {
            get { throw new NotImplementedException(); }
        }

        public virtual object ExtenderNames
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string Identity
        {
            get { throw new NotImplementedException(); }
        }

        public virtual int MajorVersion
        {
            get { return 0; }
        }

        public virtual int MinorVersion
        {
            get { return 0; }
        }

        public virtual string Name
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string Path
        {
            get { return BaseReferenceNode.Url; }
        }

        public virtual string PublicKeyToken
        {
            get { throw new NotImplementedException(); }
        }

        public virtual void Remove()
        {
            BaseReferenceNode.Remove(false);
        }

        public virtual int RevisionNumber
        {
            get { return 0; }
        }

        public virtual EnvDTE.Project SourceProject
        {
            get { return null; }
        }

        public virtual bool StrongName
        {
            get { return false; }
        }

        public virtual prjReferenceType Type
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string Version
        {
            get { return new Version().ToString(); }
        }

        public virtual object get_Extender(string ExtenderName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
