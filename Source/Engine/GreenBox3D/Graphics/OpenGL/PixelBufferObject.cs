using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics.OpenGL
{
    public sealed class PixelBufferObject : HardwareBuffer
    {
        public PixelBufferObject(BufferUsage usage)
            : base(BufferTarget.PixelUnpackBuffer, usage)
        {
        }
    }
}
