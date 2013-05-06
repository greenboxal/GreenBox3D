using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public sealed class SamplerStateCollection : IReadOnlyCollection<SamplerState>
    {
        private readonly GraphicsDevice _owner;
        private readonly SamplerState[] _samplers;
        private BitVector32 _state;

        public int Count
        {
            get { return _samplers.Length; }
        }

        public SamplerState this[int index]
        {
            get { return _samplers[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_samplers[index] != value)
                {
                    value.Bond(_owner);
                    _samplers[index] = value;
                    _state[1 << index] = true;
                }
            }
        }

        internal SamplerStateCollection(GraphicsDevice graphicsDevice)
        {
            int count;

            _owner = graphicsDevice;

            GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out count);
            _samplers = new SamplerState[count];

            for (int i = 0; i < count; i++)
                _samplers[i] = SamplerState.PointWrap;

            _state[int.MaxValue] = true;
        }

        public IEnumerator<SamplerState> GetEnumerator()
        {
            return ((IEnumerable<SamplerState>)_samplers).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Apply()
        {
            if (_state.Data == 0)
                return;

            for (int i = 0; i < _samplers.Length; i++)
            {
                if (!_state[1 << i])
                    continue;

                GL.BindSampler(i, _samplers[i].SamplerID);

                _state[1 << i] = false;
            }
        }
    }
}
