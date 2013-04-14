// Attributes.cs
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

namespace Microsoft.VisualStudio.Project
{
    /// <summary>
    ///     Defines a type converter.
    /// </summary>
    /// <remarks>
    ///     This is needed to get rid of the type TypeConverter type that could not give back the Type we were passing to him.
    ///     We do not want to use reflection to get the type back from the  ConverterTypeName. Also the GetType methos does not spwan converters from other assemblies.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments"),
     AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)
    ]
    public sealed class PropertyPageTypeConverterAttribute : Attribute
    {
        #region fields

        private readonly Type converterType;

        #endregion

        #region ctors

        public PropertyPageTypeConverterAttribute(Type type)
        {
            converterType = type;
        }

        #endregion

        #region properties

        public Type ConverterType
        {
            get { return converterType; }
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = false,
        AllowMultiple = false)]
    internal sealed class LocDisplayNameAttribute : DisplayNameAttribute
    {
        #region fields

        private readonly string name;

        #endregion

        #region ctors

        public LocDisplayNameAttribute(string name)
        {
            this.name = name;
        }

        #endregion

        #region properties

        public override string DisplayName
        {
            get
            {
                string result = SR.GetString(name, CultureInfo.CurrentUICulture);
                if (result == null)
                {
                    Debug.Assert(false, "String resource '" + name + "' is missing");
                    result = name;
                }
                return result;
            }
        }

        #endregion
    }
}
