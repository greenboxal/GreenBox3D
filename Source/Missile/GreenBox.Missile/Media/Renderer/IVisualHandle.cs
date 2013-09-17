using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D;

namespace GreenBox.Missile.Media.Renderer
{
    public interface IVisualHandle : IDisposable
    {
        Visual Visual { get; }

        int ChildrenCount { get; }
        Vector Offset { get; set; }
        float Opacity { get; set; }

        void AddChild(IVisualHandle child);
        void RemoveChild(IVisualHandle child);
        IVisualHandle GetChild(int index);
    }
}
