// WindowsGameWindow.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GreenBox3D.Graphics;
using OpenTK.Platform.Windows;

namespace GreenBox3D.Platform.Windows
{
    public class WindowsGameWindow : Form, IInternalGameWindow
    {
        private readonly WindowsGamePlatform _platform;
        private readonly PresentationParameters _creationParameters;

        private bool _isCursorVisible;
        private bool _allowUserResizing;

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public bool AllowUserResizing
        {
            get { return _allowUserResizing; }
            set
            {
                _allowUserResizing = value;
                UpdateBorderStyle();
            }
        }

        public Rectangle ClientBounds
        {
            get { return new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height); }
        }

        public bool IsCursorVisible
        {
            get { return _isCursorVisible; }
            set
            {
                if (value != _isCursorVisible)
                {
                    _isCursorVisible = value;
                    Cursor = _isCursorVisible ? Cursors.Arrow : null;
                }
            }
        }

        public WindowsGameWindow(WindowsGamePlatform platform, PresentationParameters parameters)
        {
            _platform = platform;
            _creationParameters = parameters;

            Create();
        }

        public new void Resize(int width, int height)
        {
            ClientSize = new System.Drawing.Size(width, height);
        }

        public void HandleEvents()
        {
            Application.DoEvents();
        }

        private void Create()
        {
            if (_creationParameters.IsFullScreen)
                throw new NotSupportedException();

            SuspendLayout();
            CausesValidation = false;
            ClientSize = new System.Drawing.Size(_creationParameters.BackBufferWidth, _creationParameters.BackBufferHeight);
            Name = "GreenBox3D";
            Text = "GreenBox3D";
            ResumeLayout(false);

            SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            UpdateBorderStyle();

            Show();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= (int)(NativeMethods.ClassStyles.OwnDC | NativeMethods.ClassStyles.VerticalRedraw | NativeMethods.ClassStyles.HorizontalRedraw);
                return cp;
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            _platform.SetActive(true);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            _platform.SetActive(false);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _platform.Exit();
        }

        private void UpdateBorderStyle()
        {
            if (_allowUserResizing)
            {
                MaximizeBox = true;
                FormBorderStyle = FormBorderStyle.Sizable;
            }
            else
            {
                MaximizeBox = false;
                FormBorderStyle = FormBorderStyle.FixedSingle;
            }
        }

        public OpenTK.Platform.IWindowInfo WindowInfo
        {
            get { return new WinWindowInfo(Handle, null); }
        }
    }
}
