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
    public class CompiledInputVariable : CompiledVariable
    {
        public CompiledInputVariable(string name, EffectParameterClass parameterClass, EffectParameterType parameterType,
                                     int count, int rowCount, int columnCount, VertexElementUsage usage, int usageIndex)
            : base(name, parameterClass, parameterType, count, rowCount, columnCount)
        {
            Usage = usage;
            UsageIndex = usageIndex;
        }

        public VertexElementUsage Usage { get; private set; }
        public int UsageIndex { get; private set; }
    }
}
