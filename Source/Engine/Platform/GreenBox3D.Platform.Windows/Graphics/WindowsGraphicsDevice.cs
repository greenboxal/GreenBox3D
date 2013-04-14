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
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using GreenBox3D.Platform.Windows.Graphics.Shading;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class WindowsGraphicsDevice : GraphicsDevice
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(WindowsGraphicsDevice));
        private readonly BufferManager _bufferManager;
        private readonly Dictionary<int, GraphicsContext> _contexts;

        private readonly GraphicsMode _graphicsMode;
        private readonly GraphicsContext _mainContext;
        private readonly WindowsGamePlatform _platform;
        private readonly PresentationParameters _presentationParameters;
        private readonly ShaderManager _shaderManager;
        private readonly TextureCollection _textures;
        private readonly WindowsGameWindow _window;

        internal GLShader ActiveShader;
        private GLIndexBuffer _indices;
        private bool _indicesDirty;
        private GLVertexBuffer _vertices;
        private bool _verticesDirty;
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
                int r = 8, g = 8, b = 8, a = 8;
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

                _graphicsMode = new GraphicsMode(new ColorFormat(r, g, b, a), depth, stencil,
                                                 parameters.MultiSampleCount);
            }

            _mainContext = CreateNewContext(Thread.CurrentThread.ManagedThreadId);
            _mainContext.MakeCurrent(new OpenTK.Platform.Windows.WinWindowInfo(_window.NativeHandle, null));
            _mainContext.LoadAll();

            Log.Message("OpenGL context acquired: {0}", _mainContext);

            _textures = new GLTextureCollection();
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

        public override TextureCollection Textures
        {
            get { return _textures; }
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

        public override IIndexBuffer Indices
        {
            get { return _indices; }
            set
            {
                if (_indices != value)
                {
                    _indicesDirty = true;
                    _indices = value as GLIndexBuffer;
                }
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

        public override void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int numVertices,
                                                   int startIndex, int primitiveCount)
        {
            if (Indices == null)
                throw new InvalidOperationException("Indices must be set before calling this method");

            if (ActiveShader == null)
                throw new InvalidOperationException("An Effect must be applied before calling this method");

            SetRenderingState();
            (_vertices.VertexDeclaration.GetImplementation(this) as VertexDeclarationImplementation).Bind(IntPtr.Zero);

            var indexOffsetInBytes = (IntPtr)(startIndex * _indices.ElementSize);
            var indexElementCount = GetElementCountArray(primitiveType, primitiveCount);
            var target = GetBeginMode(primitiveType);

            GL.DrawElementsBaseVertex(target, indexElementCount, _indices.DrawElementsType, indexOffsetInBytes,
                                      baseVertex);
        }

        public override void DrawPrimitives(PrimitiveType primitiveType, int startVertex, int primitiveCount)
        {
            if (_vertices == null)
                throw new InvalidOperationException("SetVertexBuffer must be called before calling this method");

            if (ActiveShader == null)
                throw new InvalidOperationException("An Effect must be applied before calling this method");

            SetRenderingState();
            (_vertices.VertexDeclaration.GetImplementation(this) as VertexDeclarationImplementation).Bind(IntPtr.Zero);

            GL.DrawArrays(GetBeginMode(primitiveType), startVertex, primitiveCount);
        }

        public override void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset,
                                                          int vertexCount, int[] indexData, int indexOffset,
                                                          int primitiveCount)
        {
            if (ActiveShader == null)
                throw new InvalidOperationException("An Effect must be applied before calling this method");

            VertexDeclaration vertexDeclaration = VertexDeclaration.FromType(typeof(T));
            SetRenderingState(false, false);

            GCHandle handle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
            IntPtr arrayStart = Marshal.UnsafeAddrOfPinnedArrayElement(vertexData, 0);

            if (vertexOffset > 0)
                arrayStart = new IntPtr(arrayStart.ToInt32() + (vertexOffset * vertexDeclaration.VertexStride));

            (vertexDeclaration.GetImplementation(this) as VertexDeclarationImplementation).Bind(arrayStart);

            unsafe
            {
                fixed (int* indicesPtr = &indexData[0])
                    GL.DrawElementsBaseVertex(GetBeginMode(primitiveType), primitiveCount, DrawElementsType.UnsignedInt,
                                              (IntPtr)(&indicesPtr[indexOffset]), vertexOffset);
            }

            handle.Free();
        }

        public override void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset,
                                                   int primitiveCount)
        {
            if (ActiveShader == null)
                throw new InvalidOperationException("An Effect must be applied before calling this method");

            VertexDeclaration vertexDeclaration = VertexDeclaration.FromType(typeof(T));
            SetRenderingState(false, false);

            GCHandle handle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
            IntPtr arrayStart = Marshal.UnsafeAddrOfPinnedArrayElement(vertexData, 0);

            if (vertexOffset > 0)
                arrayStart = new IntPtr(arrayStart.ToInt32() + (vertexOffset * vertexDeclaration.VertexStride));

            (vertexDeclaration.GetImplementation(this) as VertexDeclarationImplementation).Bind(arrayStart);

            GL.DrawArrays(GetBeginMode(primitiveType), vertexOffset, primitiveCount);

            handle.Free();
        }

        public override void SetVertexBuffer(IVertexBuffer vertexBuffer)
        {
            if (_vertices != vertexBuffer)
            {
                _vertices = vertexBuffer as GLVertexBuffer;
                _verticesDirty = true;
            }
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

        private static int GetElementCountArray(PrimitiveType primitiveType, int primitiveCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.LineList:
                    return primitiveCount * 2;
                case PrimitiveType.LineStrip:
                    return primitiveCount + 1;
                case PrimitiveType.TriangleList:
                    return primitiveCount * 3;
                case PrimitiveType.TriangleStrip:
                    return 3 + (primitiveCount - 1);
                default:
                    throw new NotSupportedException();
            }
        }

        private void SetRenderingState(bool setIndex = true, bool setVertex = true)
        {
            if (setIndex && _indicesDirty)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indices.BufferID);
                _indicesDirty = false;
            }
            else
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            if (setVertex && _verticesDirty)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertices.BufferID);
                _verticesDirty = false;
            }
            else
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
