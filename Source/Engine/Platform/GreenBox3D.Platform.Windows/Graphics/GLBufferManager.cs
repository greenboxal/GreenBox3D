using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GLBufferManager : BufferManager
    {
        private WindowsGraphicsDevice _graphicsDevice;

        public GLBufferManager(WindowsGraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public override IIndexBuffer CreateIndexBuffer(IndexElementSize indexElementSize, int indexCount, BufferUsage usage)
        {
            return new GLIndexBuffer(_graphicsDevice, indexElementSize, indexCount, usage);
        }

        public override IVertexBuffer CreateVertexBuffer(VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage usage)
        {
            return new GLVertexBuffer(_graphicsDevice, vertexDeclaration, vertexCount, usage);
        }

        public override object CreateVertexDeclarationImplementation(VertexDeclaration vertexDeclaration)
        {
            return new VertexDeclarationImplementation(_graphicsDevice, vertexDeclaration);
        }
    }
}
