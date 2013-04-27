using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D
{
    public interface IPackedVector
    {
        Vector4 ToVector4();
        void LoadFromVector4(Vector4 value);
    }
}
