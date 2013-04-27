using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.Graphics
{
    public class Texture2DContent : TextureContent
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Texture2DContent(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
