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
    public static class LogManager
    {
        #region Static Fields

        private static readonly List<ILogRouter> Routers;
        private static Type _loggerType;

        #endregion

        #region Constructors and Destructors

        static LogManager()
        {
            Routers = new List<ILogRouter>();
            _loggerType = typeof(DefaultLogger);

            RegisterRouter(new ConsoleLogRouter());
        }

        #endregion

        #region Public Methods and Operators

        public static void Error(string format, params object[] args)
        {
            Log(LogLevel.Error, string.Format(format, args));
        }

        public static void ErrorEx(string message, Exception exception)
        {
            LogEx(LogLevel.Error, message, exception);
        }

        public static ILogger GetLogger(Type type, string customName = null)
        {
            return (ILogger)Activator.CreateInstance(_loggerType, type, customName);
        }

        public static void Log(LogLevel level, string text)
        {
            foreach (ILogRouter router in Routers)
                router.Output(level, text);
        }

        public static void LogEx(LogLevel level, string text, Exception exception)
        {
            foreach (ILogRouter router in Routers)
                router.OutputEx(level, text, exception);
        }

        public static void Message(string format, params object[] args)
        {
            Log(LogLevel.Message, string.Format(format, args));
        }

        public static void MessageEx(string message, Exception exception)
        {
            LogEx(LogLevel.Message, message, exception);
        }

        public static void RegisterRouter(ILogRouter router)
        {
            Routers.Add(router);
        }

        public static void SetLogger(Type type)
        {
            _loggerType = type;
        }

        public static void Warning(string format, params object[] args)
        {
            Log(LogLevel.Warning, string.Format(format, args));
        }

        public static void WarningEx(string message, Exception exception)
        {
            LogEx(LogLevel.Warning, message, exception);
        }

        #endregion
    }
}