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
        private Font _arial;
        private ContentManager _contentManager;

        private GraphicsDevice _graphicsDevice;
        private GraphicBatch _graphicBatch;

        private IIndexBuffer _indices;
        private IInputManager _inputManager;
        private IShader _shader;
        private ITexture2D _tex;
        private IVertexBuffer _vertices;

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

            _graphicsDevice = GetService<IGraphicsDeviceManager>().GraphicsDevice;
            _inputManager = GetService<IInputManager>();
            _contentManager = new ContentManager(_graphicsDevice);
            _graphicBatch = new GraphicBatch(_graphicsDevice);

            _shader = _contentManager.LoadContent<IShader>("Shaders/Simple");
            _tex = _contentManager.LoadContent<ITexture2D>("Image1");
            _arial = _contentManager.LoadContent<Font>("Fonts/Arial");

            int[] indices = new[]
            {
                0, 1, 2, 2, 3, 0,
                3, 2, 6, 6, 7, 3,
                7, 6, 5, 5, 4, 7,
                4, 0, 3, 3, 7, 4,
                0, 1, 5, 5, 4, 0,
                1, 5, 6, 6, 2, 1
            };

            VertexPositionNormalColor[] positions = new[]
            {
                new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(), new Color(255, 0, 0)),
                new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(), new Color(0, 255, 0)),
                new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(), new Color(0, 0, 255)),
                new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(), new Color(255, 0, 0)),
                new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(), new Color(0, 255, 0)),
                new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(), new Color(0, 0, 255)),
                new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(), new Color(0, 255, 0)),
                new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(), new Color(0, 0, 255)),
            };

            _indices = _graphicsDevice.BufferManager.CreateIndexBuffer(IndexElementSize.ThirtyTwoBits, indices.Length,
                                                                       BufferUsage.StaticDraw);
            _indices.SetData(indices);

            _vertices = _graphicsDevice.BufferManager.CreateVertexBuffer(typeof(VertexPositionNormalColor),
                                                                         positions.Length, BufferUsage.StaticDraw);
            _vertices.SetData(positions);
        }

        protected override void Update(GameTime gameTime)
        {
        }

        protected override void Render(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.CornflowerBlue);

            _graphicsDevice.SetIndexBuffer(_indices);
            _graphicsDevice.SetVertexBuffer(_vertices);

            _shader.Apply();

            _shader.Parameters["WorldViewProjection"].SetValueTranspose(
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45),
                                                     _graphicsDevice.Viewport.AspectRatio, 1, 5) *
                Matrix4.LookAt(0, 0, 3, 0, 0, 0, 0, 1, 0));
            // _shader.Parameters["Alpha"].SetValue(1.0f);

            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, _indices.ElementCount);

            _graphicBatch.Begin();
            _graphicBatch.Draw(_tex, new Vector2(0, 0), Color.White);
            _graphicBatch.End();

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
