using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    [Flags]
    public enum ColorWriteChannels
    {
        None = 0,
        Alpha = 1,
        Blue = 2,
        Green = 4,
        Red = 8,
        All = 255
    }
}
