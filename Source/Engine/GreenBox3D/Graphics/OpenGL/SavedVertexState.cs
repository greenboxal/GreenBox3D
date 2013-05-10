using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public sealed class SavedVertexState : GraphicsResource
    {
        internal int ArrayID;

        internal IndexBuffer IndexBuffer;

        public SavedVertexState()
        {
            ArrayID = GL.GenVertexArray();
        }

        public void Bind()
        {
            GL.BindVertexArray(ArrayID);
            GraphicsDevice.State.ActiveVertexState = this;
            GraphicsDevice.State.ActiveIndexBuffer = IndexBuffer;
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
            GraphicsDevice.State.ActiveVertexState = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (ArrayID != 0)
            {
                GL.DeleteVertexArray(ArrayID);
                ArrayID = 0;
            }

            base.Dispose(disposing);
        }
    }
}
