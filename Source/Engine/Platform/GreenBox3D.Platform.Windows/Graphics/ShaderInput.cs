using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class ShaderInput
    {
        public string Name { get; private set; }
        public VertexElementUsage Usage { get; private set; }
        public int UsageIndex { get; private set; }
        public int Index { get; private set; }

        public ShaderInput(int programID, CompiledInputVariable variable)
        {
            Name = variable.Name;
            Usage = variable.Usage;
            UsageIndex = variable.UsageIndex;
            Index = GL.GetAttribLocation(programID, variable.Name);
        }
    }
}
