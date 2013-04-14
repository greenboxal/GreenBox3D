// TypeConverters.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Project
{
    public class OutputTypeConverter : EnumConverter
    {
        public OutputTypeConverter()
            : base(typeof(OutputType))
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;

            if (str != null)
            {
                if (str == SR.GetString(SR.Exe, culture))
                    return OutputType.Exe;
                if (str == SR.GetString(SR.Library, culture))
                    return OutputType.Library;
                if (str == SR.GetString(SR.WinExe, culture))
                    return OutputType.WinExe;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string result = null;
                // In some cases if multiple nodes are selected the windows form engine
                // calls us with a null value if the selected node's property values are not equal
                if (value != null)
                {
                    result = SR.GetString(((OutputType)value).ToString(), culture);
                }
                else
                {
                    result = SR.GetString(OutputType.Library.ToString(), culture);
                }

                if (result != null)
                    return result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new[] { OutputType.Exe, OutputType.Library, OutputType.WinExe });
        }
    }

    public class DebugModeConverter : EnumConverter
    {
        public DebugModeConverter()
            : base(typeof(DebugMode))
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;

            if (str != null)
            {
                if (str == SR.GetString(SR.Program, culture))
                    return DebugMode.Program;

                if (str == SR.GetString(SR.Project, culture))
                    return DebugMode.Project;

                if (str == SR.GetString(SR.URL, culture))
                    return DebugMode.URL;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string result = null;
                // In some cases if multiple nodes are selected the windows form engine
                // calls us with a null value if the selected node's property values are not equal
                if (value != null)
                {
                    result = SR.GetString(((DebugMode)value).ToString(), culture);
                }
                else
                {
                    result = SR.GetString(DebugMode.Program.ToString(), culture);
                }

                if (result != null)
                    return result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new[] { DebugMode.Program, DebugMode.Project, DebugMode.URL });
        }
    }

    public class BuildActionConverter : EnumConverter
    {
        public BuildActionConverter()
            : base(typeof(BuildAction))
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;

            if (str != null)
            {
                if (str == SR.GetString(SR.Compile, culture))
                    return BuildAction.Compile;

                if (str == SR.GetString(SR.Content, culture))
                    return BuildAction.Content;

                if (str == SR.GetString(SR.EmbeddedResource, culture))
                    return BuildAction.EmbeddedResource;

                if (str == SR.GetString(SR.None, culture))
                    return BuildAction.None;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string result = null;

                // In some cases if multiple nodes are selected the windows form engine
                // calls us with a null value if the selected node's property values are not equal
                // Example of windows form engine passing us null: File set to Compile, Another file set to None, bot nodes are selected, and the build action combo is clicked.
                if (value != null)
                {
                    result = SR.GetString(((BuildAction)value).ToString(), culture);
                }
                else
                {
                    result = SR.GetString(BuildAction.None.ToString(), culture);
                }

                if (result != null)
                    return result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return
                new StandardValuesCollection(new[]
                { BuildAction.Compile, BuildAction.Content, BuildAction.EmbeddedResource, BuildAction.None });
        }
    }

    public class FrameworkNameConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;

            if (str != null)
            {
                return new FrameworkName(str);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var name = value as FrameworkName;
                if (name != null)
                {
                    return name.FullName;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IServiceProvider sp = ProjectNode.ServiceProvider;
            var multiTargetService = sp.GetService(typeof(SVsFrameworkMultiTargeting)) as IVsFrameworkMultiTargeting;
            if (multiTargetService == null)
            {
                Trace.TraceError("Unable to acquire the SVsFrameworkMultiTargeting service.");
                return new StandardValuesCollection(new string[0]);
            }
            Array frameworks;
            Marshal.ThrowExceptionForHR(multiTargetService.GetSupportedFrameworks(out frameworks));
            return new StandardValuesCollection(
                frameworks.Cast<string>().Select(fx => new FrameworkName(fx)).ToArray()
                );
        }
    }
}
