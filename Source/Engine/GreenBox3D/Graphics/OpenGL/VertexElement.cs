// VertexElement.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public class VertexElement
    {
        internal int ElementCount;
        internal bool IsNormalized;
        internal VertexAttribPointerType VertexAttribPointerType;

        #region Constructors and Destructors

        public VertexElement(int offset, VertexElementFormat elementFormat, VertexElementUsage elementUsage,
                             int usageIndex = 0)
        {
            Offset = offset;
            VertexElementFormat = elementFormat;
            VertexElementUsage = elementUsage;
            UsageIndex = usageIndex;

            IsNormalized = true;

            switch (elementFormat)
            {
                case VertexElementFormat.Single:
                    SizeInBytes = 4;
                    ElementCount = 1;
                    VertexAttribPointerType = VertexAttribPointerType.Float;
                    break;
                case VertexElementFormat.Vector2:
                    SizeInBytes = 8;
                    ElementCount = 2;
                    VertexAttribPointerType = VertexAttribPointerType.Float;
                    break;
                case VertexElementFormat.Vector3:
                    SizeInBytes = 12;
                    ElementCount = 3;
                    VertexAttribPointerType = VertexAttribPointerType.Float;
                    break;
                case VertexElementFormat.Vector4:
                    SizeInBytes = 16;
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.Float;
                    break;
                case VertexElementFormat.Color:
                    SizeInBytes = 4;
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.UnsignedByte;
                    IsNormalized = false;
                    break;
                case VertexElementFormat.Byte4:
                    SizeInBytes = 4;
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.Byte;
                    IsNormalized = false;
                    break;
                case VertexElementFormat.Short2:
                    SizeInBytes = 4;
                    ElementCount = 2;
                    VertexAttribPointerType = VertexAttribPointerType.Short;
                    IsNormalized = false;
                    break;
                case VertexElementFormat.Short4:
                    SizeInBytes = 8;
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.Short;
                    IsNormalized = false;
                    break;
                case VertexElementFormat.NormalizedShort2:
                    SizeInBytes = 4;
                    ElementCount = 2;
                    VertexAttribPointerType = VertexAttribPointerType.Short;
                    break;
                case VertexElementFormat.NormalizedShort4:
                    SizeInBytes = 8;
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.Short;
                    break;
                case VertexElementFormat.HalfVector2:
                    SizeInBytes = 4;
                    ElementCount = 2;
                    VertexAttribPointerType = VertexAttribPointerType.HalfFloat;
                    break;
                case VertexElementFormat.HalfVector4:
                    SizeInBytes = 8;
                    ElementCount = 4;
                    VertexAttribPointerType = VertexAttribPointerType.HalfFloat;
                    break;
            }
        }

        #endregion

        #region Public Properties

        public int Offset { get; private set; }
        public int SizeInBytes { get; private set; }
        public int UsageIndex { get; private set; }
        public VertexElementFormat VertexElementFormat { get; private set; }
        public VertexElementUsage VertexElementUsage { get; private set; }

        #endregion
    }
}
