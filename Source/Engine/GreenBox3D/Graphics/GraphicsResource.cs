// GraphicsResource.cs
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
using GreenBox3D.Content;

namespace GreenBox3D.Graphics
{
    public class GraphicsResource : IDisposableContent
    {
        private readonly GraphicsDevice _graphicsDevice;
        private bool _disposed;
        
        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        public event EventHandler<EventArgs> Disposing;

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
            DoDispose(false);
        }

        public void Dispose()
        {
            DoDispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing && Disposing != null)
                Disposing(this, EventArgs.Empty);

            _disposed = true;
        }

        private void DoDispose(bool disposing)
        {
            GraphicsDevice.Owner.Dispatcher.RunOnGraphicThread(GraphicsDevice, () => Dispose(disposing));
        }
    }
}
