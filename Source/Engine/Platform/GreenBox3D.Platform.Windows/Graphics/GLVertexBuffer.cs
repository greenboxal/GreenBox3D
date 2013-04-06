// GreenBox3D
// 
// Copyright (c) 2013 The GreenBox Development Inc.
// Copyright (c) 2013 Mono.Xna Team and Contributors
// 
// Licensed under MIT license terms.

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
    public class GLVertexBuffer : HardwareBuffer, IVertexBuffer
    {
        #region Constructors and Destructors

        public GLVertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage usage)
            : base(graphicsDevice, BufferTarget.ArrayBuffer, vertexDeclaration.VertexStride, vertexCount, usage)
        {
            VertexDeclaration = vertexDeclaration;
        }

        #endregion

        #region Public Properties

        public VertexDeclaration VertexDeclaration { get; private set; }

        #endregion
    }
}