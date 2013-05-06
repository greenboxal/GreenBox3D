// VertexPositionNormalColor.cs
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
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalColor : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new VertexDeclaration(
            new[]
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal),
                new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color),
            });

        public Vector3 Position;
        public Vector3 Normal;
        public Color Color;

        public VertexPositionNormalColor(Vector3 position, Vector3 normal, Color color)
        {
            Position = position;
            Normal = normal;
            Color = color;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return Declaration; }
        }
    }
}
