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

namespace GreenBox3D.Graphics
{
    public class VertexElement
    {
        #region Constructors and Destructors

        public VertexElement(int offset, VertexElementFormat elementFormat, VertexElementUsage elementUsage,
                             int usageIndex = 0)
        {
            Offset = offset;
            VertexElementFormat = elementFormat;
            VertexElementUsage = elementUsage;
            UsageIndex = usageIndex;

            switch (elementFormat)
            {
                case VertexElementFormat.Single:
                    SizeInBytes = 4;
                    break;
                case VertexElementFormat.Vector2:
                    SizeInBytes = 8;
                    break;
                case VertexElementFormat.Vector3:
                    SizeInBytes = 12;
                    break;
                case VertexElementFormat.Vector4:
                    SizeInBytes = 16;
                    break;
                case VertexElementFormat.Color:
                    SizeInBytes = 4;
                    break;
                case VertexElementFormat.Byte4:
                    SizeInBytes = 4;
                    break;
                case VertexElementFormat.Short2:
                    SizeInBytes = 4;
                    break;
                case VertexElementFormat.Short4:
                    SizeInBytes = 8;
                    break;
                case VertexElementFormat.NormalizedShort2:
                    SizeInBytes = 4;
                    break;
                case VertexElementFormat.NormalizedShort4:
                    SizeInBytes = 8;
                    break;
                case VertexElementFormat.HalfVector2:
                    SizeInBytes = 4;
                    break;
                case VertexElementFormat.HalfVector4:
                    SizeInBytes = 8;
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
