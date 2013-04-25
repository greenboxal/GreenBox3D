using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GLTextureManager : TextureManager
    {
        private readonly WindowsGraphicsDevice _graphicsDevice;

        public GLTextureManager(WindowsGraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public override ITexture2D CreateTexture2D(SurfaceFormat format, int width, int height)
        {
            return new Texture2D(_graphicsDevice, format, width, height);
        }
    }
}
