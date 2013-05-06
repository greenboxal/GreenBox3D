// Texture2D.cs
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
    public sealed class Texture2D : Texture
    {
        private int _level;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public override int LevelCount { get { return _level; } }

        public Texture2D(SurfaceFormat format, int width, int height)
            : base(format, TextureTarget.Texture2D)
        {
            Width = width;
            Height = height;
        }

        public void SetData<T>(T[] data, PixelFormat format, PixelType type) where T : struct
        {
            SetData(0, null, data, 0, format, type);
        }

        public void SetData<T>(int level, T[] data, int startIndex, PixelFormat format, PixelType type) where T : struct
        {
            SetData(level, null, data, startIndex, format, type);
        }

        public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, PixelFormat format, PixelType type) where T : struct
        {
            if (level > _level)
                throw new InvalidOperationException("This Texture2D doesn't have " + level + " mipmap level.");

            bool parcial = rect.HasValue && (rect.Value.X != 0 || rect.Value.Y != 0 || rect.Value.Width != Width || rect.Value.Height != Height);
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            SetLastUnitDirty();

            if (parcial)
                GL.TexSubImage2D(TextureTarget.Texture2D, level, rect.Value.X, rect.Value.Y, rect.Value.Width, rect.Value.Height, GLUtils.GetPixelFormat(format), GLUtils.GetPixelType(type), Marshal.UnsafeAddrOfPinnedArrayElement(data, startIndex));
            else
                GL.TexImage2D(TextureTarget.Texture2D, level, InternalFormat, Width, Height, 0, GLUtils.GetPixelFormat(format), GLUtils.GetPixelType(type), Marshal.UnsafeAddrOfPinnedArrayElement(data, startIndex));

            if (level == _level)
                _level++;

            handle.Free();
        }

        public void SetData(int level, Rectangle? rect, PixelBuffer buffer, int startIndex, PixelFormat format, PixelType type)
        {
            if (level > _level)
                throw new InvalidOperationException("This Texture2D doesn't have " + level + " mipmap level.");

            bool parcial = rect.HasValue && (rect.Value.X != 0 || rect.Value.Y != 0 || rect.Value.Width != Width || rect.Value.Height != Height);

            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            SetLastUnitDirty();

            buffer.Bind();

            if (parcial)
                GL.TexSubImage2D(TextureTarget.Texture2D, level, rect.Value.X, rect.Value.Y, rect.Value.Width, rect.Value.Height, GLUtils.GetPixelFormat(format), GLUtils.GetPixelType(type), (IntPtr)startIndex);
            else
                GL.TexImage2D(TextureTarget.Texture2D, level, InternalFormat, Width, Height, 0, GLUtils.GetPixelFormat(format), GLUtils.GetPixelType(type), (IntPtr)startIndex);

            buffer.Unbind();

            if (level == _level)
                _level++;
        }
    }
}

#endif
