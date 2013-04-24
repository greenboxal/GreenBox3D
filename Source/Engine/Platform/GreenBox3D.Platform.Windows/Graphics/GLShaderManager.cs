using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GLShaderManager : ShaderManager
    {
        private readonly WindowsGraphicsDevice _graphicsDevice;

        public GLShaderManager(WindowsGraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public override IShader CreateShader(CompiledShader compiledShader)
        {
            return new Shader(_graphicsDevice, compiledShader);
        }
    }
}
