// Font.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
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

        public Vector2 MeasureString(string text)
        {
            int x = 0;

            for (int i = 0; i < text.Length; i++)
            {
                FontGlyph glyph;
                int offset;

                glyph = GetGlyph(text[i]);

                if (glyph == null)
                {
                    glyph = GetGlyph((char)0);

                    if (glyph == null)
                        continue;
                }

                x += glyph.Advance;

                if (i < text.Length - 1 && glyph.Kerning.TryGetValue(text[i + 1], out offset))
                    x += offset;
            }

            return new Vector2(x, Height);
        }
    }
}
