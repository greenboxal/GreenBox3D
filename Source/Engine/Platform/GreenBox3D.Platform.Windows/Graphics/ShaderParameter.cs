using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class ShaderParameter : IShaderParameter
    {
        private readonly string _name;
        private readonly int _location;

        public string Name
        {
            get { return _name; }
        }

        public ShaderParameter(int programID, int index)
        {
            int size;
            ActiveUniformType type;

            _name = GL.GetActiveUniform(programID, index, out size, out type);
            _location = GL.GetUniformLocation(programID, _name);
        }
    }
}
