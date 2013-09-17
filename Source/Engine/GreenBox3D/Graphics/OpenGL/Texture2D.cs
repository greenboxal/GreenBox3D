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
    public class Texture2D : Texture
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
            SetData(0, data, 0, format, type);
        }

        public void SetData<T>(int level, T[] data, int startIndex, PixelFormat format, PixelType type) where T : struct
        {
            if (level > _level)
                throw new InvalidOperationException("This Texture2D doesn't have " + level + " mipmap level.");

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.TexImage2D(TextureTarget.Texture2D, level, InternalFormat, Width, Height, 0, GLUtils.GetPixelFormat(format), GLUtils.GetPixelType(type), Marshal.UnsafeAddrOfPinnedArrayElement(data, startIndex));
            SetLastUnitDirty();

            if (level == _level)
                _level++;

            handle.Free();
        }

        public void SetData<T>(int level, Rectangle rect, T[] data, int startIndex, PixelFormat format, PixelType type) where T : struct
        {
            if (level > _level)
                throw new InvalidOperationException("This Texture2D doesn't have " + level + " mipmap level.");

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.TexSubImage2D(TextureTarget.Texture2D, level, rect.X, rect.Y, rect.Width, rect.Height, GLUtils.GetPixelFormat(format), GLUtils.GetPixelType(type), Marshal.UnsafeAddrOfPinnedArrayElement(data, startIndex));
            SetLastUnitDirty();

            handle.Free();
        }

        public void SetData(int level, PixelBuffer buffer, int startOffset, PixelFormat format, PixelType type)
        {
            if (level > _level)
                throw new InvalidOperationException("This Texture2D doesn't have " + level + " mipmap level.");

            buffer.Bind();

            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.TexImage2D(TextureTarget.Texture2D, level, InternalFormat, Width, Height, 0, GLUtils.GetPixelFormat(format), GLUtils.GetPixelType(type), (IntPtr)startOffset);
            SetLastUnitDirty();

            buffer.Unbind();

            if (level == _level)
                _level++;
        }

        public void SetData(int level, IntPtr buffer, PixelFormat format, PixelType type)
        {
            if (level > _level)
                throw new InvalidOperationException("This Texture2D doesn't have " + level + " mipmap level.");

            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.TexImage2D(TextureTarget.Texture2D, level, InternalFormat, Width, Height, 0, GLUtils.GetPixelFormat(format), GLUtils.GetPixelType(type), buffer);
            SetLastUnitDirty();

            if (level == _level)
                _level++;
        }

        public void SetData(int level, Rectangle rect, PixelBuffer buffer, int startIndex, PixelFormat format, PixelType type)
        {
            if (level > _level)
                throw new InvalidOperationException("This Texture2D doesn't have " + level + " mipmap level.");

            buffer.Bind();

            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.TexSubImage2D(TextureTarget.Texture2D, level, rect.X, rect.Y, rect.Width, rect.Height, GLUtils.GetPixelFormat(format), GLUtils.GetPixelType(type), (IntPtr)startIndex);
            SetLastUnitDirty();

            buffer.Unbind();
        }
    }
}

#endif
