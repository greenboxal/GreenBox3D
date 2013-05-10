using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using Xilium.CefGlue;

namespace GreenBox3D.Awesomium
{
    internal class GuiRenderHandler : CefRenderHandler
    {
        private readonly GuiManager _owner;
        private readonly GraphicsDevice _graphicsDevice;

        private readonly GraphicBatch _batch;

        private bool _isDirty;
        private Texture2D _texture;
        private byte[] _data;

        public GuiRenderHandler(GuiManager owner)
        {
            _owner = owner;
            _graphicsDevice = owner.Owner.GetService<IGraphicsDeviceManager>().GraphicsDevice;
            _batch = new GraphicBatch();

            Resize(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        }

        public void Resize(int w, int h)
        {
            if (_texture != null && _texture.Width == w && _texture.Height == h)
                return;

            _texture = new Texture2D(SurfaceFormat.Color, w, h);
            _data = new byte[w * h * 4];
        }

        public void Update()
        {
            if (!_isDirty)
                return;

            _texture.SetData(_data, PixelFormat.Bgra, PixelType.UnsignedInt8888Reversed);
            _isDirty = false;
        }

        public void Render()
        {
            if (_texture == null)
                return;

            _batch.Begin();
            _batch.Draw(_texture, new Vector2(0, 0), Color.White);
            _batch.End();
        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
        {
            rect = new CefRectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            return true;
        }

        protected override bool GetViewRect(CefBrowser browser, ref CefRectangle rect)
        {
            rect = new CefRectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            return true;
        }

        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            screenX = viewX;
            screenY = viewY;
            return true;
        }

        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
            return false;
        }

        protected override void OnPopupShow(CefBrowser browser, bool show)
        {

        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {
            
        }

        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            if (type == CefPaintElementType.View)
            {
                int stride = width * 4;

                foreach (CefRectangle rect in dirtyRects)
                {
                    int offset = rect.X * 4;

                    for (int y = 0; y < rect.Height; y++)
                        Marshal.Copy(buffer + (rect.Y + y) * stride + offset, _data, (rect.Y + y) * stride + offset, stride);
                }

                _isDirty = true;
            }
        }

        protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle)
        {

        }

        public int GetAlphaAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _texture.Width || y >= _texture.Height)
                return 0;

            return _data[(y * _texture.Width + x) * 4 + 3];
        }
    }
}
