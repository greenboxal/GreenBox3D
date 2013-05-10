// GraphicBatch.cs
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
using GreenBox3D.Content;

namespace GreenBox3D.Graphics
{
    public class GraphicBatch : GraphicsResource
    {
        internal Shader StandardShader;
        internal ShaderParameter StandardShaderMatrixParameter;
        internal Matrix4 StandardViewProjection;
        internal SavedVertexState SavedState;
        internal VertexBuffer VertexBuffer;
        internal IndexBuffer IndexBuffer;

        private List<GraphicOperation> _opList;

        public GraphicBatch()
        {
            CreateSharedResources();
        }

        private void CreateSharedResources()
        {
            ContentManager temp = new ContentManager(GraphicsDevice);
            StandardShader = temp.LoadContent<Shader>("Shaders/SpriteTexture");
            StandardShaderMatrixParameter = StandardShader.Parameters["WVP"];

            SavedState = new SavedVertexState();
            VertexBuffer = new VertexBuffer(typeof(VertexPositionTexture), BufferUsage.StreamDraw);
            IndexBuffer = new IndexBuffer(IndexElementSize.SixteenBits, BufferUsage.StreamDraw);

            SavedState.Bind();
            StandardShader.Apply();
            IndexBuffer.Bind();
            VertexBuffer.Bind();
            VertexBuffer.VertexDeclaration.Apply();
            SavedState.Unbind();
        }

        public void Begin()
        {
            Begin(Matrix4.Identity);
        }

        public void Begin(Matrix4 transform)
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            _opList = new List<GraphicOperation>();
            StandardViewProjection = Matrix4.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
                                                                GraphicsDevice.Viewport.Height, 0, -1, 1) * transform;
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            Draw(texture, destinationRectangle, null, color);
        }

        public void Draw(Texture2D texture,
                                Vector2 position,
                                Color color)
        {
            Draw(texture, new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height), null,
                 color);
        }

        public void Draw(Texture2D texture,
                                Vector2 position, Rectangle? sourceRectangle,
                                Color color)
        {
            Draw(texture, new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height),
                 sourceRectangle, color);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            Rectangle src;

            if (sourceRectangle.HasValue)
                src = sourceRectangle.Value;
            else
                src = new Rectangle(0, 0, texture.Width, texture.Height);

            Push(new TextureGraphicOperation(this, texture, destinationRectangle, src, color));
        }

        public void DrawString(string text, Font font, Vector2 position, Color color)
        {
            DrawString(text, font, new Rectangle((int)position.X, (int)position.Y, -1, -1), color);
        }

        public void DrawString(string text, Font font, Rectangle bounds, Color color)
        {
            Push(new FontGraphicOperation(this, text, font, bounds, color));
        }

        public void End()
        {
            StandardShader.Apply();
            StandardShaderMatrixParameter.SetValue(StandardViewProjection);

            SavedState.Bind();
            foreach (GraphicOperation t in _opList)
            {
                t.Render();
                t.Dispose();
            }
            SavedState.Unbind();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StandardShader.Dispose();
                IndexBuffer.Dispose();
                VertexBuffer.Dispose();
            }

            base.Dispose(disposing);
        }

        internal void Push(GraphicOperation op)
        {
            _opList.Add(op);
        }
    }
}
