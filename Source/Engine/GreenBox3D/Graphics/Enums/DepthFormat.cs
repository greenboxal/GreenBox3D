// DepthFormat.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;

namespace GreenBox3D.Graphics
{
    public enum DepthFormat
    {
        None = -1,
        Depth16 = 54,
        Depth24 = 51,
        Depth24Stencil8 = 48,
    }
}
