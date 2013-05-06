// ShaderInput.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
#if DESKTOP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    internal class ShaderInput
    {
        public ShaderInput(int programID, CompiledInputVariable variable)
        {
            Name = variable.Name;
            Usage = variable.Usage;
            UsageIndex = variable.UsageIndex;
            Index = GL.GetAttribLocation(programID, variable.Name);
        }

        public string Name { get; private set; }
        public VertexElementUsage Usage { get; private set; }
        public int UsageIndex { get; private set; }
        public int Index { get; private set; }
    }
}

#endif
