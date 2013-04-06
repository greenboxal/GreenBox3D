using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Input
{
    public interface IMouseFilter
    {
        bool OnMouseDown(MouseButton button, int x, int y);
        bool OnMouseUp(MouseButton button, int x, int y);
        bool OnMouseMove(int x, int y);
        bool OnMouseWheel(int delta, int x, int y);
    }
}
