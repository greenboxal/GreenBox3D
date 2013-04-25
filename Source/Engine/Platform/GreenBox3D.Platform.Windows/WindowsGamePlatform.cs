// WindowsGamePlatform.cs
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
using GreenBox3D.Input;
using GreenBox3D.Platform.Windows.Graphics;
using GreenBox3D.Platform.Windows.Input;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows
{
    public class WindowsGamePlatform : GamePlatform
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(WindowsGamePlatform));
        private bool _doPostDispose;

        private WindowsGameWindow _gameWindow;
        private GraphicsDeviceManager _graphicsDeviceManager;
        private InputManager _inputManager;
        private bool _running;
        private bool _skipFrame;

        public WindowsGamePlatform(IPlatformController controller)
            : base(controller)
        {
        }

        public override bool Running
        {
            get { return _running; }
        }

        public override IGameWindow Window
        {
            get { return _gameWindow; }
        }

        public override bool VSync
        {
            get { return ((WindowsGraphicsDevice)_graphicsDeviceManager.GraphicsDevice).VSync; }
            set { ((WindowsGraphicsDevice)_graphicsDeviceManager.GraphicsDevice).VSync = value; }
        }

        public override void Run()
        {
            Controller.Initialize();

            _running = true;
            while (_running)
            {
                _gameWindow.HandleEvents();

                Controller.Update(null);

                if (_skipFrame)
                {
                    _skipFrame = false;
                }
                else
                {
                    Controller.Render(null);
                    _graphicsDeviceManager.GraphicsDevice.Present();
                }
            }

            Controller.Shutdown();

            if (_doPostDispose)
                Dispose();
        }

        public override void InitializeGraphics(PresentationParameters parameters)
        {
            Log.Message("Initializing Graphic subsystem");

            _gameWindow = new WindowsGameWindow(this, parameters);
            _gameWindow.Create();
            _graphicsDeviceManager = new GraphicsDeviceManager(this, parameters, _gameWindow);

            Controller.RegisterService(typeof(IGraphicsDeviceManager), _graphicsDeviceManager);
        }

        public override void InitializeInput()
        {
            Log.Message("Initializing Input subsystem");

            if (_gameWindow == null)
                throw new InvalidOperationException("Graphics subsystem must be initialized before the Input subsystem");

            _inputManager = new InputManager(this, _gameWindow);
            Mouse.SetMouseImplementation(_gameWindow);
            Keyboard.SetKeyboardImplementation(_gameWindow);

            Controller.RegisterService(typeof(IInputManager), _inputManager);
        }

        public override void SkipFrame()
        {
            _skipFrame = true;
        }

        public override void Exit()
        {
            _running = false;
        }

        public override void Dispose()
        {
            if (_running)
            {
                _doPostDispose = true;
                Exit();
                return;
            }

            if (_gameWindow != null)
                _gameWindow.Dispose();
        }

        public void WindowResized()
        {
            Controller.OnResize();
        }
    }
}
