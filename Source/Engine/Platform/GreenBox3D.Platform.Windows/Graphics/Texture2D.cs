using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class Texture2D : Texture, ITexture2D
    {
        private int _level;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public override int LevelCount { get { return _level; } }

        public Texture2D(GraphicsDevice graphicsDevice, SurfaceFormat format, int width, int height)
            : base(graphicsDevice, format, TextureTarget.Texture2D)
        {
            Width = width;
            Height = height;
        }

        public void SetData<T>(T[] data) where T : struct
        {
            SetData(0, null, data, 0);
        }

        public void SetData<T>(int level, T[] data, int startIndex) where T : struct
        {
            SetData(level, null, data, startIndex);
        }

        public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex) where T : struct
        {
            if (level > _level)
                throw new InvalidOperationException("This Texture2D doesn't have " + level + " mipmap level.");

            bool parcial = rect.HasValue && (rect.Value.X != 0 || rect.Value.Y != 0 || rect.Value.Width != Width || rect.Value.Height != Height);
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            SetLastUnitDirty();

            if (parcial)
                GL.TexSubImage2D(TextureTarget.Texture2D, level, rect.Value.X, rect.Value.Y, rect.Value.Width, rect.Value.Height, PixelFormat, PixelType, Marshal.UnsafeAddrOfPinnedArrayElement(data, startIndex));
            else
                GL.TexImage2D(TextureTarget.Texture2D, level, InternalFormat, Width, Height, 0, PixelFormat, PixelType, Marshal.UnsafeAddrOfPinnedArrayElement(data, startIndex));

            if (level == _level)
                _level++;

            handle.Free();
        }
    }
}
