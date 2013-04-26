// WindowsGameWindow.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using GreenBox3D.Input;
using GreenBox3D.Platform.Windows.Input;

namespace GreenBox3D.Platform.Windows
{
    public class WindowsGameWindow : IGameWindow, IDisposable, IMouse, IKeyboard
    {
        private static int _classCounter;

        private readonly NativeMethods.WndProc _callback;
        private readonly PresentationParameters _creationParameters;
        private readonly List<Keys> _keys;
        private readonly WindowsGamePlatform _platform;
        internal IntPtr Handle;

        private bool _allowResizing;
        private Rectangle _bounds;

        private IntPtr _class;
        private bool _disposed;
        private InputManager _inputManager;
        private bool _isCursorVisible;
        private MouseState _mouseState;
        private string _title;

        public WindowsGameWindow(WindowsGamePlatform platform, PresentationParameters parameters)
        {
            _callback = WndProc;
            _creationParameters = parameters;
            _platform = platform;
            _keys = new List<Keys>();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_class != IntPtr.Zero)
                    NativeMethods.UnregisterClass(_class, Marshal.GetHINSTANCE(typeof(WindowsGameWindow).Module));

                if (Handle != IntPtr.Zero)
                    NativeMethods.DestroyWindow(Handle);

                _disposed = true;
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NativeMethods.SetWindowText(Handle, _title);
            }
        }

        public bool AllowUserResizing
        {
            get { return _allowResizing; }
            set
            {
                _allowResizing = value;

                NativeMethods.WindowStyles ws =
                    (NativeMethods.WindowStyles)NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_STYLE);

                if (_allowResizing)
                    ws |= NativeMethods.WindowStyles.WS_SIZEFRAME | NativeMethods.WindowStyles.WS_MAXIMIZEBOX;
                else
                    ws &= ~(NativeMethods.WindowStyles.WS_SIZEFRAME | NativeMethods.WindowStyles.WS_MAXIMIZEBOX);

                NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_STYLE, (int)ws);
            }
        }

        public bool IsCursorVisible
        {
            get { return _isCursorVisible; }
            set
            {
                _isCursorVisible = value;
                NativeMethods.SetCursor(_isCursorVisible
                                            ? NativeMethods.LoadCursor(IntPtr.Zero,
                                                                       NativeMethods.IDC_STANDARD_CURSORS.IDC_ARROW)
                                            : IntPtr.Zero);
            }
        }

        public Rectangle ClientBounds
        {
            get { return _bounds; }
        }

        public void Resize(int width, int height)
        {
        }

        KeyboardState IKeyboard.GetState()
        {
            return new KeyboardState(_keys.ToArray());
        }

        public MouseState GetState()
        {
            return _mouseState;
        }

        public void SetPosition(int x, int y)
        {
            NativeMethods.SetCursorPos(x, y);
        }

        ~WindowsGameWindow()
        {
            Dispose();
        }

        public void Create()
        {
            CreateClass();
            CreateWindow();

            AllowUserResizing = false;

            NativeMethods.SetFocus(Handle);
        }

        private void CreateClass()
        {
            NativeMethods.WNDCLASSEX cls = new NativeMethods.WNDCLASSEX();

            cls.cbSize = Marshal.SizeOf(cls);
            cls.style = NativeMethods.ClassStyles.OwnDC | NativeMethods.ClassStyles.HorizontalRedraw |
                        NativeMethods.ClassStyles.VerticalRedraw;
            cls.lpfnWndProc = _callback;
            cls.cbClsExtra = 0;
            cls.cbWndExtra = 0;
            cls.hInstance = Marshal.GetHINSTANCE(typeof(WindowsGameWindow).Module);
            cls.hIcon = IntPtr.Zero;
            cls.hCursor = NativeMethods.LoadCursor(IntPtr.Zero, NativeMethods.IDC_STANDARD_CURSORS.IDC_ARROW);
            cls.hbrBackground = IntPtr.Zero;
            cls.lpszMenuName = null;
            cls.lpszClassName = "GB3DWGWCLS" + _classCounter++;
            cls.hIconSm = IntPtr.Zero;

            if ((_class = NativeMethods.RegisterClassEx(ref cls)) == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error creating Window Class");
        }

        private void CreateWindow()
        {
            NativeMethods.WindowStyles ws;
            NativeMethods.WindowStylesEx wsx;

            if (_creationParameters.IsFullScreen)
            {
                SetFullScreen(true);
                wsx = NativeMethods.WindowStylesEx.WS_EX_APPWINDOW | NativeMethods.WindowStylesEx.WS_EX_TOPMOST;
                ws = NativeMethods.WindowStyles.WS_POPUP | NativeMethods.WindowStyles.WS_CLIPSIBLINGS |
                     NativeMethods.WindowStyles.WS_CLIPCHILDREN;
            }
            else
            {
                wsx = NativeMethods.WindowStylesEx.WS_EX_APPWINDOW | NativeMethods.WindowStylesEx.WS_EX_WINDOWEDGE;
                ws = NativeMethods.WindowStyles.WS_CAPTION | NativeMethods.WindowStyles.WS_OVERLAPPED |
                     NativeMethods.WindowStyles.WS_SYSMENU | NativeMethods.WindowStyles.WS_MINIMIZE |
                     NativeMethods.WindowStyles.WS_SIZEFRAME | NativeMethods.WindowStyles.WS_CLIPSIBLINGS |
                     NativeMethods.WindowStyles.WS_CLIPCHILDREN;
            }

            Handle = NativeMethods.CreateWindowEx(wsx, _class, "GreenBox3D", ws, 128, 128,
                                                  _creationParameters.BackBufferWidth,
                                                  _creationParameters.BackBufferHeight, IntPtr.Zero, IntPtr.Zero,
                                                  Marshal.GetHINSTANCE(typeof(WindowsGameWindow).Module), IntPtr.Zero);

            if (Handle == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error creating Window");

            NativeMethods.ShowWindow(Handle, NativeMethods.ShowWindowCommands.ShowDefault);
            NativeMethods.SetFocus(Handle);
        }

        public void SetFullScreen(bool fullscreen)
        {
            _creationParameters.IsFullScreen = fullscreen;
            SetFullScreenCore(fullscreen);
        }

        public void SetFullScreenCore(bool fullscreen)
        {
            NativeMethods.DEVMODE mod = new NativeMethods.DEVMODE();

            if (fullscreen)
            {
                mod.dmSize = (short)Marshal.SizeOf(mod);
                mod.dmPelsWidth = _creationParameters.BackBufferWidth;
                mod.dmPelsHeight = _creationParameters.BackBufferHeight;
                mod.dmBitsPerPel = 32;
                mod.dmFields = NativeMethods.DM.BitsPerPixel | NativeMethods.DM.PelsWidth | NativeMethods.DM.PelsHeight;

                if (NativeMethods.ChangeDisplaySettings(ref mod, NativeMethods.CDS.Fullscreen) !=
                    NativeMethods.DISP_CHANGE.Successful)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Error changing display mode");
            }
            else
            {
                mod.dmSize = (short)Marshal.SizeOf(mod);

                if (NativeMethods.ChangeDisplaySettings(ref mod, NativeMethods.CDS.Reset) !=
                    NativeMethods.DISP_CHANGE.Successful)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Error changing display mode");
            }
        }

        public void HandleEvents()
        {
            NativeMethods.MSG msg;

            while (NativeMethods.PeekMessage(out msg, Handle, 0, 0, 1))
            {
                NativeMethods.TranslateMessage(ref msg);
                NativeMethods.DispatchMessage(ref msg);
            }
        }

        public void Close()
        {
            NativeMethods.DestroyWindow(Handle);
            Handle = IntPtr.Zero;
        }

        private IntPtr WndProc(IntPtr hWnd, NativeMethods.WM msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case NativeMethods.WM.SIZE:
                    if (lParam != IntPtr.Zero)
                    {
                        _bounds = new Rectangle(_bounds.X, _bounds.Y, lParam.ToInt32() & 0xffff, lParam.ToInt32() >> 16);
                        _platform.WindowResized();
                    }
                    break;
                case NativeMethods.WM.MOVING:
                case NativeMethods.WM.MOVE:
                    _bounds = new Rectangle(lParam.ToInt32() & 0xffff, lParam.ToInt32() >> 16, _bounds.Width,
                                            _bounds.Height);
                    break;
                case NativeMethods.WM.ACTIVATE:
                    {
                        bool active = wParam != IntPtr.Zero;

                        if (_creationParameters.IsFullScreen)
                            SetFullScreenCore(active);

                        _platform.SetActive(active);

                        if (active)
                            IsCursorVisible = _isCursorVisible;
                    }
                    break;
                case NativeMethods.WM.KEYDOWN:
                    {
                        Keys key;
                        KeyModifiers mod;

                        if ((lParam.ToInt32() & 0x40000000) != 0)
                            break;

                        TranslateKeys(wParam, out key, out mod);
                        _keys.Add(key);

                        lock (_inputManager.KeyboardFilters)
                        {
                            foreach (IKeyboardFilter filter in _inputManager.KeyboardFilters)
                                if (filter.OnKeyDown(key, mod))
                                    break;
                        }
                    }
                    break;
                case NativeMethods.WM.KEYUP:
                    {
                        Keys key;
                        KeyModifiers mod;

                        TranslateKeys(wParam, out key, out mod);
                        _keys.Remove(key);

                        lock (_inputManager.KeyboardFilters)
                        {
                            foreach (IKeyboardFilter filter in _inputManager.KeyboardFilters)
                                if (filter.OnKeyUp(key, mod))
                                    break;
                        }
                    }
                    break;
                case NativeMethods.WM.CHAR:
                    {
                        lock (_inputManager.KeyboardFilters)
                        {
                            foreach (IKeyboardFilter filter in _inputManager.KeyboardFilters)
                                filter.OnKeyChar((char)wParam);
                        }
                    }
                    break;
                    // FIXME: Will we use this?
                    /*case NativeMethods.WM.INPUT:
                    {
                        int pcbSize = sizeof(NativeMethods.RawInput);
                        NativeMethods.RawInput raw = new NativeMethods.RawInput();
                        NativeMethods.GetRawInputData(lParam, NativeMethods.RawInputCommand.Input, out raw, ref pcbSize,
                                                      sizeof(NativeMethods.RawInputHeader));

                        if (raw.Header.Type == NativeMethods.RawInputType.Mouse)
                        {
                            //_mouseState.X = raw.Data.Mouse.LastX;
                            //_mouseState.Y = raw.Data.Mouse.LastY;
                        }
                    }
                    break;*/
                case NativeMethods.WM.MOUSEMOVE:
                    {
                        int x = unchecked((short)lParam);
                        int y = unchecked((short)((uint)lParam >> 16));

                        lock (_inputManager.MouseFilters)
                        {
                            foreach (IMouseFilter filter in _inputManager.MouseFilters)
                                if (filter.OnMouseMove(x, y))
                                    break;
                        }
                    }
                    break;
                case NativeMethods.WM.LBUTTONDOWN:
                case NativeMethods.WM.MBUTTONDOWN:
                case NativeMethods.WM.RBUTTONDOWN:
                    {
                        // TODO: Implement XButton handling
                        int x = unchecked((short)lParam);
                        int y = unchecked((short)((uint)lParam >> 16));

                        MouseButton button = MouseButton.Left;

                        switch (msg)
                        {
                            case NativeMethods.WM.LBUTTONDOWN:
                                button = MouseButton.Left;
                                _mouseState.LeftButton = ButtonState.Pressed;
                                break;
                            case NativeMethods.WM.MBUTTONDOWN:
                                button = MouseButton.Middle;
                                _mouseState.MiddleButton = ButtonState.Pressed;
                                break;
                            case NativeMethods.WM.RBUTTONDOWN:
                                button = MouseButton.Right;
                                _mouseState.RightButton = ButtonState.Pressed;
                                break;
                        }

                        lock (_inputManager.MouseFilters)
                        {
                            foreach (IMouseFilter filter in _inputManager.MouseFilters)
                                if (filter.OnMouseDown(button, x, y))
                                    break;
                        }
                    }
                    break;
                case NativeMethods.WM.LBUTTONUP:
                case NativeMethods.WM.MBUTTONUP:
                case NativeMethods.WM.RBUTTONUP:
                    {
                        int x = unchecked((short)lParam);
                        int y = unchecked((short)((uint)lParam >> 16));

                        MouseButton button = MouseButton.Left;

                        switch (msg)
                        {
                            case NativeMethods.WM.LBUTTONUP:
                                button = MouseButton.Left;
                                _mouseState.LeftButton = ButtonState.Released;
                                break;
                            case NativeMethods.WM.MBUTTONUP:
                                button = MouseButton.Middle;
                                _mouseState.MiddleButton = ButtonState.Released;
                                break;
                            case NativeMethods.WM.RBUTTONUP:
                                button = MouseButton.Right;
                                _mouseState.RightButton = ButtonState.Released;
                                break;
                        }

                        lock (_inputManager.MouseFilters)
                        {
                            foreach (IMouseFilter filter in _inputManager.MouseFilters)
                                if (filter.OnMouseUp(button, x, y))
                                    break;
                        }
                    }
                    break;
                case NativeMethods.WM.MOUSEWHEEL:
                    {
                        int x = unchecked((short)lParam);
                        int y = unchecked((short)((uint)lParam >> 16));
                        short wheel = ((short)(wParam.ToInt32() >> 16));

                        _mouseState.ScrollWheelValue += wheel;

                        lock (_inputManager.MouseFilters)
                        {
                            foreach (IMouseFilter filter in _inputManager.MouseFilters)
                                if (filter.OnMouseWheel(wheel, x, y))
                                    break;
                        }
                    }
                    break;
                case NativeMethods.WM.CLOSE:
                    _platform.Exit();
                    break;
                default:
                    return NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
            }

            return IntPtr.Zero;
        }

        private void TranslateKeys(IntPtr wParam, out Keys key, out KeyModifiers mod)
        {
            mod = KeyModifiers.None;

            // CapsLock
            if ((NativeMethods.GetKeyState(NativeMethods.VK_CAPITAL) & 0x0001) != 0)
                mod |= KeyModifiers.Capital;

            // Shift
            if ((NativeMethods.GetKeyState(NativeMethods.VK_SHIFT) & 0x8000) != 0)
                mod |= KeyModifiers.Shift;

            // Ctrl
            if ((NativeMethods.GetKeyState(NativeMethods.VK_CONTROL) & 0x8000) != 0)
                mod |= KeyModifiers.Control;

            // Alt
            if ((NativeMethods.GetKeyState(NativeMethods.VK_MENU) & 0x8000) != 0)
                mod |= KeyModifiers.Alt;

            // Luckily our Keys enum is mapped to Win32 VK's
            key = (Keys)wParam;
        }

        internal unsafe void SetInputManager(InputManager manager)
        {
            if (_inputManager == null)
            {
                NativeMethods.RAWINPUTDEVICE[] rid = new NativeMethods.RAWINPUTDEVICE[1];
                rid[0].UsagePage = NativeMethods.HIDUsagePage.Generic;
                rid[0].Usage = NativeMethods.HIDUsage.Mouse;
                rid[0].Flags = NativeMethods.RawInputDeviceFlags.InputSink;
                rid[0].WindowHandle = Handle;
                NativeMethods.RegisterRawInputDevices(rid, 1, sizeof(NativeMethods.RAWINPUTDEVICE));
            }

            _inputManager = manager;
        }
    }
}
