using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public sealed class PixelBuffer : HardwareBuffer
    {
        public PixelBuffer(BufferUsage usage)
            : base(BufferTarget.PixelUnpackBuffer, usage)
        {
        }
    }
}
