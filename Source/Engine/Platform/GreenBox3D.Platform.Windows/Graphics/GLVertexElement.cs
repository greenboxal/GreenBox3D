// GLVertexElement.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using GreenBox3D.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GLVertexElement
    {
        public int ElementCount;
        public bool IsNormalized;
        public VertexAttribPointerType VertexAttribPointerType;

        public GLVertexElement(VertexElement element)
        {
            IsNormalized = true;

            switch (element.VertexElementFormat)
            {
                case VertexElementFormat.Single:
                    ElementCount = 1;
                    VertexAttribPointerType = VertexAttribPointerType.Float;
                    break;
                case VertexElementFormat.Vector2:
                    ElementCount = 2;
                    VertexAttribPointerType = VertexAttribPointerType.Float;
                    break;
                case VertexElementFormat.Vector3:
                    ElementCount = 3;
                    VertexAttribPointerType = VertexAttribPointerType.Float;
                    break;
                case VertexElementFormat.Vector4:
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.Float;
                    break;
                case VertexElementFormat.Color:
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.UnsignedByte;
                    IsNormalized = false;
                    break;
                case VertexElementFormat.Byte4:
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.Byte;
                    IsNormalized = false;
                    break;
                case VertexElementFormat.Short2:
                    ElementCount = 2;
                    VertexAttribPointerType = VertexAttribPointerType.Short;
                    IsNormalized = false;
                    break;
                case VertexElementFormat.Short4:
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.Short;
                    IsNormalized = false;
                    break;
                case VertexElementFormat.NormalizedShort2:
                    ElementCount = 2;
                    VertexAttribPointerType = VertexAttribPointerType.Short;
                    break;
                case VertexElementFormat.NormalizedShort4:
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.Short;
                    break;
                case VertexElementFormat.HalfVector2:
                    ElementCount = 2;
                    VertexAttribPointerType = VertexAttribPointerType.HalfFloat;
                    break;
                case VertexElementFormat.HalfVector4:
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.HalfFloat;
                    break;
            }
        }
    }
}
