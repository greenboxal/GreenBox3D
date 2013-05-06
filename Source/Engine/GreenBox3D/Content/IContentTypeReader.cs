﻿// IContentTypeReader.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Content
{
    public interface IContentTypeReader
    {
        #region Public Methods and Operators

        object Load(ContentManager manager, Stream stream);

        #endregion
    }
}
