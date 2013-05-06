// Attributes.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ContentImporterAttribute : Attribute
    {
        #region Constructors and Destructors

        public ContentImporterAttribute(params string[] extensions)
        {
            Extensions = extensions ?? new string[0];
        }

        #endregion

        #region Public Properties

        public string DefaultProcessor { get; set; }
        public string DisplayName { get; set; }
        public string[] Extensions { get; set; }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ContentProcessorAttribute : Attribute
    {
        #region Constructors and Destructors

        public ContentProcessorAttribute(string writer)
        {
            Writer = writer;
        }

        #endregion

        #region Public Properties

        public string DisplayName { get; set; }
        public string Writer { get; set; }

        #endregion
    }

    public sealed class ContentTypeWriterAttribute : Attribute
    {
        #region Constructors and Destructors

        public ContentTypeWriterAttribute()
        {
            Extension = ".bin";
        }

        #endregion

        #region Public Properties

        public string Extension { get; set; }

        #endregion
    }
}
