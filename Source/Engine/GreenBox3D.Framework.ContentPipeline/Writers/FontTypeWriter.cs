using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Content;
using GreenBox3D.ContentPipeline.Graphics;

namespace GreenBox3D.ContentPipeline.Writers
{
    [ContentTypeWriter(Extension = ".font")]
    public class FontTypeWriter : ContentTypeWriter<FontDescriptorSource>
    {
        protected override ContentHeader GetHeader(FontDescriptorSource input)
        {
            return new ContentHeader("F0NT", new Version(1, 0));
        }

        protected override void Write(ContentWriter stream, FontDescriptorSource input)
        {
            stream.Write(input.Name);
            stream.Write(input.Height);
            stream.Write(input.Pages.Count);
            stream.Write(input.Glyphs.Count);

            foreach (Texture2DContent page in input.Pages)
                stream.WriteRawObject<TextureContent>(page);

            foreach (KeyValuePair<char, FontGlyph> kvp in input.Glyphs)
            {
                FontGlyph glyph = kvp.Value;

                stream.Write(kvp.Key);
                stream.Write(glyph.Page);
                stream.Write(glyph.PageBounds);
                stream.Write(glyph.Offset);
                stream.Write(glyph.Advance);
                stream.Write(glyph.Kerning.Count);

                foreach (KeyValuePair<char, int> kern in glyph.Kerning)
                {
                    stream.Write(kern.Key);
                    stream.Write(kern.Value);
                }
            }
        }
    }
}
