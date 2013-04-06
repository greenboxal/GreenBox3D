// GreenBox3D
// 
// Copyright (c) 2013 The GreenBox Development Inc.
// Copyright (c) 2013 Mono.Xna Team and Contributors
// 
// Licensed under MIT license terms.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public class PresentationParameters : IDisposable
    {
        #region Constants

        public const int DefaultPresentRate = 60;

        #endregion

        #region Fields

        private SurfaceFormat _backBufferFormat;
        private int _backBufferHeight;
        private int _backBufferWidth;
        private DepthFormat _depthStencilFormat;
        private bool _disposed;
        private int _multiSampleCount;

        #endregion

        #region Constructors and Destructors

        public PresentationParameters()
        {
            Clear();
        }

        ~PresentationParameters()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public SurfaceFormat BackBufferFormat { get { return _backBufferFormat; } set { _backBufferFormat = value; } }
        public int BackBufferHeight { get { return _backBufferHeight; } set { _backBufferHeight = value; } }
        public int BackBufferWidth { get { return _backBufferWidth; } set { _backBufferWidth = value; } }
        public DepthFormat DepthStencilFormat { get { return _depthStencilFormat; } set { _depthStencilFormat = value; } }
        public bool IsFullScreen { get; set; }
        public int MultiSampleCount { get { return _multiSampleCount; } set { _multiSampleCount = value; } }

        #endregion

        #region Public Methods and Operators

        public void Clear()
        {
            _backBufferFormat = SurfaceFormat.Color;
            _backBufferWidth = 1024;
            _backBufferHeight = 768;
            _depthStencilFormat = DepthFormat.None;
            _multiSampleCount = 0;
        }

        public PresentationParameters Clone()
        {
            PresentationParameters clone = new PresentationParameters();
            clone._backBufferFormat = _backBufferFormat;
            clone._backBufferHeight = _backBufferHeight;
            clone._backBufferWidth = _backBufferWidth;
            clone._disposed = _disposed;
            clone.IsFullScreen = IsFullScreen;
            clone._depthStencilFormat = _depthStencilFormat;
            clone._multiSampleCount = _multiSampleCount;
            return clone;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                _disposed = true;
        }

        #endregion
    }
}