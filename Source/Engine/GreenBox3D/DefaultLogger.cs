// DefaultLogger.cs
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

namespace GreenBox3D
{
    public class DefaultLogger : ILogger
    {
        #region Fields

        private readonly string _prefix;
        private readonly Type _type;

        #endregion

        #region Constructors and Destructors

        public DefaultLogger(Type type, string customName)
        {
            _type = type;
            _prefix = "<" + (customName ?? _type.FullName) + ">: ";
        }

        #endregion

        #region Public Methods and Operators

        public void Error(string format, params object[] args)
        {
            LogManager.Error(_prefix + format, args);
        }

        public void ErrorEx(string text, Exception exception)
        {
            LogManager.ErrorEx(_prefix + text, exception);
        }

        public void Message(string format, params object[] args)
        {
            LogManager.Message(_prefix + format, args);
        }

        public void MessageEx(string text, Exception exception)
        {
            LogManager.MessageEx(_prefix + text, exception);
        }

        public void Warning(string format, params object[] args)
        {
            LogManager.Warning(_prefix + format, args);
        }

        public void WarningEx(string text, Exception exception)
        {
            LogManager.WarningEx(_prefix + text, exception);
        }

        #endregion
    }
}
