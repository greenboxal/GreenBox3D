// TextureCollection.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public abstract class TextureCollection : IReadOnlyCollection<ITexture>
    {
        #region Public Properties

        public abstract int Count { get; }

        #endregion

        #region Public Indexers

        public abstract ITexture this[int index] { get; set; }

        #endregion

        #region Public Methods and Operators

        public abstract IEnumerator<ITexture> GetEnumerator();

        #endregion

        #region Explicit Interface Methods

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Methods

        public abstract void Apply();

        #endregion
    }
}
