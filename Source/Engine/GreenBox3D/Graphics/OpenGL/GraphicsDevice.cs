// GraphicsDevice.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
#if DESKTOP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenBox3D.Platform;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;

namespace GreenBox3D.Graphics
{
    public sealed class GraphicsDevice : IDisposable
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(GraphicsDevice));

        [ThreadStatic]
        private static GraphicsDevice _currentDevice;

        public static GraphicsDevice ActiveDevice
        {
            get { return _currentDevice; }
        }

        internal Shader ActiveShader;
        internal readonly SamplerState SamplerPointWrap;
        internal readonly SamplerState SamplerPointClamp;
        internal readonly SamplerState SamplerLinearWrap;
        internal readonly SamplerState SamplerLinearClamp;

        private readonly GraphicsMode _graphicsMode;
        private readonly IInternalGameWindow _window;

        internal readonly GraphicsContext MainContext;
        private readonly Dictionary<int, GraphicsContext> _contexts;

        private readonly TextureCollection _textures;
        private readonly SamplerStateCollection _samplers;

        private BlendState _blendState;
        private RasterizerState _rasterizerState;

        private PresentationParameters _presentationParameters;
        private Viewport _viewport;
        private bool _vsync;

        private bool _disposed;
        private IndexBuffer _indices;
        private bool _indicesDirty;
        private VertexBuffer _vertices;
        private bool _verticesDirty;

        public TextureCollection Textures { get { return _textures; } }
        public SamplerStateCollection SamplerStates { get { return _samplers; } }

        public BlendState BlendState
        {
            get { return _blendState; }
            set
            {
                bool doEnable = _blendState == null;

                if (_blendState != value)
                {
                    _blendState = value;

                    if (_blendState == null)
                    {
                        GL.Disable(EnableCap.Blend);
                        return;
                    }

                    if (doEnable)
                        GL.Enable(EnableCap.Blend);

                    _blendState.Bond();
                    _blendState.ApplyState();
                }
            }
        }

        public RasterizerState RasterizerState
        {
            get { return _rasterizerState; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_rasterizerState != value)
                {
                    _rasterizerState = value;
                    _rasterizerState.Bond();
                    _rasterizerState.ApplyState();
                }
            }
        }

        public PresentationParameters PresentationParameters
        {
            get { return _presentationParameters; }
        }

        public Viewport Viewport
        {
            get { return _viewport; }
            set
            {
                _viewport = value;
                GL.Viewport(value.X, value.Y, value.Width,
                            value.Height);
                GL.DepthRange(value.MinDepth, value.MaxDepth);
            }
        }

        public bool VSync
        {
            get { return _vsync; }
            set
            {
                if (_vsync != value)
                {
                    if (value)
                        MainContext.SwapInterval = -1;
                    else
                        MainContext.SwapInterval = 0;

                    _vsync = value;
                }
            }
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        internal GraphicsDevice(PresentationParameters parameters,
                                     IInternalGameWindow window)
        {
            _window = window;
            _presentationParameters = parameters;
            _contexts = new Dictionary<int, GraphicsContext>();

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

            MainContext = CreateNewContext(Thread.CurrentThread.ManagedThreadId);
            MakeCurrent();
            MainContext.LoadAll();

            Log.Message("OpenGL context acquired: {0}", MainContext);

            SamplerLinearClamp = new SamplerState
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = TextureFilter.Linear
            };
            SamplerLinearClamp.Bond(this);

            SamplerLinearWrap = new SamplerState
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = TextureFilter.Linear
            };
            SamplerLinearWrap.Bond(this);

            SamplerPointClamp = new SamplerState
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = TextureFilter.Point
            };
            SamplerPointClamp.Bond(this);

            SamplerPointWrap = new SamplerState
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = TextureFilter.Point
            };
            SamplerPointWrap.Bond(this);
            
            _textures = new TextureCollection();
            _samplers = new SamplerStateCollection(this);

            BlendState = BlendState.Opaque;
            RasterizerState = RasterizerState.CullNone;
            Viewport = new Viewport(0, 0, _presentationParameters.BackBufferWidth,
                                    _presentationParameters.BackBufferHeight);
        }

        public void MakeCurrent()
        {
            if (MakeCurrentInternal())
                _currentDevice = this;
        }

        public void Clear(ClearOptions options, Color color)
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

        public void Clear(Color color)
        {
            Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil | ClearOptions.Target, color);
        }

        public void Clear(ClearOptions options)
        {
            Clear(options, Color.Black);
        }

        public void SetVertexBuffer(VertexBuffer vertexBuffer)
        {
            if (_vertices != vertexBuffer)
            {
                _vertices = vertexBuffer;
                _verticesDirty = true;
            }
        }

        public void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            if (_indices != indexBuffer)
            {
                _indices = indexBuffer;
                _indicesDirty = true;
            }
        }

        public void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex,
                                                   int startIndex, int numVertices)
        {
            IntPtr indexOffsetInBytes = (IntPtr)(startIndex * _indices.ElementSize);

            if (_vertices == null)
                throw new InvalidOperationException("A VertexBuffer must be set before calling this method");

            if (_indices == null)
                throw new InvalidOperationException("An IndexBuffer must be set before calling this method");

            if (ActiveShader == null)
                throw new InvalidOperationException("An Effect must be applied before calling this method");

            SetRenderingState();
            _vertices.VertexDeclaration.Bind(IntPtr.Zero);

            GL.DrawElementsBaseVertex(GetBeginMode(primitiveType), numVertices, _indices.DrawElementsType,
                                      indexOffsetInBytes,
                                      baseVertex);
        }

        public void DrawPrimitives(PrimitiveType primitiveType, int startVertex, int numVertices)
        {
            if (_vertices == null)
                throw new InvalidOperationException("A VertexBuffer must be set before calling this method");

            if (_indices == null)
                throw new InvalidOperationException("An IndexBuffer must be set before calling this method");

            if (ActiveShader == null)
                throw new InvalidOperationException("An Effect must be applied before calling this method");

            SetRenderingState();
            _vertices.VertexDeclaration.Bind(IntPtr.Zero);

            GL.DrawArrays(GetBeginMode(primitiveType), startVertex, numVertices);
        }

        public void Present()
        {
            MainContext.SwapBuffers();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                MainContext.Dispose();

                lock (_contexts)
                {
                    foreach (GraphicsContext context in _contexts.Values)
                        context.Dispose();

                    _contexts.Clear();
                }

                _disposed = true;
            }
        }

        internal void SetPresentationParameters(PresentationParameters presentationParameters)
        {
            _presentationParameters = presentationParameters;
        }

        private GraphicsContext CreateNewContext(int threadId)
        {
            GraphicsContext context = new GraphicsContext(_graphicsMode, _window.WindowInfo, 4, 2, GraphicsContextFlags.Default);

            _contexts[threadId] = context;

            return context;
        }

        private void SetRenderingState()
        {
            _textures.Apply();
            _samplers.Apply();

            if (_indicesDirty)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indices.BufferID);
                _indicesDirty = false;
            }

            if (_verticesDirty)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertices.BufferID);
                _verticesDirty = false;
            }
        }

        private bool MakeCurrentInternal()
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
                context.MakeCurrent(_window.WindowInfo);
            }
            catch
            {
                Log.Error("Error setting current context for thread {0}", Thread.CurrentThread.ManagedThreadId);
                return false;
            }

            return true;
        }

        private static BeginMode GetBeginMode(PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineList:
                    return BeginMode.LineLoop;
                case PrimitiveType.LineStrip:
                    return BeginMode.LineStrip;
                case PrimitiveType.TriangleList:
                    return BeginMode.TriangleFan;
                case PrimitiveType.TriangleStrip:
                    return BeginMode.TriangleStrip;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

#endif
