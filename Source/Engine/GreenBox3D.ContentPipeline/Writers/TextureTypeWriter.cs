using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Content;
using GreenBox3D.ContentPipeline.Graphics;

namespace GreenBox3D.ContentPipeline.Writers
{
    [ContentTypeWriter(Extension = ".tex")]
    public class TextureTypeWriter : ContentTypeWriter<TextureContent>
    {
        protected override ContentHeader GetHeader(TextureContent input)
        {
            string magic = "TEX";

            if (input.GetType() == typeof(Texture2DContent))
                magic = "TEX2D";

            return new ContentHeader(magic, new Version(1, 0));
        }

        protected override void Write(ContentWriter stream, TextureContent input)
        {
            
        }
    }
}
