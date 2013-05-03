using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public abstract class GraphicOperation : IDisposable
    {
        public abstract void Render();
        public abstract void Dispose();
    }
}
