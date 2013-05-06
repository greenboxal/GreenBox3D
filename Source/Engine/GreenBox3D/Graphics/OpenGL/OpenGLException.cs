// OpenGLException.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public class OpenGLException : Exception
    {
        #region Constructors and Destructors

        public OpenGLException()
        {
            Error = GL.GetError();
        }

        public OpenGLException(ErrorCode error)
        {
            Error = error;
        }

        #endregion

        #region Public Properties

        public ErrorCode Error { get; private set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            // TODO: Better information?
            return Error.ToString();
        }

        #endregion
    }
}
