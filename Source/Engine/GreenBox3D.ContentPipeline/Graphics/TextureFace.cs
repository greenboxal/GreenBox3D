using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.Graphics
{
    public class TextureFace : Collection<BitmapContent>
    {
        public static implicit operator TextureFace(BitmapContent bmp)
        {
            return new TextureFace { bmp };
        }
    }
}
