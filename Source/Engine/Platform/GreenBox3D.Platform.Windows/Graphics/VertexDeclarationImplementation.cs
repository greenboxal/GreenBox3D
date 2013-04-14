// VertexDeclarationImplementation.cs
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
    public class VertexDeclarationImplementation
    {
        private static VertexDeclarationImplementation _lastUsed;
        private static IntPtr _lastUsedPointer;
        private static IShader _lastUsedShader;
        private readonly GLVertexElement[] _elements;

        private readonly WindowsGraphicsDevice _graphicsDevice;
        private readonly VertexDeclaration _vertexDeclaration;

        public VertexDeclarationImplementation(WindowsGraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration)
        {
            VertexElement[] elements = vertexDeclaration.GetVertexElements();

            _graphicsDevice = graphicsDevice;
            _vertexDeclaration = vertexDeclaration;

            _elements = new GLVertexElement[elements.Length];
            for (int i = 0; i < _elements.Length; i++)
                _elements[i] = new GLVertexElement(elements[i]);
        }

        public void Bind(IntPtr baseAddress)
        {
            if (_lastUsed == this && _lastUsedPointer == baseAddress && _lastUsedShader == _graphicsDevice.ActiveShader)
                return;

            VertexElement[] elements = _vertexDeclaration.GetVertexElements();

            for (int i = 0; i < _graphicsDevice.ActiveShader.Input.Length; i++)
            {
                VertexElement element = null;
                GLVertexElement gle = null;
                ShaderInput input = _graphicsDevice.ActiveShader.Input[i];

                for (int j = 0; j < elements.Length; j++)
                {
                    if (elements[j].VertexElementUsage == input.Usage && elements[j].UsageIndex == input.UsageIndex)
                    {
                        element = elements[j];
                        gle = _elements[j];
                        break;
                    }
                }

                if (element != null)
                {
                    GL.EnableVertexAttribArray(input.Index);
                    GL.VertexAttribPointer(input.Index, gle.ElementCount, gle.VertexAttribPointerType, !gle.IsNormalized,
                                           _vertexDeclaration.VertexStride, baseAddress + element.Offset);
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
