// Texture.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
#if DESKTOP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public abstract class Texture : GraphicsResource
    {
        protected readonly PixelInternalFormat InternalFormat;

        public abstract int LevelCount { get; }
        public SurfaceFormat Format { get; private set; }

        public int TextureID { get; protected set; }
        public TextureTarget TextureTarget { get; protected set; }

        internal protected Texture(SurfaceFormat format, TextureTarget target)
        {
            Format = format;
            TextureTarget = target;
            TextureID = GL.GenTexture();
            InternalFormat = GLUtils.GetPixelInternalFormat(format);
        }

        internal protected void SetLastUnitDirty()
        {
            GraphicsDevice.Textures.SetLastUnitDirty();
        }

        protected override void Dispose(bool disposing)
        {
            if (TextureID != -1)
            {
                GL.DeleteTexture(TextureID);
                TextureID = -1;
            }
        }
    }
}

#endif
