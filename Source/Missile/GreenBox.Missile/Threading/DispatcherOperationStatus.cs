using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenBox.Missile.Threading
{
    public enum DispatcherOperationStatus
    {
        Pending,
        Aborted,
        Completed,
        Executing,
    }
}
