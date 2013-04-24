// WindowsGraphicsDevice.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GreenBox3D.Graphics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class WindowsGraphicsDevice : GraphicsDevice
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(WindowsGraphicsDevice));

        internal Shader ActiveShader;

        private readonly BufferManager _bufferManager;
        private readonly Dictionary<int, GraphicsContext> _contexts;

        private readonly GraphicsMode _graphicsMode;
        private readonly GraphicsContext _mainContext;
        private readonly WindowsGamePlatform _platform;
        private readonly PresentationParameters _presentationParameters;
        private readonly ShaderManager _shaderManager;
        private readonly WindowsGameWindow _window;

        private Viewport _viewport;
        private bool _vsync;

        public WindowsGraphicsDevice(WindowsGamePlatform platform, PresentationParameters parameters,
                                     WindowsGameWindow window)
        {
            _platform = platform;
            _window = window;
            _presentationParameters = parameters;
            _contexts = new Dictionary<int, GraphicsContext>();
            _bufferManager = new GLBufferManager(this);
            _shaderManager = new GLShaderManager(this);

            if (parameters.BackBufferFormat != SurfaceFormat.Color)
                throw new NotSupportedException("PresentationParameters's BackBufferFormat must be SurfaceFormat.Color");

            GraphicsContext.ShareContexts = true;

            {
                int depth = 0, stencil = 0;

                switch (parameters.DepthStencilFormat)
                {
                    case DepthFormat.Depth16:
                        depth = 16;
                        stencil = 0;
                        break;
                    case DepthFormat.Depth24:
                        depth = 24;
                        stencil = 0;
                        break;
                    case DepthFormat.Depth24Stencil8:
                        depth = 24;
                        stencil = 8;
                        break;
                }

                _graphicsMode = new GraphicsMode(new ColorFormat(8, 8, 8, 8), depth, stencil,
                                                 parameters.MultiSampleCount);
            }

            _mainContext = CreateNewContext(Thread.CurrentThread.ManagedThreadId);
            _mainContext.MakeCurrent(new OpenTK.Platform.Windows.WinWindowInfo(_window.NativeHandle, null));
            _mainContext.LoadAll();

            Log.Message("OpenGL context acquired: {0}", _mainContext);
        }

        public bool VSync
        {
            get { return _vsync; }
            set
            {
                if (_vsync != value)
                {
                    if (value)
                        _mainContext.SwapInterval = -1;
                    else
                        _mainContext.SwapInterval = 0;

                    _vsync = value;
                }
            }
        }

        public override BufferManager BufferManager
        {
            get { return _bufferManager; }
        }

        public override ShaderManager ShaderManager
        {
            get { return _shaderManager; }
        }

        public override PresentationParameters PresentationParameters
        {
            get { return _presentationParameters; }
        }

        public override Viewport Viewport
        {
            get { return _viewport; }
            set
            {
                _viewport = value;

                // TODO: Reimplement after render target reimplementation
                //if (IsRenderTargetBound)
                //    GL.Viewport(value.X, value.Y, value.Width, value.Height);
                //else

                GL.Viewport(value.X, PresentationParameters.BackBufferHeight - value.Y - value.Height, value.Width,
                            value.Height);
                // GL.DepthRange(value.MinDepth, value.MaxDepth);
            }
        }

        protected override bool MakeCurrentInternal()
        {
            GraphicsContext context;

            if (!_contexts.TryGetValue(Thread.CurrentThread.ManagedThreadId, out context))
            {
                context = CreateNewContext(Thread.CurrentThread.ManagedThreadId);

                if (context == null)
                {
                    Log.Error("Error creating new context for thread {0}", Thread.CurrentThread.ManagedThreadId);
                    return false;
                }
            }

            try
            {
                // We use a hacked version of OpenTK which exposes WinWindowInfo as well IWindowInfo implementation for other platforms
                context.MakeCurrent(new OpenTK.Platform.Windows.WinWindowInfo(_window.NativeHandle, null));
            }
            catch
            {
                Log.Error("Error setting current context for thread {0}", Thread.CurrentThread.ManagedThreadId);
                return false;
            }

            return true;
        }

        public override void Clear(ClearOptions options, Color color)
        {
            ClearBufferMask mask = 0;

            if ((options & ClearOptions.DepthBuffer) != 0)
                mask |= ClearBufferMask.DepthBufferBit;

            if ((options & ClearOptions.Stencil) != 0)
                mask |= ClearBufferMask.StencilBufferBit;

            if ((options & ClearOptions.Target) != 0)
            {
                mask |= ClearBufferMask.ColorBufferBit;
                GL.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
            }

            GL.Clear(mask);
        }

        public override void Present()
        {
            _mainContext.SwapBuffers();
        }

        public override void Dispose()
        {
            _mainContext.Dispose();

            lock (_contexts)
            {
                foreach (GraphicsContext context in _contexts.Values)
                    context.Dispose();

                _contexts.Clear();
            }
        }

        private GraphicsContext CreateNewContext(int threadId)
        {
            OpenTK.Platform.Windows.WinWindowInfo window =
                new OpenTK.Platform.Windows.WinWindowInfo(_window.NativeHandle, null);
            GraphicsContext context = new GraphicsContext(_graphicsMode, window, 4, 2, GraphicsContextFlags.Default);

            _contexts[threadId] = context;

            return context;
        }
    }
}
