// RenderTarget2D.cs
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
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public class RenderTarget2D : Texture2D
    {
        internal int DepthStencilBuffer;

        public DepthFormat DepthStencilFormat { get; private set; }

        public RenderTarget2D(int width, int height, SurfaceFormat format, DepthFormat depthFormat)
            : base(format, width, height)
        {
            DepthStencilFormat = depthFormat;

            if (depthFormat == DepthFormat.None)
                return;

            RenderbufferStorage depthStorage;
            RenderbufferStorage stencilStorage;

            GLUtils.GetRenderBufferDepthStorage(depthFormat, out depthStorage, out stencilStorage);
            //GL.BindFramebuffer();

        }
    }
}

#endif
