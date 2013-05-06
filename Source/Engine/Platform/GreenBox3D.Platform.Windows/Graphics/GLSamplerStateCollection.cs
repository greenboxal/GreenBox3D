using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GLSamplerStateCollection : SamplerStateCollection
    {
        private readonly SamplerState[] _samplers;
        private BitVector32 _state;

        public override int Count
        {
            get { return _samplers.Length; }
        }

        public override SamplerState this[int index]
        {
            get { return _samplers[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_samplers[index] != value)
                {
                    (value as GLSamplerState).Bond();
                    _samplers[index] = value;
                    _state[1 << index] = true;
                }
            }
        }

        public GLSamplerStateCollection(WindowsGraphicsDevice graphicsDevice)
        {
            int count;

            GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out count);
            _samplers = new SamplerState[count];

            for (int i = 0; i < count; i++)
                _samplers[i] = graphicsDevice.StateManager.SamplerPointWrap;

            _state[int.MaxValue] = true;
        }

        public override IEnumerator<SamplerState> GetEnumerator()
        {
            return ((IEnumerable<SamplerState>)_samplers).GetEnumerator();
        }

        public void Apply()
        {
            if (_state.Data == 0)
                return;

            for (int i = 0; i < _samplers.Length; i++)
            {
                if (!_state[1 << i])
                    continue;

                GLSamplerState sampler = _samplers[i] as GLSamplerState;
                GL.BindSampler(i, sampler.SamplerID);

                _state[1 << i] = false;
            }
        }
    }
}
