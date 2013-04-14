// ContentTypeReaderAttribute.cs
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

namespace GreenBox3D.Content
{
    public sealed class ContentTypeReaderAttribute : Attribute
    {
        #region Constructors and Destructors

        public ContentTypeReaderAttribute()
        {
            Extension = ".bin";
        }

        #endregion

        #region Public Properties

        public string Extension { get; set; }

        #endregion
    }
}
