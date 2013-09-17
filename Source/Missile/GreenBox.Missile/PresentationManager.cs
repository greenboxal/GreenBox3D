using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox.Missile.Media;
using GreenBox.Missile.Media.Renderer;

namespace GreenBox.Missile
{
    public  static class PresentationManager
    {
        public static IRenderer Renderer { get; private set; }

        public static void Initialize(IRenderer renderer)
        {
            Renderer = renderer;
        }
    }
}
