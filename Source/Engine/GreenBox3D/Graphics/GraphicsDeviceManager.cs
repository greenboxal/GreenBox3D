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
using GreenBox3D.Platform;

namespace GreenBox3D.Graphics
{
    public sealed class GraphicsDeviceManager : IInternalGraphicsDeviceManager
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly IInternalGameWindow _window;

        public GraphicsDeviceManager(Game owner, PresentationParameters parameters, IInternalGameWindow window)
        {
            _window = window;

            _graphicsDevice = new GraphicsDevice(owner, parameters, window);
            _graphicsDevice.MakeCurrent();

            (this as IInternalGraphicsDeviceManager).Update();
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        public void Dispose()
        {
            _graphicsDevice.Dispose();
        }

        void IInternalGraphicsDeviceManager.Update()
        {
            _graphicsDevice.MainContext.Update(_window.WindowInfo);
            _graphicsDevice.SetPresentationParameters(new PresentationParameters
            {
                BackBufferWidth = _window.ClientBounds.Width,
                BackBufferHeight = _window.ClientBounds.Height,
                DepthStencilFormat = _graphicsDevice.PresentationParameters.DepthStencilFormat,
                IsFullScreen = _graphicsDevice.PresentationParameters.IsFullScreen,
                MultiSampleCount = _graphicsDevice.PresentationParameters.MultiSampleCount
            });
        }
    }
}
