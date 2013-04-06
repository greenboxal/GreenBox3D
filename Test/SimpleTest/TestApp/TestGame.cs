// This file is part of GreenBox3D
//
// Copyright (c) 2013 The GreenBox Development Inc., all rights reserved
// Redistributing this file without express authorization may lead to legal actions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
using GreenBox3D;
using GreenBox3D.Graphics;
using GreenBox3D.Input;
using GreenBox3D.Awesomium;
using GreenBox3D.Content;
using GreenBox3D.ContentPipeline;
using GreenBox3D.ContentPipeline.Compiler;

namespace TestApp
{
    public class TestGame : Game
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(TestGame));

        private GraphicsDevice _graphicsDevice;
        private IInputManager _inputManager;

        private IEffect _effect;
        private IIndexBuffer _indices;
        private IVertexBuffer _vertices;

        public TestGame()
        {
            FileManager.RegisterLoader(new FolderFileLoader("./Output/"));

#if DEBUG
       //     PipelineManager.RegisterJustInDesignExtensions(new PipelineProject("ContentProject.rb"));
#endif
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

            _effect = _graphicsDevice.EffectManager.LoadEffect("Simple/Simple");

            int[] indices = new[] { 0, 1, 2 };
            VertexPositionNormalColor[] positions = new[]
            {
                new VertexPositionNormalColor(new Vector3(0.75f, 0.75f, 0.0f), new Vector3(), new Color(255, 0, 0)), new VertexPositionNormalColor(new Vector3(0.75f, -0.75f, 0.0f), new Vector3(), new Color(0, 255, 0)),
                new VertexPositionNormalColor(new Vector3(-0.75f, -0.75f, 0.0f), new Vector3(), new Color(0, 0, 255)),
            };

            _indices = _graphicsDevice.BufferManager.CreateIndexBuffer(IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.StaticDraw);
            _indices.SetData(indices);

            _vertices = _graphicsDevice.BufferManager.CreateVertexBuffer(typeof(VertexPositionNormalColor), positions.Length, BufferUsage.StaticDraw);
            _vertices.SetData(positions);
        }

        protected override void Update(GameTime gameTime)
        {
        }

        protected override void Render(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.Black);

            _graphicsDevice.Indices = _indices;
            _graphicsDevice.SetVertexBuffer(_vertices);

            _effect.Parameters["WorldViewProjection"].SetValue(Matrix4.Identity);

            foreach (IEffectPass pass in _effect.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, _indices.ElementCount, 0, 1);
            }
        }

        protected override void OnResize()
        {
        }

        protected override void Shutdown()
        {
        }
    }
}
*/