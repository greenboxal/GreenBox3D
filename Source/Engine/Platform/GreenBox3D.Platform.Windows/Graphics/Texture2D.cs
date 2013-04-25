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
        private bool _hasTexture;
        private int _level;

        public Texture2D(GraphicsDevice graphicsDevice, SurfaceFormat format, int width, int height)
            : base(graphicsDevice, format)
        {
            Target = TextureTarget.Texture2D;
            ;

            Width = width;
            Height = height;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public override int LevelCount
        {
            get { return _level; }
        }

        public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex) where T : struct
        {
            Create();

            if (LevelCount > level)
                throw new ArgumentException("Level can be higher than the actual LevelCount", "level");

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                if (rect.HasValue)
                    GL.TexSubImage2D(TextureTarget.Texture2D, level, rect.Value.X, rect.Value.Y, rect.Value.Width,
                                     rect.Value.Height, PixelFormat, PixelType,
                                     Marshal.UnsafeAddrOfPinnedArrayElement(data, startIndex));
                else
                    GL.TexImage2D(TextureTarget.Texture2D, level, InternalFormat, Width, Height, 0, PixelFormat,
                                  PixelType, Marshal.UnsafeAddrOfPinnedArrayElement(data, startIndex));
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }

            if (GL.GetError() != ErrorCode.NoError)
                throw new OpenGLException();

            if (level == _level)
                _level++;

            _hasTexture = true;
        }

        public void SetData<T>(T[] data) where T : struct
        {
            SetData(0, null, data, 0);
        }

        public void SetData<T>(int level, T[] data, int startIndex) where T : struct
        {
            SetData(level, null, data, startIndex);
        }

        protected override void Create(bool texImage = false)
        {
            if (TextureID == -1)
            {
                TextureID = GL.GenTexture();

                if (TextureID == -1)
                    throw new OpenGLException();
            }

            if (texImage && !_hasTexture)
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat, Width, Height, 0, PixelFormat, PixelType,
                              IntPtr.Zero);
                _hasTexture = true;
            }
        }
    }
}
