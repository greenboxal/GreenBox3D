// BufferManager.cs
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

namespace GreenBox3D.Graphics
{
    public abstract class BufferManager
    {
        public abstract IIndexBuffer CreateIndexBuffer(IndexElementSize indexElementSize, int indexCount,
                                                       BufferUsage usage);

        public virtual IIndexBuffer CreateIndexBuffer(Type elementType, int indexCount, BufferUsage usage)
        {
            return CreateIndexBuffer(GetElementSizeFromType(elementType), indexCount, usage);
        }

        public abstract IVertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration, int vertexCount,
                                                         BufferUsage usage);

        public virtual IVertexBuffer CreateVertexBuffer(Type elementType, int vertexCount, BufferUsage usage)
        {
            return CreateVertexBuffer(VertexDeclaration.FromType(elementType), vertexCount, usage);
        }

        public abstract object CreateVertexDeclarationImplementation(VertexDeclaration vertexDeclaration);

        private static IndexElementSize GetElementSizeFromType(Type type)
        {
            switch (Marshal.SizeOf(type))
            {
                case 1:
                    return IndexElementSize.EightBits;
                case 2:
                    return IndexElementSize.SixteenBits;
                case 3:
                    return IndexElementSize.ThirtyTwoBits;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
