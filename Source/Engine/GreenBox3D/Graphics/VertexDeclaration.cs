// VertexDeclaration.cs
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
    public class VertexDeclaration
    {
        #region Fields

        private readonly VertexElement[] _vertexElements;
        private object _implementation;

        #endregion

        #region Constructors and Destructors

        public VertexDeclaration(int vertexStride, VertexElement[] elements)
        {
            _vertexElements = (VertexElement[])elements.Clone();
            VertexStride = vertexStride;
        }

        public VertexDeclaration(VertexElement[] elements)
            : this(CalculateStride(elements), elements)
        {
        }

        #endregion

        #region Public Properties

        public int VertexStride { get; private set; }

        #endregion

        #region Public Methods and Operators

        public static VertexDeclaration FromType(Type elementType)
        {
            IVertexType vertexType = Activator.CreateInstance(elementType) as IVertexType;

            if (vertexType == null)
                throw new ArgumentException("elementType must implement IVertexType", "elementType");

            return vertexType.VertexDeclaration;
        }

        public VertexElement[] GetVertexElements()
        {
            return _vertexElements;
        }

        public object GetImplementation(GraphicsDevice graphicsDevice)
        {
            if (_implementation == null)
                _implementation = graphicsDevice.BufferManager.CreateVertexDeclarationImplementation(this);

            return _implementation;
        }

        #endregion

        #region Methods

        private static int CalculateStride(IEnumerable<VertexElement> elements)
        {
            int stride = 0;

            foreach (VertexElement element in elements)
                stride = Math.Max(stride, element.Offset + element.SizeInBytes);

            return stride;
        }

        #endregion
    }
}
