// GLVertexBuffer.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

#if DESKTOP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public sealed class VertexBuffer : HardwareBuffer
    {
        #region Constructors and Destructors

        public VertexBuffer(VertexDeclaration vertexDeclaration, int vertexCount,
                            BufferUsage usage)
            : base(BufferTarget.ArrayBuffer, vertexDeclaration.VertexStride, vertexCount, usage)
        {
            VertexDeclaration = vertexDeclaration;
        }

        public VertexBuffer(Type elementType, int vertexCount, BufferUsage usage)
            : this(CreateVertexDeclaration(elementType), vertexCount, usage)
        {
        }

        #endregion

        #region Public Properties

        public VertexDeclaration VertexDeclaration { get; private set; }

        #endregion

        private static VertexDeclaration CreateVertexDeclaration(Type elementType)
        {
            IVertexType vertexType = Activator.CreateInstance(elementType) as IVertexType;

            if (vertexType == null)
                throw new ArgumentException("elementType must implement IVertexType", "elementType");

            return vertexType.VertexDeclaration;
        }
    }
}

#endif
