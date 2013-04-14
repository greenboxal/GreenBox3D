// GraphicsDeviceManager.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

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
        private readonly WindowsGraphicsDevice _graphicsDevice;
        private readonly WindowsGamePlatform _platform;
        private readonly WindowsGameWindow _window;

        public GraphicsDeviceManager(WindowsGamePlatform platform, PresentationParameters parameters,
                                     WindowsGameWindow window)
        {
            _platform = platform;
            _window = window;

            _graphicsDevice = new WindowsGraphicsDevice(platform, parameters, window);
            _graphicsDevice.MakeCurrent();
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        public void Dispose()
        {
            _graphicsDevice.Dispose();
        }
    }
}
