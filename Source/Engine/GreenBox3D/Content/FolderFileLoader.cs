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
    public class FolderFileLoader : IFileLoader
    {
        #region Fields

        private readonly string _BasePath;

        #endregion

        #region Constructors and Destructors

        public FolderFileLoader(string basePath)
        {
            _BasePath = basePath;

            if (!Directory.Exists(_BasePath))
                throw new IOException("The specified path doesn't exist");
        }

        #endregion

        #region Public Methods and Operators

        public Stream OpenFile(string filename)
        {
            try
            {
                return new FileStream(Path.Combine(_BasePath, filename), FileMode.Open);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}