// ContentProcessor.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using GreenBox3D.ContentPipeline.CompilerServices;

namespace GreenBox3D.ContentPipeline
{
    public abstract class ContentProcessor<TInput, TOutput> : IContentProcessor
    {
        #region Public Methods and Operators

        protected abstract TOutput Process(TInput input, ContentProcessorContext context);

        #endregion

        #region Explicit Interface Methods

        object IContentProcessor.Process(object input, ContentProcessorContext context)
        {
            Type type = GetType();

            foreach (KeyValuePair<string, object> kvp in context.Parameters)
            {
                PropertyInfo info = type.GetProperty(kvp.Key,
                                                     BindingFlags.SetProperty | BindingFlags.IgnoreCase |
                                                     BindingFlags.Public | BindingFlags.Instance);

                if (info == null)
                    continue;

                TypeConverter converter = TypeDescriptor.GetConverter(info.PropertyType);

                if (!converter.IsValid(kvp.Value))
                    continue;

                info.SetValue(this, converter.ConvertFrom(kvp.Value));
            }

            return Process((TInput)input, context);
        }

        #endregion
    }
}
