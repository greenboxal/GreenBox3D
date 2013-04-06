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
    public class ContentHeader
    {
        #region Constructors and Destructors

        public ContentHeader(string magic, Version version, Encoding encoding = null)
        {
            Magic = magic;
            Version = version;
            Encoding = encoding ?? Encoding.UTF8;
        }

        #endregion

        #region Public Properties

        public Encoding Encoding { get; private set; }
        public string Magic { get; private set; }
        public Version Version { get; private set; }

        #endregion
    }
}