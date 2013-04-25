// GLVertexBuffer.cs
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
    public class VertexBuffer : HardwareBuffer, IVertexBuffer
    {
        #region Constructors and Destructors

        public VertexBuffer(GraphicsDevice graphicsDevice, IVertexDeclaration vertexDeclaration, int vertexCount,
                            BufferUsage usage)
            : base(graphicsDevice, BufferTarget.ArrayBuffer, vertexDeclaration.VertexStride, vertexCount, usage)
        {
            VertexDeclaration = vertexDeclaration;
        }

        #endregion

        #region Public Properties

        public IVertexDeclaration VertexDeclaration { get; private set; }

        #endregion
    }
}
