// KeyModifiers.cs
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

namespace GreenBox3D.Input
{
    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Shift = 1,
        Control = 2,
        Capital = 4,
        Alt = 8,
    }
}
