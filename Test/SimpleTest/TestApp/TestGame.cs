// TestGame.cs
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
using GreenBox3D;
using GreenBox3D.Content;
using GreenBox3D.Graphics;
using GreenBox3D.Input;

namespace TestApp
{
    public class TestGame : Game
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(TestGame));
        private ContentManager _contentManager;

        private GraphicsDevice _graphicsDevice;
        private IInputManager _inputManager;

        private Font _font;
        private GraphicBatch _batch;

        private int _fpsAcc;
        private double _fps, _fpsCounter;

        public TestGame()
        {
            FileManager.RegisterLoader(new FolderFileLoader("Data"));
        }

        protected override void Initialize()
        {
            PresentationParameters presentationParameters = new PresentationParameters();

            presentationParameters.BackBufferWidth = 1280;
            presentationParameters.BackBufferHeight = 720;

            Platform.InitializeGraphics(presentationParameters);
            Platform.InitializeInput();

            Dispatcher.SpawnDefaultThreads();

            _graphicsDevice = GetService<IGraphicsDeviceManager>().GraphicsDevice;
            _inputManager = GetService<IInputManager>();
            _contentManager = new ContentManager(_graphicsDevice);

            _font = _contentManager.LoadContent<Font>("Fonts/Arial");
            _batch = new GraphicBatch();
        }

        protected override void Update(GameTime gameTime)
        {
            _fpsAcc++;
            _fpsCounter += gameTime.ElapsedTime.TotalSeconds;

            if (_fpsCounter >= 1.0f)
            {
                _fps = _fpsAcc / _fpsCounter;
                _fpsCounter = 0;
                _fpsAcc = 0;
            }
        }

        protected override void Render(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.CornflowerBlue);

            _batch.Begin();
            _batch.DrawString(_fps.ToString(), _font, new Vector2(0, 0), Color.White);
            _batch.End();

            _graphicsDevice.Present();
        }

        protected override void OnResize()
        {
            if (_graphicsDevice == null)
                return;

            _graphicsDevice.Viewport = new Viewport(0, 0, _graphicsDevice.PresentationParameters.BackBufferWidth,
                                                    _graphicsDevice.PresentationParameters.BackBufferHeight);
        }

        protected override void Shutdown()
        {

        }
    }
}
