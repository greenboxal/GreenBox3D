using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GreenBox3D.Graphics.Detail;

namespace GreenBox3D.Graphics
{
    public abstract class ShaderManager
    {
        public abstract IShader CreateShader(CompiledShader compiledShader);
    }
}
