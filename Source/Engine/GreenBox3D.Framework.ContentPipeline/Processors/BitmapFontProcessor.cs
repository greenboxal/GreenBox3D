using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using GreenBox3D.ContentPipeline.CompilerServices;
using GreenBox3D.ContentPipeline.Graphics;

namespace GreenBox3D.ContentPipeline.Processors
{
    [ContentProcessor("FontTypeWriter", DisplayName = "Bitmap Font Processor")]
    public class BitmapFontProcessor : ContentProcessor<XmlDocument, FontDescriptorSource>
    {
        protected override FontDescriptorSource Process(XmlDocument input, ContentProcessorContext context)
        {
            string name;
            int height;

            XmlNode info = input.SelectSingleNode("font/info");

            name = info.Attributes["face"].Value;
            height = int.Parse(info.Attributes["size"].Value);

            FontDescriptorSource source = new FontDescriptorSource(name, height);

            foreach (XmlNode page in input.SelectNodes("font/pages/page"))
                source.Pages.Add(
                    (Texture2DContent)
                    context.BuildAndLoadAsset<TextureContent>(context.ResolveRelativePath(page.Attributes["file"].Value)));

            foreach (XmlNode chr in input.SelectNodes("font/chars/char"))
            {
                FontGlyph glyph = new FontGlyph();
                string id = chr.Attributes["id"].Value;

                glyph.PageBounds =
                    new Rectangle(int.Parse(chr.Attributes["x"].Value),
                                  int.Parse(chr.Attributes["y"].Value),
                                  int.Parse(chr.Attributes["width"].Value),
                                  int.Parse(chr.Attributes["height"].Value));

                glyph.Offset = new Point(int.Parse(chr.Attributes["xoffset"].Value),
                                         int.Parse(chr.Attributes["yoffset"].Value));

                glyph.Advance = int.Parse(chr.Attributes["xadvance"].Value);
                glyph.Page = int.Parse(chr.Attributes["page"].Value);

                foreach (XmlNode kernel in input.SelectNodes("font/kernings/kerning[@first=" + id + "]"))
                    glyph.Kerning[(char)int.Parse(kernel.Attributes["second"].Value)] =
                        int.Parse(kernel.Attributes["amount"].Value);

                source.Glyphs[(char)int.Parse(id)] = glyph;
            }

            return source;
        }
    }
}
