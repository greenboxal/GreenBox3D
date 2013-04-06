// GreenBox3D
// 
// Copyright (c) 2013 The GreenBox Development Inc.
// Copyright (c) 2013 Mono.Xna Team and Contributors
// 
// Licensed under MIT license terms.

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