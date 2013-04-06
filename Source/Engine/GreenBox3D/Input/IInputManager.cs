using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Input
{
    public interface IInputManager
    {
        void RegisterMouseFilter(IMouseFilter filter);
        void UnregisterMouseFilter(IMouseFilter filter);
        void RegisterKeyboardFilter(IKeyboardFilter filter);
        void UnregisterKeyboardFilter(IKeyboardFilter filter);
    }
}
