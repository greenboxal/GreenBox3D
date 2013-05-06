using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public abstract class BlendState : GraphicsResource
    {
        public static BlendState Additive { get { return GraphicsDevice.ActiveDevice.StateManager.BlendAdditive; } }
        public static BlendState AlphaBlend { get { return GraphicsDevice.ActiveDevice.StateManager.BlendAlphaBlend; } }
        public static BlendState NonPremultiplied { get { return GraphicsDevice.ActiveDevice.StateManager.BlendNonPremultiplied; } }
        public static BlendState Opaque { get { return GraphicsDevice.ActiveDevice.StateManager.BlendOpaque; } }

        protected BlendState(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }

        public abstract BlendFunction AlphaBlendFunction { get; set; }
        public abstract Blend AlphaDestinationBlend { get; set; }
        public abstract Blend AlphaSourceBlend { get; set; }
        public abstract Color BlendFactor { get; set; }
        public abstract BlendFunction ColorBlendFunction { get; set; }
        public abstract Blend ColorDestinationBlend { get; set; }
        public abstract Blend ColorSourceBlend { get; set; }
        public abstract ColorWriteChannels ColorWriteChannels { get; set; }
        public abstract int MultiSampleMask { get; set; }
    }
}
