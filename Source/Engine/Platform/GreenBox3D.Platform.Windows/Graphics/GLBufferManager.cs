// GLBufferManager.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GLBufferManager : BufferManager
    {
        private readonly WindowsGraphicsDevice _graphicsDevice;

        public GLBufferManager(WindowsGraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public override IIndexBuffer CreateIndexBuffer(IndexElementSize indexElementSize, int indexCount,
                                                       BufferUsage usage)
        {
            return new GLIndexBuffer(_graphicsDevice, indexElementSize, indexCount, usage);
        }

        public override IVertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration, int vertexCount,
                                                         BufferUsage usage)
        {
            return new GLVertexBuffer(_graphicsDevice, vertexDeclaration, vertexCount, usage);
        }

        public override object CreateVertexDeclarationImplementation(VertexDeclaration vertexDeclaration)
        {
            return new VertexDeclarationImplementation(_graphicsDevice, vertexDeclaration);
        }
    }
}
