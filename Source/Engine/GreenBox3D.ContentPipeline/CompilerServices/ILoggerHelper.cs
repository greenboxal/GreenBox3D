using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.CompilerServices
{
    public interface ILoggerHelper
    {
        void Log(MessageLevel level, string errorCode, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);
        void Log(MessageLevel level, Exception exception, bool showStacktrace = false, bool showDetail = false, string file = null);
    }

    public enum MessageLevel
    {
        None,
        Warning,
        Error
    }
}
