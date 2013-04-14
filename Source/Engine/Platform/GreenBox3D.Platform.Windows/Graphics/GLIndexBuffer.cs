// GLIndexBuffer.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GLIndexBuffer : HardwareBuffer, IIndexBuffer
    {
        #region Fields

        internal DrawElementsType DrawElementsType;

        #endregion

        #region Constructors and Destructors

        public GLIndexBuffer(GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, int indexCount,
                             BufferUsage usage)
            : base(
                graphicsDevice, BufferTarget.ElementArrayBuffer, GetElementSizeInBytes(indexElementSize), indexCount,
                usage)
        {
            IndexElementSize = indexElementSize;

            switch (indexElementSize)
            {
                case IndexElementSize.EightBits:
                    DrawElementsType = DrawElementsType.UnsignedByte;
                    break;
                case IndexElementSize.SixteenBits:
                    DrawElementsType = DrawElementsType.UnsignedShort;
                    break;
                case IndexElementSize.ThirtyTwoBits:
                    DrawElementsType = DrawElementsType.UnsignedInt;
                    break;
            }
        }

        #endregion

        #region Public Properties

        public IndexElementSize IndexElementSize { get; private set; }

        #endregion

        #region Methods

        private static int GetElementSizeInBytes(IndexElementSize size)
        {
            switch (size)
            {
                case IndexElementSize.EightBits:
                    return 1;
                case IndexElementSize.SixteenBits:
                    return 2;
                case IndexElementSize.ThirtyTwoBits:
                    return 4;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion
    }
}
