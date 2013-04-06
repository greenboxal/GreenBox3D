using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GreenBox3D.Graphics;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GraphicsDeviceManager : IGraphicsDeviceManager
    {
        private readonly WindowsGamePlatform _platform;
        private readonly WindowsGameWindow _window;
        private readonly WindowsGraphicsDevice _graphicsDevice;

        public GraphicsDevice GraphicsDevice { get { return _graphicsDevice; } }

        public GraphicsDeviceManager(WindowsGamePlatform platform, PresentationParameters parameters, WindowsGameWindow window)
        {
            _platform = platform;
            _window = window;

            _graphicsDevice = new WindowsGraphicsDevice(platform, parameters, window);
            _graphicsDevice.MakeCurrent();
        }

        public void Dispose()
        {
            _graphicsDevice.Dispose();
        }
    }
}
