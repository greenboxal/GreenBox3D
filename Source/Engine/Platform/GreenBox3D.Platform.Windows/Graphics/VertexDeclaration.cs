// GLVertexDeclaration.cs
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
using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class VertexDeclaration : IVertexDeclaration
    {
        private static VertexDeclaration _lastUsed;
        private static IntPtr _lastUsedPointer;
        private static Shader _lastUsedShader;

        private readonly VertexElement[] _elements;
        private readonly InternalVertexElement[] _glelements;
        private readonly WindowsGraphicsDevice _graphicsDevice;
        private readonly int _stride;

        public VertexDeclaration(WindowsGraphicsDevice graphicsDevice, int stride, VertexElement[] elements)
        {
            _graphicsDevice = graphicsDevice;
            _stride = stride;
            _elements = (VertexElement[])elements.Clone();
            _glelements = new InternalVertexElement[_elements.Length];

            for (int i = 0; i < _glelements.Length; i++)
                _glelements[i] = new InternalVertexElement(_elements[i]);
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
            if (_lastUsed == this && _lastUsedPointer == baseAddress && _lastUsedShader == _graphicsDevice.ActiveShader)
                return;

            for (int i = 0; i < _graphicsDevice.ActiveShader.Input.Length; i++)
            {
                VertexElement element = null;
                InternalVertexElement gle = null;
                ShaderInput input = _graphicsDevice.ActiveShader.Input[i];

                if (input.Index == -1)
                    continue;

                for (int j = 0; j < _elements.Length; j++)
                {
                    if (_elements[j].VertexElementUsage == input.Usage && _elements[j].UsageIndex == input.UsageIndex)
                    {
                        element = _elements[j];
                        gle = _glelements[j];
                        break;
                    }
                }

                if (element != null)
                {
                    GL.EnableVertexAttribArray(input.Index);
                    GL.VertexAttribPointer(input.Index, gle.ElementCount, gle.VertexAttribPointerType, !gle.IsNormalized,
                                           _stride, baseAddress + element.Offset);
                }
                else
                {
                    GL.DisableVertexAttribArray(input.Index);
                }
            }

            _lastUsed = this;
            _lastUsedPointer = baseAddress;
            _lastUsedShader = _graphicsDevice.ActiveShader;
        }
    }
}
