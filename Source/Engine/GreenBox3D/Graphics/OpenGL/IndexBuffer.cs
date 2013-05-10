// IndexBuffer.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
#if DESKTOP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public sealed class IndexBuffer : HardwareBuffer
    {
        internal DrawElementsType DrawElementsType;
        internal int ElementSize;

        public IndexElementSize IndexElementSize { get; private set; }

        public IndexBuffer(IndexElementSize indexElementSize, BufferUsage usage)
            : base(
                BufferTarget.ElementArrayBuffer, usage)
        {
            IndexElementSize = indexElementSize;

            switch (indexElementSize)
            {
                case IndexElementSize.EightBits:
                    DrawElementsType = DrawElementsType.UnsignedByte;
                    ElementSize = 1;
                    break;
                case IndexElementSize.SixteenBits:
                    DrawElementsType = DrawElementsType.UnsignedShort;
                    ElementSize = 2;
                    break;
                case IndexElementSize.ThirtyTwoBits:
                    DrawElementsType = DrawElementsType.UnsignedInt;
                    ElementSize = 4;
                    break;
            }
        }

        public override void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, BufferID);
            GraphicsDevice.State.ActiveIndexBuffer = this;

            if (GraphicsDevice.State.ActiveVertexState != null)
                GraphicsDevice.State.ActiveVertexState.IndexBuffer = this;
        }

        public override void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GraphicsDevice.State.ActiveIndexBuffer = null;

            if (GraphicsDevice.State.ActiveVertexState != null)
                GraphicsDevice.State.ActiveVertexState.IndexBuffer = null;
        }
    }
}

#endif
