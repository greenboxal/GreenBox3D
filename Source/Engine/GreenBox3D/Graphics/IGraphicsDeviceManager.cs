using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public interface IGraphicsDeviceManager : IDisposable
    {
        GraphicsDevice GraphicsDevice { get; }
    }
}
