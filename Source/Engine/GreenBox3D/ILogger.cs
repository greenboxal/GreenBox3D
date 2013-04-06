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
using System.Threading.Tasks;

namespace GreenBox3D
{
    public interface ILogger
    {
        #region Public Methods and Operators

        void Error(string format, params object[] args);
        void ErrorEx(string text, Exception exception);
        void Message(string format, params object[] args);
        void MessageEx(string text, Exception exception);
        void Warning(string format, params object[] args);
        void WarningEx(string text, Exception exception);

        #endregion
    }
}