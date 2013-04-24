// CompiledInputVariable.cs
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
    public class CompiledInputVariable
    {
        public CompiledInputVariable(string name, VertexElementUsage usage, int usageIndex)
        {
            Name = name;
            Usage = usage;
            UsageIndex = usageIndex;
        }

        public string Name { get; private set; }
        public VertexElementUsage Usage { get; private set; }
        public int UsageIndex { get; private set; }
    }
}
