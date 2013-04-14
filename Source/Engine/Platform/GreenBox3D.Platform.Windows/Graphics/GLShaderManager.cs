// GLShaderManager.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using GreenBox3D.Platform.Windows.Graphics.Shading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GLShaderManager : ShaderManager
    {
        private readonly GraphicsDevice _graphicsDevice;

        public GLShaderManager(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public override IShader CreateShader(string name, int version, string fallback, ShaderInput[] input,
                                             ShaderParameter[] parameters, IShaderPass[] passes)
        {
            return new GLShader(_graphicsDevice, name, version, fallback, input, parameters, passes);
        }

        public override IShaderPass CreatePass(string vertexCode, string pixelCode)
        {
            return new GLShaderPass(_graphicsDevice, vertexCode, pixelCode);
        }

        public override IEffect CreateEffect(IShader shader)
        {
            return new GLEffect(shader as GLShader);
        }
    }
}
