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
            Parameters = new VariableCollection();
            Globals = new VariableCollection();
            Passes = new PassCollection();
        }

        public string Name { get; set; }
        public int Version { get; set; }
        public string Fallback { get; set; }
        public InputVariableCollection Input { get; private set; }
        public VariableCollection Parameters { get; private set; }
        public VariableCollection Globals { get; private set; }
        public PassCollection Passes { get; private set; }
    }
}
