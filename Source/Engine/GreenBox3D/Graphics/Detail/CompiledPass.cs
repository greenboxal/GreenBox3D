using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics.Detail
{
    public class CompiledPass
    {
        public string GlslVertexCode { get; private set; }
        public string GlslPixelCode { get; private set; }
        public string HlslVertexCode { get; private set; }
        public string HlslPixelCode { get; private set; }

        public CompiledPass(string glslVertexCode, string glslPixelCode, string hlslVertexCode, string hlslPixelCode)
        {
            GlslVertexCode = glslVertexCode;
            GlslPixelCode = glslPixelCode;
            HlslVertexCode = hlslVertexCode;
            HlslPixelCode = hlslPixelCode;
        }
    }
}
