using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public interface IGraphicsResource : IDisposable
    {
        GraphicsDevice GraphicsDevice { get; }
        bool IsDisposed { get; }

        event EventHandler<EventArgs> Disposing;
    }
}
