using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D
{
    public interface IConsoleCommand
    {
        void Execute(string arguments);
    }
}
