using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using GreenBox3D.Input;
using Xilium.CefGlue;

namespace GreenBox3D.Awesomium
{
    public class GuiManager : IDisposable, IMouseFilter, IKeyboardFilter
    {
        private readonly IInputManager _inputManager;
        private bool _forceMouseUp, _forceKeyUp;
        private CefEventFlags _mouseMods;

        private readonly GuiClient _client;
        private readonly CefWindowInfo _windowInfo;
        private CefBrowser _browser;
        private CefBrowserHost _host;
        private CefFrame _mainFrame;

        internal Game Owner { get; private set; }
        internal CefFocusSource CurrentFocusSource { get; set; }

        internal void OnCreated(CefBrowser browser)
        {
            _browser = browser;
            _host = _browser.GetHost();
            _mainFrame = _browser.GetMainFrame();
            _host.SetFocus(true);
        }

        public GuiManager(Game game)
        {
            Owner = game;

            _inputManager = game.GetService<IInputManager>();
            _inputManager.RegisterMouseFilter(this);
            _inputManager.RegisterKeyboardFilter(this);

            _client = new GuiClient(this);

            _windowInfo = CefWindowInfo.Create();
            _windowInfo.SetAsOffScreen(game.Platform.Window.NativeHandle);
            _windowInfo.TransparentPainting = true;

            CefBrowserSettings settings = new CefBrowserSettings();
            CefBrowserHost.CreateBrowser(_windowInfo, _client, settings, "http://www.google.com");
        }

        public void Open(string path)
        {
            _mainFrame.LoadUrl(path);
        }

        public void Resize(int width, int height)
        {
            _client.Resize(width, height);

            if (_host != null)
                _host.WasResized();
        }

        public void Update()
        {
            _client.Update();
        }
        
        public void Render()
        {
            _client.Render();
        }

        public void Dispose()
        {
            _inputManager.RegisterMouseFilter(this);
            _inputManager.UnregisterKeyboardFilter(this);
        }

        private CefKeyEvent CreateKeyEvent(Keys key, KeyModifiers mod, CefKeyEventType type)
        {
            CefKeyEvent e = new CefKeyEvent();

            e.EventType = type;
            e.WindowsKeyCode = (int)key;
            
            if ((mod & KeyModifiers.Alt) != KeyModifiers.None)
                e.Modifiers |= CefEventFlags.AltDown;

            if ((mod & KeyModifiers.Control) != KeyModifiers.None)
                e.Modifiers |= CefEventFlags.ControlDown;

            if ((mod & KeyModifiers.Shift) != KeyModifiers.None)
                e.Modifiers |= CefEventFlags.ShiftDown;

            return e;
        }

        bool IKeyboardFilter.OnKeyDown(Keys key, KeyModifiers mod)
        {
            if (_host != null && ChromiumRuntime.SelectedNodeEditable)
            {
                _host.SendKeyEvent(CreateKeyEvent(key, mod, CefKeyEventType.KeyDown));
                _forceKeyUp = true;
                return true;
            }

            return false;
        }

        bool IKeyboardFilter.OnKeyUp(Keys key, KeyModifiers mod)
        {
            if (_forceKeyUp)
            {
                _host.SendKeyEvent(CreateKeyEvent(key, mod, CefKeyEventType.KeyUp));
                _forceKeyUp = false;
                return true;
            }

            return false;
        }

        bool IKeyboardFilter.OnKeyChar(char unicode)
        {
            if (_host != null && ChromiumRuntime.SelectedNodeEditable)
            {
                CefKeyEvent e = new CefKeyEvent();

                e.EventType = CefKeyEventType.Char;
                e.Character = unicode;
                e.UnmodifiedCharacter = unicode;
                _host.SendKeyEvent(e);

                return true;
            }

            return false;
        }

        bool IMouseFilter.OnMouseDown(MouseButton button, int x, int y)
        {
            if (_host == null || !_client.CheckMousePosition(x, y))
            {
                if (_host != null)
                    _host.SetFocus(false);

                return false;
            }

            CefMouseButtonType type;

            switch (button)
            {
                case MouseButton.Left:
                    _mouseMods |= CefEventFlags.LeftMouseButton;
                    type = CefMouseButtonType.Left;
                    break;
                case MouseButton.Middle:
                    _mouseMods |= CefEventFlags.MiddleMouseButton;
                    type = CefMouseButtonType.Middle;
                    break;
                case MouseButton.Right:
                    _mouseMods |= CefEventFlags.RightMouseButton;
                    type = CefMouseButtonType.Right;
                    break;
                default:
                    return false;
            }

            _host.SendMouseClickEvent(new CefMouseEvent(x, y, CefEventFlags.None), type, false, 1);
            _forceMouseUp = true;
            return true;
        }

        bool IMouseFilter.OnMouseUp(MouseButton button, int x, int y)
        {
            if (!_forceMouseUp)
                return false;

            CefMouseButtonType type;

            switch (button)
            {
                case MouseButton.Left:
                    _mouseMods &= ~CefEventFlags.LeftMouseButton;
                    type = CefMouseButtonType.Left;
                    break;
                case MouseButton.Middle:
                    _mouseMods &= ~CefEventFlags.MiddleMouseButton;
                    type = CefMouseButtonType.Middle;
                    break;
                case MouseButton.Right:
                    _mouseMods &= ~CefEventFlags.RightMouseButton;
                    type = CefMouseButtonType.Right;
                    break;
                default:
                    return false;
            }

            _host.SendMouseClickEvent(new CefMouseEvent(x, y, CefEventFlags.None), type, true, 1);
            _forceMouseUp = false;
            return true;
        }

        bool IMouseFilter.OnMouseMove(int x, int y)
        {
            if (_host == null)
                return false;

            bool inArea = _client.CheckMousePosition(x, y);
            _host.SendMouseMoveEvent(new CefMouseEvent(x, y, CefEventFlags.None), !inArea);

            return inArea;
        }

        bool IMouseFilter.OnMouseWheel(int delta, int x, int y)
        {
            if (_host == null || !_client.CheckMousePosition(x, y))
                return false;

            _host.SendMouseWheelEvent(new CefMouseEvent(x, y, _mouseMods), delta, 0);

            return true;
        }
    }
}
