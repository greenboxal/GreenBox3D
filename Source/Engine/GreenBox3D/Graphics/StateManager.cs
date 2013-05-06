using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public abstract class StateManager
    {
        // SamplerState
        public abstract SamplerState SamplerLinearClamp { get; }
        public abstract SamplerState SamplerLinearWrap { get; }
        public abstract SamplerState SamplerPointClamp { get; }
        public abstract SamplerState SamplerPointWrap { get; }
        public abstract SamplerState CreateSamplerState();

        // BlendState
        public abstract BlendState BlendAdditive { get; }
        public abstract BlendState BlendAlphaBlend { get; }
        public abstract BlendState BlendNonPremultiplied { get; }
        public abstract BlendState BlendOpaque { get; }
        public abstract BlendState CreateBlendState();

        // RasterizerState
        public abstract RasterizerState RasterizerCullClockwise { get; }
        public abstract RasterizerState RasterizerCullCounterClockwise { get; }
        public abstract RasterizerState RasterizerCullNone { get; }
        public abstract RasterizerState CreateRasterizerState();
    }
}
