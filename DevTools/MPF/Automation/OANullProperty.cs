// OANullProperty.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Runtime.InteropServices;
using EnvDTE;

namespace Microsoft.VisualStudio.Project.Automation
{
    /// <summary>
    ///     This object defines a so called null object that is returned as instead of null. This is because callers in VSCore usually crash if a null propery is returned for them.
    /// </summary>
    [CLSCompliant(false), ComVisible(true)]
    public class OANullProperty : Property
    {
        #region fields

        private readonly OAProperties parent;

        #endregion

        #region ctors

        public OANullProperty(OAProperties parent)
        {
            this.parent = parent;
        }

        #endregion

        #region EnvDTE.Property

        public object Application
        {
            get { return String.Empty; }
        }

        public Properties Collection
        {
            get
            {
                //todo: EnvDTE.Property.Collection
                return parent;
            }
        }

        public DTE DTE
        {
            get { return null; }
        }

        public object get_IndexedValue(object index1, object index2, object index3, object index4)
        {
            return String.Empty;
        }

        public void let_Value(object value)
        {
            //todo: let_Value
        }

        public string Name
        {
            get { return String.Empty; }
        }

        public short NumIndices
        {
            get { return 0; }
        }

        public object Object
        {
            get { return parent.Target; }
            set { }
        }

        public Properties Parent
        {
            get { return parent; }
        }

        public void set_IndexedValue(object index1, object index2, object index3, object index4, object value)
        {
        }

        public object Value
        {
            get { return String.Empty; }
            set { }
        }

        #endregion
    }
}
