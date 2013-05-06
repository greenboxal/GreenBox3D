// Token.cs
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

namespace GreenBox3D.ContentPipeline.Graphics.Shading
{
    public class Token
    {
        public int Column;
        public int EndColumn;
        public int EndLine;
        public string File;
        public int Line;
        public string Text;
        public TokenType Type;
    }
}
