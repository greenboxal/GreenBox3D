// IHardwareBuffer.cs
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
    // TODO: Implement GetData<T>
    public interface IHardwareBuffer : IGraphicsResource
    {
        BufferUsage BufferUsage { get; }
        int ElementCount { get; }

        void SetData<T>(T[] data) where T : struct;
        void SetData<T>(T[] data, int offset, int count) where T : struct;
        void SetData<T>(int offsetInBytes, T[] data, int offset, int count) where T : struct;
    }
}
