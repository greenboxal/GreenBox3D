using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Platform
{
    public interface IGameWindow
    {
        string Title { get; set; }
        Point Position { get; set; }
        Point Size { get; }
        bool ShowCursor { get; set; }
        bool Resizable { get; set; }

        void Resize(int width, int height);
    }
}
