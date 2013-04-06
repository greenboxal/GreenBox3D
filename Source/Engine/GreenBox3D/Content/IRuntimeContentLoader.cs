// GreenBox3D
// 
// Copyright (c) 2013 The GreenBox Development Inc.
// Copyright (c) 2013 Mono.Xna Team and Contributors
// 
// Licensed under MIT license terms.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Content
{
    internal interface IRuntimeContentLoader
    {
        #region Public Methods and Operators

        T LoadContent<T>(ContentManager manager, string filename) where T : class;

        #endregion
    }
}