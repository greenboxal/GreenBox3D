/*using Awesomium.Core;
using GreenBox3D.Graphics;
using GreenBox3D.Input;
//using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using ACore = Awesomium.Core;

namespace GreenBox3D.Awesomium
{
    public class GuiManager : IMouseFilter, IKeyboardFilter, ISurface
    {
        private readonly GraphicsDevice _graphicsDevice;
        private WebSession _session;
        private WebView _view;

        private int _width, _height;
        private byte[] _backbuffer;

        public GuiManager(Game game)
        {
            IInputManager input = game.GetService<IInputManager>();

            input.RegisterKeyboardFilter(this);
            input.RegisterMouseFilter(this);

            _graphicsDevice = game.GetService<IGraphicsDeviceManager>().GraphicsDevice;

            WebCore.Initialize(new WebConfig(), true);
            _session = WebCore.CreateWebSession(new WebPreferences());
            _view = WebCore.CreateWebView(_graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight, _session, WebViewType.Offscreen);
            _view.IsTransparent = true;
            _view.CreateSurface += _view_CreateSurface;
            _view.Source = new Uri("http://www.google.com/");
        }

        void _view_CreateSurface(object sender, CreateSurfaceEventArgs e)
        {
            e.Surface = this;
        }

        public void Shutdown()
        {
            WebCore.Shutdown();
        }

        public void Resized()
        {
            _view.Resize(_graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);
        }

        public void Update()
        {
            WebCore.Update();
        }

        public void Render()
        {
            if (_backbuffer != null)
            {
          //      GL.RasterPos2(0, 0);
          //      GL.DrawPixels(_width, _height, PixelFormat.Bgra, PixelType.UnsignedByte, _backbuffer);
            }
        }

        bool IMouseFilter.OnMouseDown(Input.MouseButton button, int x, int y)
        {
            ACore.MouseButton mb;

            if (_backbuffer == null || x >= _width || y >= _height)
                return false;

            if (_backbuffer[(y * _width + x) * 4 + 3] < 128)
                return false;

            switch (button)
            {
                case Input.MouseButton.Left:
                    mb = ACore.MouseButton.Left;
                    break;
                case Input.MouseButton.Middle:
                    mb = ACore.MouseButton.Middle;
                    break;
                case Input.MouseButton.Right:
                    mb = ACore.MouseButton.Right;
                    break;
                default:
                    return false;
            }

            _view.InjectMouseDown(mb);

            return true;
        }

        bool IMouseFilter.OnMouseUp(Input.MouseButton button, int x, int y)
        {
            ACore.MouseButton mb;

            if (_backbuffer == null || x >= _width || y >= _height)
                return false;

            if (_backbuffer[(y * _width + x) * 4 + 3] < 128)
                return false;

            switch (button)
            {
                case Input.MouseButton.Left:
                    mb = ACore.MouseButton.Left;
                    break;
                case Input.MouseButton.Middle:
                    mb = ACore.MouseButton.Middle;
                    break;
                case Input.MouseButton.Right:
                    mb = ACore.MouseButton.Right;
                    break;
                default:
                    return false;
            }

            _view.InjectMouseUp(mb);

            return true;
        }

        bool IMouseFilter.OnMouseMove(int x, int y)
        {
            if (_backbuffer == null || x >= _width || y >= _height)
                return false;

            if (_backbuffer[(y * _width + x) * 4 + 3] < 128)
                return false;

            _view.InjectMouseMove(x, y);

            return true;
        }

        bool IMouseFilter.OnMouseWheel(int delta, int x, int y)
        {
            if (_backbuffer == null || x >= _width || y >= _height)
                return false;

            if (_backbuffer[(y * _width + x) * 4 + 3] < 128)
                return false;

            _view.InjectMouseWheel(delta, 0);

            return true;
        }

        bool IKeyboardFilter.OnKeyDown(Keys key, KeyModifiers mod)
        {
           // WebKeyboardEvent kb = new WebKeyboardEvent();



            //_view.InjectKeyboardEvent(kb);
            return false;
        }

        bool IKeyboardFilter.OnKeyUp(Keys key, KeyModifiers mod)
        {
            //WebKeyboardEvent kb = new WebKeyboardEvent();

            //kb.Type = WebKeyboardEventType.

            //_view.InjectKeyboardEvent(kb);
            return false;
        }

        bool IKeyboardFilter.OnKeyChar(char unicode)
        {
            return false;
        }

        void ISurface.Initialize(IWebView view, int width, int height)
        {
            _width = width;
            _height = height;
            _backbuffer = new byte[width * height * 4];
        }

        void ISurface.Paint(IntPtr srcBuffer, int srcRowSpan, AweRect srcRect, AweRect destRect)
        {
            for (int y = 0; y < srcRect.Height; y++)
                Marshal.Copy(srcBuffer + ((srcRect.Y + y) * srcRect.Width + srcRect.X) * 4, _backbuffer, ((_height - (destRect.Y + y) - 1) * _width + destRect.X) * 4, destRect.Width * 4);
        }

        void ISurface.Scroll(int dx, int dy, AweRect clipRect)
        {
            int cX = clipRect.X;
            int cY = clipRect.Y;
            int h = clipRect.Height;

            if (dy < 0)
            {
                return;
            }

          //  cY = _height - cY - 1;
          //  dy = _height - dy - 1;

            for (int y = 0; y < h; y++)
                Buffer.BlockCopy(_backbuffer, ((cY + y) * _width + cX) * 4, _backbuffer, ((dy + y) * _width + dx) * 4, clipRect.Width * 4);
        }

        void IDisposable.Dispose()
        {
            _backbuffer = null;
        }
    }
}*/
