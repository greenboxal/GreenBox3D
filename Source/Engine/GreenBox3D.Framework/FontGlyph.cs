// FontGlyph.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D
{
    public class FontGlyph
    {
        public FontGlyph()
        {
            Kerning = new Dictionary<char, int>();
        }

        public int Page { get; set; }
        public Rectangle PageBounds { get; set; }
        public Point Offset { get; set; }
        public int Advance { get; set; }
        public Dictionary<char, int> Kerning { get; private set; }
    }
}
