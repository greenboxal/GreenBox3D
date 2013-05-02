using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;

namespace GreenBox3D.Content.Readers
{
    [ContentTypeReader(Extension = ".font")]
    public class FontTypeReader : ContentTypeReader<Font>
    {
        public FontTypeReader()
        {
            Magic = "F0NT";
            Version = new Version(1, 0);
        }

        protected override Font Load(ContentManager manager, ContentReader reader)
        {
            string name = reader.ReadString();
            int height = reader.ReadInt32();
            int pageCount = reader.ReadInt32();
            int glyphCount = reader.ReadInt32();
            Font font = new Font(name, height);

            for (int i = 0; i < pageCount; i++)
                font.Pages.Add(reader.ReadRawObject<ITexture2D>());

            for (int i = 0; i < glyphCount; i++)
            {
                char c = reader.ReadChar();
                FontGlyph glyph = new FontGlyph();

                glyph.Page = reader.ReadInt32();
                glyph.PageBounds = reader.ReadRectangle();
                glyph.Offset = reader.ReadPoint();
                glyph.Advance = reader.ReadInt32();

                int count = reader.ReadInt32();

                for (int j = 0; j < count; j++)
                    glyph.Kerning.Add(reader.ReadChar(), reader.ReadInt32());

                font.Glyphs.Add(c, glyph);
            }

            return font;
        }
    }
}
