// FileManager.cs
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
    public static class FileManager
    {
        #region Static Fields

        private static readonly List<IFileLoader> _Loaders;

        #endregion

        #region Constructors and Destructors

        static FileManager()
        {
            _Loaders = new List<IFileLoader>();
        }

        #endregion

        #region Public Methods and Operators

        public static Stream OpenFile(string path)
        {
            return _Loaders.Select(loader => loader.OpenFile(path)).FirstOrDefault(s => s != null);
        }

        public static void RegisterLoader(IFileLoader loader)
        {
            _Loaders.Add(loader);
        }

        #endregion
    }
}
