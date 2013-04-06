using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.Graphics.Shading.Ast
{
    public class Pass
    {
        public string VertexGlsl { get; set; }
        public string PixelGlsl { get; set; }

        public string VertexHlsl { get; set; }
        public string PixelHlsl { get; set; }
    }
}
