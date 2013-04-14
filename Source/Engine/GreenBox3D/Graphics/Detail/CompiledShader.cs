// CompiledShader.cs
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

namespace GreenBox3D.Graphics.Detail
{
    public class CompiledShader
    {
        public CompiledShader(string name, int version, string fallback)
        {
            Name = name;
            Version = version;
            Fallback = fallback;

            Input = new CompiledInputVariableCollection();
            Globals = new CompiledVariableCollection();
            Parameters = new CompiledVariableCollection();
            Passes = new CompiledPassCollection();
        }

        public string Name { get; private set; }
        public int Version { get; private set; }
        public string Fallback { get; private set; }
        public CompiledInputVariableCollection Input { get; private set; }
        public CompiledVariableCollection Globals { get; private set; }
        public CompiledVariableCollection Parameters { get; private set; }
        public CompiledPassCollection Passes { get; private set; }
    }
}
