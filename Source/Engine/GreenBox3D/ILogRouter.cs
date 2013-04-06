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
    public interface ILogRouter
    {
        #region Public Methods and Operators

        void Output(LogLevel level, string text);
        void OutputEx(LogLevel level, string text, Exception exception);

        #endregion
    }

    internal class ConsoleLogRouter : ILogRouter
    {
        #region Public Methods and Operators

        public void Output(LogLevel level, string text)
        {
            switch (level)
            {
                case LogLevel.Error:
                    ConsoleManager.ErrorMessage("{0}");
                    break;
                case LogLevel.Warning:
                    ConsoleManager.WarningMessage("{0}");
                    break;
                case LogLevel.Message:
                    ConsoleManager.Message("{0}", text);
                    break;
            }
        }

        public void OutputEx(LogLevel level, string text, Exception exception)
        {
            switch (level)
            {
                case LogLevel.Error:
                    ConsoleManager.ErrorMessage("{0}\n{1}", text, exception.ToString());
                    break;
                case LogLevel.Warning:
                    ConsoleManager.WarningMessage("{0}\n{1}", text, exception.ToString());
                    break;
                case LogLevel.Message:
                    ConsoleManager.Message("{0}\n{1}", text, exception.ToString());
                    break;
            }
        }

        #endregion
    }
}