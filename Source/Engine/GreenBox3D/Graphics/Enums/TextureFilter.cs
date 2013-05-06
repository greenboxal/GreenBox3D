using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public enum TextureFilter
    {
        Point,
        Linear,
        LinearMipPoint,
        PointMipLinear,
        MinLinearMagPointMipLinear,
        MinLinearMagPointMipPoint,
        MinPointMagLinearMipLinear,
        MinPointMagLinearMipPoint
    }
}
