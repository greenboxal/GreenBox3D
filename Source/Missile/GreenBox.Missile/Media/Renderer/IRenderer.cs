using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox.Missile.Media.Renderer
{
    public interface IRenderer
    {
        IVisualHandle CreateVisualHandle(Visual visual);
    }
}
