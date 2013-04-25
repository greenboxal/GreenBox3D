using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public abstract class TextureManager
    {
        public abstract ITexture2D CreateTexture2D(SurfaceFormat format, int width, int height);
    }
}
