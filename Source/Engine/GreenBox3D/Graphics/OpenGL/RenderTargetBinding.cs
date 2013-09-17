using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics.OpenGL
{
    public struct RenderTargetBinding
    {
        private readonly Texture _texture;
        private readonly CubeMapFace _cubeMapFace;

        public Texture RenderTarget { get { return _texture; } }
        public CubeMapFace CubeMapFace { get { return _cubeMapFace; } }

        public RenderTargetBinding(RenderTarget2D target)
        {
            _texture = target;
            _cubeMapFace = CubeMapFace.PositiveZ;
        }
    }
}
