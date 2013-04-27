using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.Graphics
{
    public abstract class TextureContent
    {
        public TextureFaceCollection Faces { get; private set; }

        protected TextureContent()
        {
            Faces = new TextureFaceCollection();
        }
    }
}
