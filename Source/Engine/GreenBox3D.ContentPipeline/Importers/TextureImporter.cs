using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.ContentPipeline.CompilerServices;
using GreenBox3D.ContentPipeline.Graphics;

namespace GreenBox3D.ContentPipeline.Importers
{
    [ContentImporter(".bmp", ".png", ".jpg", DisplayName = "Texture Importer", DefaultProcessor = "TextureProcessor")]
    public class TextureImporter : ContentImporter<TextureContent>
    {
        protected override TextureContent Import(string filename, ContentImporterContext context)
        {
            using (Bitmap bitmap = new Bitmap(filename))
            {
                Texture2DContent texture = new Texture2DContent(bitmap.Width, bitmap.Height);
                BitmapContent content = ImageHelper.BitmapContentFromBitmap(bitmap);

                if (content == null)
                    context.Logger.Log(MessageLevel.Error, null, filename, 0, 0, 0, 0,
                                       "Unsupported image pixel format");

                texture.Faces.Add(content);

                return texture;
            }
        }
    }
}
