// VertexPositionTexture.cs
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
    public struct VertexPositionTexture : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new VertexDeclaration(
            new[]
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate),
            });

        public Vector3 Position;
        public Vector2 TextureCoord;

        public VertexPositionTexture(Vector3 position, Vector2 texCoord)
        {
            Position = position;
            TextureCoord = texCoord;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return Declaration; }
        }
    }
}
