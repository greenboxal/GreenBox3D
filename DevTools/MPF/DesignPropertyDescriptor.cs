// DesignPropertyDescriptor.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Project
{
    /// <summary>
    ///     The purpose of DesignPropertyDescriptor is to allow us to customize the
    ///     display name of the property in the property grid.  None of the CLR
    ///     implementations of PropertyDescriptor allow you to change the DisplayName.
    /// </summary>
    public class DesignPropertyDescriptor : PropertyDescriptor
    {
        private readonly string displayName; // Custom display name
        private readonly Hashtable editors = new Hashtable(); // Type -> editor instance
        private readonly PropertyDescriptor property; // Base property descriptor
        private TypeConverter converter;

        /// <summary>
        ///     Constructor.  Copy the base property descriptor and also hold a pointer
        ///     to it for calling its overridden abstract methods.
        /// </summary>
        public DesignPropertyDescriptor(PropertyDescriptor prop)
            : base(prop)
        {
            if (prop == null)
            {
                throw new ArgumentNullException("prop");
            }

            property = prop;

            DisplayNameAttribute attr = prop.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

            if (attr != null)
            {
                displayName = attr.DisplayName;
            }
            else
            {
                displayName = prop.Name;
            }
        }

        /// <summary>
        ///     Delegates to base.
        /// </summary>
        public override string DisplayName
        {
            get { return displayName; }
        }

        /// <summary>
        ///     Delegates to base.
        /// </summary>
        public override Type ComponentType
        {
            get { return property.ComponentType; }
        }

        /// <summary>
        ///     Delegates to base.
        /// </summary>
        public override bool IsReadOnly
        {
            get { return property.IsReadOnly; }
        }

        /// <summary>
        ///     Delegates to base.
        /// </summary>
        public override Type PropertyType
        {
            get { return property.PropertyType; }
        }

        /// <summary>
        ///     Return type converter for property
        /// </summary>
        public override TypeConverter Converter
        {
            get
            {
                if (converter == null)
                {
                    PropertyPageTypeConverterAttribute attr =
                        (PropertyPageTypeConverterAttribute)Attributes[typeof(PropertyPageTypeConverterAttribute)];
                    if (attr != null && attr.ConverterType != null)
                    {
                        converter = (TypeConverter)CreateInstance(attr.ConverterType);
                    }

                    if (converter == null)
                    {
                        converter = TypeDescriptor.GetConverter(PropertyType);
                    }
                }
                return converter;
            }
        }

        /// <summary>
        ///     Delegates to base.
        /// </summary>
        public override object GetEditor(Type editorBaseType)
        {
            object editor = editors[editorBaseType];
            if (editor == null)
            {
                for (int i = 0; i < Attributes.Count; i++)
                {
                    EditorAttribute attr = Attributes[i] as EditorAttribute;
                    if (attr == null)
                    {
                        continue;
                    }
                    Type editorType = Type.GetType(attr.EditorBaseTypeName);
                    if (editorBaseType == editorType)
                    {
                        Type type = GetTypeFromNameProperty(attr.EditorTypeName);
                        if (type != null)
                        {
                            editor = CreateInstance(type);
                            editors[type] = editor; // cache it
                            break;
                        }
                    }
                }
            }
            return editor;
        }

        /// <summary>
        ///     Convert name to a Type object.
        /// </summary>
        public virtual Type GetTypeFromNameProperty(string typeName)
        {
            return Type.GetType(typeName);
        }

        /// <summary>
        ///     Delegates to base.
        /// </summary>
        public override bool CanResetValue(object component)
        {
            bool result = property.CanResetValue(component);
            return result;
        }

        /// <summary>
        ///     Delegates to base.
        /// </summary>
        public override object GetValue(object component)
        {
            object value = property.GetValue(component);
            return value;
        }

        /// <summary>
        ///     Delegates to base.
        /// </summary>
        public override void ResetValue(object component)
        {
            property.ResetValue(component);
        }

        /// <summary>
        ///     Delegates to base.
        /// </summary>
        public override void SetValue(object component, object value)
        {
            property.SetValue(component, value);
        }

        /// <summary>
        ///     Delegates to base.
        /// </summary>
        public override bool ShouldSerializeValue(object component)
        {
            bool result = property.ShouldSerializeValue(component);
            return result;
        }
    }
}
