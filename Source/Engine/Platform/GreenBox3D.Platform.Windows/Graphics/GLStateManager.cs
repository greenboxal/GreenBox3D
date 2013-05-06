using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public sealed class GLStateManager : StateManager
    {
        private readonly WindowsGraphicsDevice _graphicsDevice;
        private readonly GLSamplerState _samplerLinearClamp, _samplerLinearWrap, _samplerPointClamp, _samplerPointWrap;
        private readonly GLBlendState _blendAdditive, _blendAlphaBlend, _blendNonPremultiplied, _blendOpaque;
        private readonly GLRasterizerState _rasterizerCullClockwise, _rasterizerCullCounterClockwise, _rasterizerCullNone;

        public override SamplerState SamplerLinearClamp
        {
            get { return _samplerLinearClamp; }
        }

        public override SamplerState SamplerLinearWrap
        {
            get { return _samplerLinearWrap; }
        }

        public override SamplerState SamplerPointClamp
        {
            get { return _samplerPointClamp; }
        }

        public override SamplerState SamplerPointWrap
        {
            get { return _samplerPointWrap; }
        }

        public override BlendState BlendAdditive
        {
            get { return _blendAdditive; }
        }

        public override BlendState BlendAlphaBlend
        {
            get { return _blendAlphaBlend; }
        }

        public override BlendState BlendNonPremultiplied
        {
            get { return _blendNonPremultiplied; }
        }

        public override BlendState BlendOpaque
        {
            get { return _blendOpaque; }
        }

        public override RasterizerState RasterizerCullClockwise
        {
            get { return _rasterizerCullClockwise; }
        }

        public override RasterizerState RasterizerCullCounterClockwise
        {
            get { return _rasterizerCullCounterClockwise; }
        }

        public override RasterizerState RasterizerCullNone
        {
            get { return _rasterizerCullNone; }
        }

        public GLStateManager(WindowsGraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            _samplerLinearClamp = new GLSamplerState(_graphicsDevice)
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = TextureFilter.Linear
            };
            _samplerLinearClamp.Bond(_graphicsDevice);

            _samplerLinearWrap = new GLSamplerState(_graphicsDevice)
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = TextureFilter.Linear
            };
            _samplerLinearWrap.Bond(_graphicsDevice);

            _samplerPointClamp = new GLSamplerState(_graphicsDevice)
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = TextureFilter.Point
            };
            _samplerPointClamp.Bond(_graphicsDevice);

            _samplerPointWrap = new GLSamplerState(_graphicsDevice)
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = TextureFilter.Point
            };
            _samplerPointWrap.Bond(_graphicsDevice);

            _blendAdditive = new GLBlendState(_graphicsDevice)
            {
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.One,
                AlphaDestinationBlend = Blend.One
            };
            _blendAdditive.Bond(_graphicsDevice);

            _blendAlphaBlend = new GLBlendState(_graphicsDevice)
            {
                ColorSourceBlend = Blend.One,
                AlphaSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha
            };
            _blendAlphaBlend.Bond(_graphicsDevice);

            _blendNonPremultiplied = new GLBlendState(_graphicsDevice)
            {
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha
            };
            _blendNonPremultiplied.Bond(_graphicsDevice);

            _blendOpaque = new GLBlendState(_graphicsDevice)
            {
                ColorSourceBlend = Blend.One,
                AlphaSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.Zero
            };
            _blendOpaque.Bond(_graphicsDevice);

            _rasterizerCullClockwise = new GLRasterizerState(_graphicsDevice)
            {
                CullMode = CullMode.CullClockwiseFace
            };
            _rasterizerCullClockwise.Bond(_graphicsDevice);

            _rasterizerCullCounterClockwise = new GLRasterizerState(_graphicsDevice)
            {
                CullMode = CullMode.CullCounterClockwiseFace
            };
            _rasterizerCullCounterClockwise.Bond(_graphicsDevice);

            _rasterizerCullNone = new GLRasterizerState(_graphicsDevice)
            {
                CullMode = CullMode.None
            };
            _rasterizerCullNone.Bond(_graphicsDevice);
        }

        public override SamplerState CreateSamplerState()
        {
            return new GLSamplerState(_graphicsDevice);
        }

        public override BlendState CreateBlendState()
        {
            return new GLBlendState(_graphicsDevice);
        }

        public override RasterizerState CreateRasterizerState()
        {
            return new GLRasterizerState(_graphicsDevice);
        }
    }
}
