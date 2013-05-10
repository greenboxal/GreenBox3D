// InputManager.cs
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
using System.Windows.Forms;
using GreenBox3D.Input;

namespace GreenBox3D.Platform.Windows.Input
{
    public class InputManager : IInputManager, IMouse, IKeyboard
    {
        private readonly List<IKeyboardFilter> _keyboardFilters;
        private readonly List<IMouseFilter> _mouseFilters;

        public InputManager()
        {
            _mouseFilters = new List<IMouseFilter>();
            _keyboardFilters = new List<IKeyboardFilter>();
        }

        public void Initialize(WindowsGameWindow window)
        {
            window.MouseDown += window_MouseDown;
            window.MouseUp += window_MouseUp;
            window.MouseMove += window_MouseMove;
            window.MouseWheel += window_MouseWheel;
            window.KeyUp += window_KeyUp;
            window.KeyDown += window_KeyDown;
            window.KeyPress += window_KeyPress;
        }

        public void RegisterMouseFilter(IMouseFilter filter)
        {
            lock (_mouseFilters)
                _mouseFilters.Add(filter);
        }

        public void UnregisterMouseFilter(IMouseFilter filter)
        {
            lock (_mouseFilters)
                _mouseFilters.Remove(filter);
        }

        public void RegisterKeyboardFilter(IKeyboardFilter filter)
        {
            lock (_keyboardFilters)
                _keyboardFilters.Add(filter);
        }

        public void UnregisterKeyboardFilter(IKeyboardFilter filter)
        {
            lock (_keyboardFilters)
                _keyboardFilters.Remove(filter);
        }

        MouseState IMouse.GetState()
        {
            return new MouseState();
        }

        void IMouse.SetPosition(int x, int y)
        {
            NativeMethods.SetCursorPos(x, y);
        }

        KeyboardState IKeyboard.GetState()
        {
            return new KeyboardState();
        }

        private void window_MouseUp(object sender, MouseEventArgs e)
        {
            MouseButton button;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    button = MouseButton.Left;
                    break;
                case MouseButtons.Middle:
                    button = MouseButton.Middle;
                    break;
                case MouseButtons.Right:
                    button = MouseButton.Right;
                    break;
                case MouseButtons.XButton1:
                    button = MouseButton.XButton1;
                    break;
                case MouseButtons.XButton2:
                    button = MouseButton.XButton2;
                    break;
                default:
                    return;
            }

            foreach (IMouseFilter filter in _mouseFilters)
                if (filter.OnMouseUp(button, e.X, e.Y))
                    break;
        }

        private void window_MouseDown(object sender, MouseEventArgs e)
        {
            MouseButton button;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    button = MouseButton.Left;
                    break;
                case MouseButtons.Middle:
                    button = MouseButton.Middle;
                    break;
                case MouseButtons.Right:
                    button = MouseButton.Right;
                    break;
                case MouseButtons.XButton1:
                    button = MouseButton.XButton1;
                    break;
                case MouseButtons.XButton2:
                    button = MouseButton.XButton2;
                    break;
                default:
                    return;
            }

            foreach (IMouseFilter filter in _mouseFilters)
                if (filter.OnMouseDown(button, e.X, e.Y))
                    break;
        }

        private void window_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (IMouseFilter filter in _mouseFilters)
                if (filter.OnMouseMove(e.X, e.Y))
                    break;
        }

        private void window_MouseWheel(object sender, MouseEventArgs e)
        {
            foreach (IMouseFilter filter in _mouseFilters)
                if (filter.OnMouseWheel(e.Delta, e.X, e.Y))
                    break;
        }

        private void window_KeyUp(object sender, KeyEventArgs e)
        {
            KeyModifiers mods = KeyModifiers.None;

            if (e.Shift)
                mods |= KeyModifiers.Shift;

            if (e.Control)
                mods |= KeyModifiers.Control;

            if (e.Alt)
                mods |= KeyModifiers.Alt;

            foreach (IKeyboardFilter filter in _keyboardFilters)
                if (filter.OnKeyUp((GreenBox3D.Input.Keys)e.KeyCode, mods))
                    break;

            e.Handled = true;
        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            KeyModifiers mods = KeyModifiers.None;

            if (e.Shift)
                mods |= KeyModifiers.Shift;

            if (e.Control)
                mods |= KeyModifiers.Control;

            if (e.Alt)
                mods |= KeyModifiers.Alt;

            foreach (IKeyboardFilter filter in _keyboardFilters)
                if (filter.OnKeyDown((GreenBox3D.Input.Keys)e.KeyCode, mods))
                    break;

            e.Handled = true;
        }

        private void window_KeyPress(object sender, KeyPressEventArgs e)
        {
            foreach (IKeyboardFilter filter in _keyboardFilters)
                if (filter.OnKeyChar(e.KeyChar))
                    break;

            e.Handled = true;
        }
    }
}
