// GLShader.cs
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
using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using OpenTK.Graphics.OpenGL;
using ShaderParameter = GreenBox3D.Graphics.Detail.ShaderParameter;

namespace GreenBox3D.Platform.Windows.Graphics.Shading
{
    public class GLShader : GraphicsResource, IShader
    {
        private readonly string _fallback;
        private readonly ShaderInput[] _input;
        private readonly string _name;
        private readonly ShaderParameterCollection _parameters;
        private readonly ShaderPassCollection _passes;
        private readonly int _version;
        private bool _created;
        private bool _isValid;

        public GLShader(GraphicsDevice graphicsDevice, string name, int version, string fallback, ShaderInput[] input,
                        ShaderParameter[] parameters, IShaderPass[] passes)
            : base(graphicsDevice)
        {
            _name = name;
            _version = version;
            _fallback = fallback;
            _input = input;

            _parameters = new ShaderParameterCollection();
            _passes = new ShaderPassCollection();

            foreach (ShaderParameter parameter in parameters)
                _parameters.Add(new GLShaderParameter(parameter));

            foreach (IShaderPass pass in passes)
                _passes.Add(pass);
        }

        public string Name
        {
            get { return _name; }
        }

        public int Version
        {
            get { return _version; }
        }

        public string Fallback
        {
            get { return _fallback; }
        }

        public bool IsValid
        {
            get { return _isValid; }
        }

        public bool Created
        {
            get { return _created; }
        }

        public ShaderInput[] Input
        {
            get { return _input; }
        }

        public ShaderParameterCollection Parameters
        {
            get { return _parameters; }
        }

        public ShaderPassCollection Passes
        {
            get { return _passes; }
        }

        public int ParametersSize { get; private set; }

        public int GetInputIndex(VertexElementUsage usage, int usageIndex)
        {
            foreach (ShaderInput input in Input)
            {
                if (input.Usage == usage && input.UsageIndex == usageIndex)
                    return input.Index;
            }

            return -1;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                foreach (IShaderPass pass in _passes)
                    pass.Dispose();
            }

            base.Dispose(disposing);
        }

        public void Create()
        {
            _created = true;
            _isValid = true;

            foreach (GLShaderPass pass in Passes)
            {
                pass.Create(this);
                _isValid &= pass.IsValid;
            }

            if (_isValid)
            {
                int textureCounter = 0;

                foreach (GLShaderParameter parameter in Parameters)
                {
                    parameter.Offset = ParametersSize;
                    parameter.Slot = GL.GetUniformLocation((Passes[0] as GLShaderPass).ProgramID, "u" + parameter.Name);
                    ParametersSize += parameter.ByteSize;

                    if (parameter.Class == EffectParameterClass.Sampler)
                    {
                        int count = parameter.Count == 0 ? 1 : parameter.Count;
                        parameter.TextureUnit = textureCounter;

                        // TODO: reimpl
                        /*  if (textureCounter + count > GraphicsDevice.Textures.Count)
                          {
                              _isValid = false;
                              break;
                          }*/

                        textureCounter += count;
                    }
                }
            }
        }
    }
}
