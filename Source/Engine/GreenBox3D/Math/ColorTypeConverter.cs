// ColorTypeConverter.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D
{
    public class ColorTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(String))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] parts = (value as string).Split(',');

                if (parts.Length == 3)
                    return new Color(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]));

                if (parts.Length == 4)
                    return new Color(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]),
                                     byte.Parse(parts[3]));
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            if (value is string)
            {
                string[] parts = (value as string).Split(',');
                byte tmp;

                if (parts.Length != 3 && parts.Length != 4)
                    return false;

                if (!byte.TryParse(parts[0], out tmp) || !byte.TryParse(parts[1], out tmp) ||
                    !byte.TryParse(parts[2], out tmp))
                    return false;

                if (parts.Length == 4 && !byte.TryParse(parts[3], out tmp))
                    return false;

                return true;
            }

            return base.IsValid(context, value);
        }
    }
}
