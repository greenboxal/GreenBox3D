// FolderFileLoader.cs
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
