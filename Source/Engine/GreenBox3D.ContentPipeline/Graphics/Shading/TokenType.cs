using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.Graphics.Shading
{
    public enum TokenType
    {
        None,
        Error,

        Shader,
        Version,
        Input,
        Parameters,
        Globals,
        Passes,
        Pass,
        VertexGlsl,
        PixelGlsl,
        VertexHlsl,
        PixelHlsl,
        Fallback,

        Identifier,
        Number,
        String,

        LeftBrackets,
        RightBrackets,
        LeftSquare,
        RightSquare,
        Semicolon,
        Colon,

        Eof
    }
}
