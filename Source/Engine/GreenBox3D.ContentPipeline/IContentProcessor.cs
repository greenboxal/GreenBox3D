// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
//
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GreenBox3D.ContentPipeline.CompilerServices;

namespace GreenBox3D.ContentPipeline
{
    public interface IContentProcessor
    {
        #region Public Methods and Operators

        object Process(object input, ContentProcessorContext context);

        #endregion
    }
}