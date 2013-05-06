// PresentationParameters.cs
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
    public class PresentationParameters : IDisposable
    {
        #region Constants

        public const int DefaultPresentRate = 60;

        #endregion

        #region Fields

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

        public int BackBufferHeight
        {
            get { return _backBufferHeight; }
            set { _backBufferHeight = value; }
        }

        public int BackBufferWidth
        {
            get { return _backBufferWidth; }
            set { _backBufferWidth = value; }
        }

        public DepthFormat DepthStencilFormat
        {
            get { return _depthStencilFormat; }
            set { _depthStencilFormat = value; }
        }

        public bool IsFullScreen { get; set; }

        public int MultiSampleCount
        {
            get { return _multiSampleCount; }
            set { _multiSampleCount = value; }
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Clear()
        {
            _backBufferWidth = 1024;
            _backBufferHeight = 768;
            _depthStencilFormat = DepthFormat.None;
            _multiSampleCount = 0;
        }

        public PresentationParameters Clone()
        {
            PresentationParameters clone = new PresentationParameters();
            clone._backBufferHeight = _backBufferHeight;
            clone._backBufferWidth = _backBufferWidth;
            clone._disposed = _disposed;
            clone.IsFullScreen = IsFullScreen;
            clone._depthStencilFormat = _depthStencilFormat;
            clone._multiSampleCount = _multiSampleCount;
            return clone;
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
