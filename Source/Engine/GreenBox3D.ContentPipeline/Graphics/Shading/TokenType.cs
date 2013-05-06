// TokenType.cs
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
    public enum TokenType
    {
        None,
        Error,

        Shader,
        Version,
        Input,
        Parameters,
        Globals,
        Passes,
        Pass,
        VertexGlsl,
        PixelGlsl,
        VertexHlsl,
        PixelHlsl,
        Fallback,

        Identifier,
        Number,
        String,

        LeftBrackets,
        RightBrackets,
        LeftSquare,
        RightSquare,
        Semicolon,
        Colon,

        Eof
    }
}
