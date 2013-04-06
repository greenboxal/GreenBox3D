using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.Graphics.Shading.Ast
{
    public class InputVariable : Variable
    {
        public string Usage { get; set; }
        public int UsageIndex { get; set; }
    }
}
