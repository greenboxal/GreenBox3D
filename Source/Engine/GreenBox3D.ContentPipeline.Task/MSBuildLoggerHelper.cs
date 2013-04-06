using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using GreenBox3D.ContentPipeline.CompilerServices;

namespace GreenBox3D.ContentPipeline.Task
{
    public class MSBuildLoggerHelper : ILoggerHelper
    {
        private readonly TaskLoggingHelper _helper;

        public MSBuildLoggerHelper(TaskLoggingHelper helper)
        {
            _helper = helper;
        }

        public void Log(MessageLevel level, string errorCode, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
        {
            switch (level)
            {
                case MessageLevel.None:
                    _helper.LogMessage(null, errorCode, null, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, MessageImportance.Normal, "{0}", string.Format(message, messageArgs));
                    break;
                case MessageLevel.Warning:
                    _helper.LogWarning(null, errorCode, null, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, "{0}", string.Format(message, messageArgs));
                    break;
                case MessageLevel.Error:
                    _helper.LogError(null, errorCode, null, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, "{0}", string.Format(message, messageArgs));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        // FIXME: Take care with this, if any custom exception is passed down through our isolated AppDomain the assembly will be locked down by devenv or msbuild process
        // This was already fixed on the normal Log() by formating the message on our AppDomain
        public void Log(MessageLevel level, Exception exception, bool showStacktrace = false, bool showDetail = false, string file = null)
        {
            switch (level)
            {
                case MessageLevel.Warning:
                    _helper.LogWarningFromException(exception, showStacktrace);
                    break;
                case MessageLevel.Error:
                    _helper.LogErrorFromException(exception, showStacktrace, showDetail, file);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
