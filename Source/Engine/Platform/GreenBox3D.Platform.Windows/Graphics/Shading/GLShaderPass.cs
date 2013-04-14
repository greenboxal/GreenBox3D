// GLShaderPass.cs
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

namespace GreenBox3D.Platform.Windows.Graphics.Shading
{
    public class GLShaderPass : GraphicsResource, IShaderPass
    {
        private readonly string _pixelCode;
        private readonly string _vertexCode;
        private bool _isValid;
        private GLShader _owner;

        public GLShaderPass(GraphicsDevice graphicsDevice, string vertexCode, string pixelCode)
            : base(graphicsDevice)
        {
            _vertexCode = vertexCode;
            _pixelCode = pixelCode;
            _isValid = false;
        }

        public string VertexCode
        {
            get { return _vertexCode; }
        }

        public string PixelCode
        {
            get { return _pixelCode; }
        }

        public bool IsValid
        {
            get { return _isValid; }
        }

        public int ProgramID { get; set; }
        public int VertexShader { get; set; }
        public int PixelShader { get; set; }

        public void Create(GLShader owner)
        {
            _owner = owner;

            int status;

            if (string.IsNullOrEmpty(_vertexCode) || string.IsNullOrEmpty(_pixelCode))
                return;

            ProgramID = GL.CreateProgram();
            if (ProgramID == -1)
                throw new OpenGLException();

            VertexShader = GL.CreateShader(OpenTK.Graphics.OpenGL.ShaderType.VertexShader);
            if (VertexShader == -1)
                throw new OpenGLException();

            GL.ShaderSource(VertexShader, _vertexCode);
            GL.CompileShader(VertexShader);

            GL.GetShader(VertexShader, OpenTK.Graphics.OpenGL.ShaderParameter.CompileStatus, out status);
            if (status == 0)
                throw new Exception(GL.GetShaderInfoLog(VertexShader));

            PixelShader = GL.CreateShader(OpenTK.Graphics.OpenGL.ShaderType.FragmentShader);
            if (PixelShader == -1)
                throw new OpenGLException();

            GL.ShaderSource(PixelShader, _pixelCode);
            GL.CompileShader(PixelShader);

            GL.GetShader(PixelShader, OpenTK.Graphics.OpenGL.ShaderParameter.CompileStatus, out status);
            if (status == 0)
                throw new Exception(GL.GetShaderInfoLog(PixelShader));

            GL.AttachShader(ProgramID, VertexShader);
            GL.AttachShader(ProgramID, PixelShader);
            GL.LinkProgram(ProgramID);

            GL.GetProgram(ProgramID, ProgramParameter.LinkStatus, out status);
            if (status == 0)
                throw new Exception(GL.GetProgramInfoLog(ProgramID));

            _isValid = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                GL.DeleteProgram(ProgramID);
                GL.DeleteShader(VertexShader);
                GL.DeleteShader(PixelShader);
            }

            base.Dispose(disposing);
        }
    }
}
