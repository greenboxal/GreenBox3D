using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public abstract class SamplerState : GraphicsResource
    {
        protected SamplerState(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }

        public abstract TextureAddressMode AddressU { get; set; }
        public abstract TextureAddressMode AddressV { get; set; }
        public abstract TextureAddressMode AddressW { get; set; }
        public abstract TextureFilter Filter { get; set; }
        public abstract int MaxAnisotropy { get; set; }
        public abstract int MaxMipLevel { get; set; }
        public abstract float MipMapLevelOfDetailBias { get; set; }
    }
}
