using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;

namespace GreenBox3D
{
    public interface IGraphicsDeviceManager : IDisposable
    {
        GraphicsDevice GraphicsDevice { get; }
    }
}
