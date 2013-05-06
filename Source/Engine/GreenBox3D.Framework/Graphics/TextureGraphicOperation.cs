// TextureGraphicOperation.cs
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

namespace GreenBox3D.Graphics
{
    public class TextureGraphicOperation : GraphicOperation
    {
        private readonly GraphicBatch _graphicBatch;
        private readonly Texture2D _texture;
        private readonly VertexBuffer _vertices;
        private readonly Color _color;

        public TextureGraphicOperation(GraphicBatch graphicBatch, Texture2D texture, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color)
        {
            _graphicBatch = graphicBatch;
            _texture = texture;
            _vertices = new VertexBuffer(typeof(VertexPositionTexture), 4, BufferUsage.StaticDraw);
            _vertices.SetData(new[]
            {
                new VertexPositionTexture(new Vector3(destinationRectangle.X, destinationRectangle.Y, 0), new Vector2(sourceRectangle.X / texture.Width, sourceRectangle.Y / texture.Height)),
                new VertexPositionTexture(new Vector3(destinationRectangle.X + destinationRectangle.Width, destinationRectangle.Y, 0), new Vector2((sourceRectangle.X + sourceRectangle.Width) / texture.Width, sourceRectangle.Y / texture.Height)),
                new VertexPositionTexture(new Vector3(destinationRectangle.X, destinationRectangle.Y + destinationRectangle.Height, 0), new Vector2(sourceRectangle.X / texture.Width, (sourceRectangle.Y + sourceRectangle.Height) / texture.Height)),
                new VertexPositionTexture(new Vector3(destinationRectangle.X + destinationRectangle.Width, destinationRectangle.Y + destinationRectangle.Height, 0), new Vector2((sourceRectangle.X + sourceRectangle.Width) / texture.Width, (sourceRectangle.Y + sourceRectangle.Height) / texture.Height)),
            });
            _color = color;
        }

        public override void Render()
        {
            _graphicBatch.GraphicsDevice.Textures[0] = _texture;
            _graphicBatch.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            _graphicBatch.StandardShader.Apply();
            _graphicBatch.StandardShaderMatrixParameter.SetValue(_graphicBatch.StandardViewProjection);
            _graphicBatch.StandardShader.Parameters["Tint"].SetValue(_color);
            _graphicBatch.StandardShader.Parameters["Texture"].SetValue(0);
            _graphicBatch.GraphicsDevice.SetVertexBuffer(_vertices);
            _graphicBatch.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 4);
        }

        public override void Dispose()
        {
            _vertices.Dispose();
        }
    }
}
