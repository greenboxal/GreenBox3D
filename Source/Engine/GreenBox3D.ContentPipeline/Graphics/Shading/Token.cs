using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.Graphics.Shading
{
    public class Token
    {
        public TokenType Type;
        public int Line, Column;
        public int EndLine, EndColumn;
        public string File;
        public string Text;
    }
}
