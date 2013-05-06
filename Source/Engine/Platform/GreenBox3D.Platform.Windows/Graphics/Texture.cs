using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public abstract class Texture : GraphicsResource, ITexture
    {
        protected readonly PixelInternalFormat InternalFormat;
        protected readonly PixelFormat PixelFormat;
        protected readonly PixelType PixelType;

        public abstract int LevelCount { get; }
        public SurfaceFormat Format { get; private set; }

        public int TextureID { get; protected set; }
        public TextureTarget TextureTarget { get; protected set; }

        protected Texture(GraphicsDevice graphicsDevice, SurfaceFormat format, TextureTarget target)
            : base(graphicsDevice)
        {
            Format = format;
            TextureTarget = target;
            TextureID = GL.GenTexture();
            GLUtils.GetOpenGLTextureFormat(format, out InternalFormat, out PixelFormat, out PixelType);
        }

        protected void SetLastUnitDirty()
        {
            (GraphicsDevice.Textures as GLTextureCollection).SetLastUnitDirty();
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
