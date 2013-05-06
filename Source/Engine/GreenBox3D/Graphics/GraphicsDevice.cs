// GraphicsDevice.cs
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
    public abstract class GraphicsDevice : IDisposable
    {
        [ThreadStatic]
        private static GraphicsDevice _currentDevice;

        public static GraphicsDevice ActiveDevice
        {
            get { return _currentDevice; }
        }

        public abstract StateManager StateManager { get; }
        public abstract BufferManager BufferManager { get; }
        public abstract ShaderManager ShaderManager { get; }
        public abstract TextureManager TextureManager { get; }
        public abstract TextureCollection Textures { get; }
        public abstract SamplerStateCollection SamplerStates { get; }

        public abstract PresentationParameters PresentationParameters { get; }
        public abstract Viewport Viewport { get; set; }
        public abstract bool IsDisposed { get; }

        public void MakeCurrent()
        {
            if (MakeCurrentInternal())
                _currentDevice = this;
        }

        protected abstract bool MakeCurrentInternal();

        public virtual void Clear(Color color)
        {
            Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil | ClearOptions.Target, color);
        }

        public virtual void Clear(ClearOptions options)
        {
            Clear(options, Color.Black);
        }

        public abstract void Clear(ClearOptions options, Color color);

        public abstract void SetVertexBuffer(IVertexBuffer vertexBuffer);

        public abstract void SetIndexBuffer(IIndexBuffer indexBuffer);

        public abstract void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex,
                                                   int startIndex, int numVertices);

        public abstract void DrawPrimitives(PrimitiveType primitiveType, int startVertex, int numVertices);

        public abstract void Present();

        public abstract void Dispose();
    }
}
