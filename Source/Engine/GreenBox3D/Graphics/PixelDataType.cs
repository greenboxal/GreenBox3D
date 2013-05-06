using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public class PixelDataType
    {
        public PixelFormat Format { get; private set; }
        public PixelType Type { get; private set; }

        public PixelDataType(PixelFormat format, PixelType type)
        {
            Format = format;
            Type = type;
        }
    }
}
