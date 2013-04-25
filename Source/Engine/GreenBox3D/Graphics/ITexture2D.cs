// ITexture2D.cs
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

namespace GreenBox3D.Graphics
{
    public interface ITexture2D : ITexture
    {
        int Width { get; }
        int Height { get; }

        void SetData<T>(T[] data) where T : struct;
        void SetData<T>(int level, T[] data, int startIndex) where T : struct;
        void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex) where T : struct;
    }
}
