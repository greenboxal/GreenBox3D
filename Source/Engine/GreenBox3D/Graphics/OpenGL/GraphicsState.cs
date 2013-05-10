using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    internal class GraphicsState
    {
        public Shader ActiveShader;
        public IndexBuffer ActiveIndexBuffer;
        public SavedVertexState ActiveVertexState;
    }
}
