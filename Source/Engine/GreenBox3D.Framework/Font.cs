using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;

namespace GreenBox3D
{
    public class Font
    {
        public Font(string name, int height)
        {
            Name = name;
            Height = height;
            Pages = new Collection<Texture2D>();
            Glyphs = new Dictionary<char, FontGlyph>();
        }

        public string Name { get; private set; }
        public int Height { get; private set; }
        public Collection<Texture2D> Pages { get; private set; }
        public Dictionary<char, FontGlyph> Glyphs { get; private set; }

        public FontGlyph GetGlyph(char c)
        {
            FontGlyph glyph;
            Glyphs.TryGetValue(c, out glyph);
            return glyph;
        }
    }
}
