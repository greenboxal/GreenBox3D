// VertexDeclaration.cs
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
    public sealed class VertexDeclaration
    {
        private static VertexDeclaration _lastUsed;
        private static IntPtr _lastUsedPointer;
        private static Shader _lastUsedShader;

        private readonly VertexElement[] _elements;
        private readonly int _stride;

        public VertexDeclaration(int stride, VertexElement[] elements)
        {
            _stride = stride;
            _elements = (VertexElement[])elements.Clone();
        }

        public VertexDeclaration(VertexElement[] elements)
            : this(CalculateStride(elements), elements)
        {

        }

        public int VertexStride
        {
            get { return _stride; }
        }

        public VertexElement[] GetVertexElements()
        {
            return (VertexElement[])_elements.Clone();
        }

        public void Bind(IntPtr baseAddress)
        {
            if (_lastUsed == this && _lastUsedPointer == baseAddress && _lastUsedShader == GraphicsDevice.ActiveDevice.ActiveShader)
                return;

            for (int i = 0; i < GraphicsDevice.ActiveDevice.ActiveShader.Input.Length; i++)
            {
                VertexElement element = null;
                ShaderInput input = GraphicsDevice.ActiveDevice.ActiveShader.Input[i];

                if (input.Index == -1)
                    continue;

                for (int j = 0; j < _elements.Length; j++)
                {
                    if (_elements[j].VertexElementUsage == input.Usage && _elements[j].UsageIndex == input.UsageIndex)
                    {
                        element = _elements[j];
                        break;
                    }
                }

                if (element != null)
                {
                    GL.EnableVertexAttribArray(input.Index);
                    GL.VertexAttribPointer(input.Index, element.ElementCount, element.VertexAttribPointerType,
                                           !element.IsNormalized,
                                           _stride, baseAddress + element.Offset);
                }
                else
                {
                    GL.DisableVertexAttribArray(input.Index);
                }
            }

            _lastUsed = this;
            _lastUsedPointer = baseAddress;
            _lastUsedShader = GraphicsDevice.ActiveDevice.ActiveShader;
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

#endif
