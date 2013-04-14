// ContentImporter.cs
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
using GreenBox3D.ContentPipeline.CompilerServices;

namespace GreenBox3D.ContentPipeline
{
    public abstract class ContentImporter<TOutput> : IContentImporter
    {
        #region Public Methods and Operators

        protected abstract TOutput Import(string filename, ContentImporterContext context);

        #endregion

        #region Explicit Interface Methods

        object IContentImporter.Import(string filename, ContentImporterContext context)
        {
            return Import(filename, context);
        }

        #endregion
    }
}
