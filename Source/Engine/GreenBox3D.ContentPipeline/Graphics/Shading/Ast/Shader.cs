// Shader.cs
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

namespace GreenBox3D.ContentPipeline.Graphics.Shading.Ast
{
    public class Shader
    {
        public Shader()
        {
            Version = 210;
            Input = new InputVariableCollection();
        }

        public string Name { get; set; }
        public int Version { get; set; }
        public string Fallback { get; set; }
        public InputVariableCollection Input { get; private set; }

        public string GlslVertexCode { get; set; }
        public string GlslPixelCode { get; set; }
        public string HlslVertexCode { get; set; }
        public string HlslPixelCode { get; set; }
    }
}
