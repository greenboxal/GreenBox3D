// LocalizableProperties.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Project
{
    /// <summary>
    ///     Enables a managed object to expose properties and attributes for COM objects.
    /// </summary>
    [ComVisible(true)]
    public class LocalizableProperties : ICustomTypeDescriptor
    {
        #region ICustomTypeDescriptor

        public virtual AttributeCollection GetAttributes()
        {
            AttributeCollection col = TypeDescriptor.GetAttributes(this, true);
            return col;
        }

        public virtual EventDescriptor GetDefaultEvent()
        {
            EventDescriptor ed = TypeDescriptor.GetDefaultEvent(this, true);
            return ed;
        }

        public virtual PropertyDescriptor GetDefaultProperty()
        {
            PropertyDescriptor pd = TypeDescriptor.GetDefaultProperty(this, true);
            return pd;
        }

        public virtual object GetEditor(Type editorBaseType)
        {
            object o = TypeDescriptor.GetEditor(this, editorBaseType, true);
            return o;
        }

        public virtual EventDescriptorCollection GetEvents()
        {
            EventDescriptorCollection edc = TypeDescriptor.GetEvents(this, true);
            return edc;
        }

        public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            EventDescriptorCollection edc = TypeDescriptor.GetEvents(this, attributes, true);
            return edc;
        }

        public virtual object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public virtual PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pcol = GetProperties(null);
            return pcol;
        }

        public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            ArrayList newList = new ArrayList();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this, attributes, true);

            for (int i = 0; i < props.Count; i++)
                newList.Add(CreateDesignPropertyDescriptor(props[i]));

            return new PropertyDescriptorCollection((PropertyDescriptor[])newList.ToArray(typeof(PropertyDescriptor)));
            ;
        }

        public virtual string GetComponentName()
        {
            string name = TypeDescriptor.GetComponentName(this, true);
            return name;
        }

        public virtual TypeConverter GetConverter()
        {
            TypeConverter tc = TypeDescriptor.GetConverter(this, true);
            return tc;
        }

        public virtual string GetClassName()
        {
            return GetType().FullName;
        }

        public virtual DesignPropertyDescriptor CreateDesignPropertyDescriptor(PropertyDescriptor propertyDescriptor)
        {
            return new DesignPropertyDescriptor(propertyDescriptor);
        }

        #endregion ICustomTypeDescriptor
    }
}
