using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenBox.Missile.Threading
{
    public delegate void DispatcherUnhandledExceptionFilterEventHandler(object sender, DispatcherUnhandledExceptionFilterEventArgs e);

    public sealed class DispatcherUnhandledExceptionFilterEventArgs : DispatcherEventArgs
    {
        private bool _requestCatch;
        public Exception Exception { get; private set; }

        public bool RequestCatch
        {
            get
            {
                return _requestCatch;
            }
            set
            {
                if (value)
                    return;

                _requestCatch = false;
            }
        }

        internal DispatcherUnhandledExceptionFilterEventArgs(Dispatcher dispatcher, Exception exception)
            : base(dispatcher)
        {
            Exception = exception;
            _requestCatch = true;
        }
    }
}
