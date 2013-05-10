using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    internal class FontGraphicOperation : GraphicOperation
    {
        private readonly GraphicBatch _graphicBatch;
        private readonly Color _color;
        private readonly VertexPositionTexture[] _vertices;
        private readonly short[] _indicies;
        private readonly Texture2D _page;

        public FontGraphicOperation(GraphicBatch graphicBatch, string text, Font font, Rectangle bounds, Color color)
        {
            bool checkWidth = bounds.Width >= 0;
            int x = bounds.X, y = bounds.Y;

            _graphicBatch = graphicBatch;
            _color = color;
            _page = font.Pages[0];

            _vertices = new VertexPositionTexture[text.Length * 4];
            _indicies = new short[text.Length * 6];

            for (int i = 0; i < text.Length; i++)
            {
                int offset = i * 4, ioffset = i * 6;
                FontGlyph glyph;

                glyph = font.GetGlyph(text[i]);

                if (glyph == null)
                {
                    glyph = font.GetGlyph((char)0);

                    if (glyph == null)
                        continue;
                }

                if (checkWidth && x + glyph.Offset.X + glyph.Advance >= bounds.Width)
                {
                    Array.Resize(ref _vertices, offset);
                    Array.Resize(ref _indicies, ioffset);
                    break;
                }

                int x0 = x + glyph.Offset.X;
                int y0 = y + glyph.Offset.Y;
                int x1 = x + glyph.Offset.X + glyph.PageBounds.Width;
                int y1 = y + glyph.Offset.Y + glyph.PageBounds.Height;

                float u0 = glyph.PageBounds.X / (float)_page.Width;
                float v0 = glyph.PageBounds.Y / (float)_page.Height;
                float u1 = (glyph.PageBounds.X + glyph.PageBounds.Width) / (float)_page.Width;
                float v1 = (glyph.PageBounds.Y + glyph.PageBounds.Height) / (float)_page.Height;

                _vertices[offset + 0] = new VertexPositionTexture(new Vector3(x0, y0, 0),
                                                             new Vector2(u0, v0));

                _vertices[offset + 1] = new VertexPositionTexture(new Vector3(x1, y0, 0),
                                                             new Vector2(u1, v0));

                _vertices[offset + 2] = new VertexPositionTexture(new Vector3(x0, y1, 0),
                                                             new Vector2(u0, v1));

                _vertices[offset + 3] =
                    new VertexPositionTexture(new Vector3(x1, y1, 0),
                                              new Vector2(u1, v1));

                _indicies[ioffset + 0] = (short)(offset + 0);
                _indicies[ioffset + 1] = (short)(offset + 1);
                _indicies[ioffset + 2] = (short)(offset + 2);
                _indicies[ioffset + 3] = (short)(offset + 2);
                _indicies[ioffset + 4] = (short)(offset + 1);
                _indicies[ioffset + 5] = (short)(offset + 3);

                x += glyph.Advance;

                if (i < text.Length - 1 && glyph.Kerning.TryGetValue(text[i + 1], out offset))
                    x += offset;
            }
        }

        public override void Render()
        {
            _graphicBatch.IndexBuffer.SetData(_indicies);
            _graphicBatch.VertexBuffer.SetData(_vertices);

            _graphicBatch.StandardShader.Parameters["Tint"].SetValue(_color);
            _graphicBatch.StandardShader.Parameters["Texture"].SetValue(0);

            _graphicBatch.GraphicsDevice.Textures[0] = _page;
            _graphicBatch.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            _graphicBatch.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _graphicBatch.IndexBuffer.ElementCount);
        }

        public override void Dispose()
        {

        }
    }
}
