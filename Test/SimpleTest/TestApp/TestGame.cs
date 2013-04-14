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

        private GraphicsDevice _graphicsDevice;
        private IInputManager _inputManager;

        public TestGame()
        {
            FileManager.RegisterLoader(new FolderFileLoader("./Output/"));
        }

        protected override void Initialize()
        {
            PresentationParameters presentationParameters = new PresentationParameters();

            presentationParameters.BackBufferWidth = 1280;
            presentationParameters.BackBufferHeight = 720;

            Platform.InitializeGraphics(presentationParameters);
            Platform.InitializeInput();

            _graphicsDevice = GetService<IGraphicsDeviceManager>().GraphicsDevice;
            _inputManager = GetService<IInputManager>();
        }

        protected override void Update(GameTime gameTime)
        {
        }

        protected override void Render(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.Black);
        }

        protected override void OnResize()
        {
        }

        protected override void Shutdown()
        {
        }
    }
}
