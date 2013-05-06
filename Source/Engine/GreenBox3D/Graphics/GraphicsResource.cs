﻿// GraphicsResource.cs
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
    public class GraphicsResource
    {
        #region Fields

        private readonly GraphicsDevice _graphicsDevice;
        private bool _disposed;

        #endregion

        #region Constructors and Destructors

        protected GraphicsResource()
            : this(GraphicsDevice.ActiveDevice)
        {
            
        }

        protected GraphicsResource(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        ~GraphicsResource()
        {
            if (!_graphicsDevice.IsDisposed)
                Dispose(false);
        }

        #endregion

        #region Public Events

        public event EventHandler<EventArgs> Disposing;

        #endregion

        #region Public Properties

        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            // Dispose of managed objects as well
            if (!_graphicsDevice.IsDisposed)
                Dispose(true);

            // Since we have been manually disposed, do not call the finalizer on this object
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The method that derived classes should override to implement disposing of managed and native resources.
        /// </summary>
        /// <param name="disposing">True if managed objects should be disposed.</param>
        /// <remarks>Native resources should always be released regardless of the value of the disposing parameter.</remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            // Do not trigger the event if called from the finalizer
            if (disposing && Disposing != null)
                Disposing(this, EventArgs.Empty);

            _disposed = true;
        }

        #endregion
    }
}
