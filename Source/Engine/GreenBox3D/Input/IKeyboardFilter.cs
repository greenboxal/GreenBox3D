using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Input
{
    public interface IKeyboardFilter
    {
        bool OnKeyDown(Keys key, KeyModifiers mod);
        bool OnKeyUp(Keys key, KeyModifiers mod);
        bool OnKeyChar(char unicode);
    }
}
