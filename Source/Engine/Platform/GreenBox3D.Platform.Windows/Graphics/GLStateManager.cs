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
            _samplerLinearClamp.Bond();

            _samplerLinearWrap = new GLSamplerState(_graphicsDevice)
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = TextureFilter.Linear
            };
            _samplerLinearWrap.Bond();

            _samplerPointClamp = new GLSamplerState(_graphicsDevice)
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = TextureFilter.Point
            };
            _samplerPointClamp.Bond();

            _samplerPointWrap = new GLSamplerState(_graphicsDevice)
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = TextureFilter.Point
            };
            _samplerPointWrap.Bond();
        }

        public override SamplerState CreateSamplerState()
        {
            return new GLSamplerState(_graphicsDevice);
        }
    }
}
