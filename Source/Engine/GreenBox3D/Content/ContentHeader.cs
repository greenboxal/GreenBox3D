// ContentHeader.cs
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
