// GreenBox3D
// 
// Copyright (c) 2013 The GreenBox Development Inc.
// Copyright (c) 2013 Mono.Xna Team and Contributors
// 
// Licensed under MIT license terms.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
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