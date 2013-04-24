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

        public abstract IVertexBuffer CreateVertexBuffer(IVertexDeclaration vertexDeclaration, int vertexCount,
                                                         BufferUsage usage);

        public virtual IVertexBuffer CreateVertexBuffer(Type elementType, int vertexCount, BufferUsage usage)
        {
            return CreateVertexBuffer(CreateVertexDeclaration(elementType), vertexCount, usage);
        }

        public virtual IVertexDeclaration CreateVertexDeclaration(Type elementType)
        {
            IVertexType vertexType = Activator.CreateInstance(elementType) as IVertexType;

            if (vertexType == null)
                throw new ArgumentException("elementType must implement IVertexType", "elementType");

            return CreateVertexDeclaration(vertexType.VertexDeclaration);
        }

        public virtual IVertexDeclaration CreateVertexDeclaration(VertexElement[] elements)
        {
            return CreateVertexDeclaration(CalculateStride(elements), elements);
        }

        public abstract IVertexDeclaration CreateVertexDeclaration(int stride, VertexElement[] elements);

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

        private static int CalculateStride(IEnumerable<VertexElement> elements)
        {
            int stride = 0;

            foreach (VertexElement element in elements)
                stride = Math.Max(stride, element.Offset + element.SizeInBytes);

            return stride;
        }
    }
}
