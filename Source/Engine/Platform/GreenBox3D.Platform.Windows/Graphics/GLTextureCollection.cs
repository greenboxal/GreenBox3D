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
    public class GLTextureCollection : TextureCollection
    {
        private readonly ITexture[] _textures;
        private int _lastActiveUnit;
        private BitVector32 _state;

        public override int Count
        {
            get { return _textures.Length; }
        }

        public override ITexture this[int index]
        {
            get { return _textures[index]; }
            set
            {
                if (_textures[index] != value)
                {
                    _textures[index] = value;
                    _state[1 << index] = true;
                }
            }
        }

        public GLTextureCollection()
        {
            int count;

            GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out count);
            _textures = new ITexture[count];
        }

        public override IEnumerator<ITexture> GetEnumerator()
        {
            return ((IEnumerable<ITexture>)_textures).GetEnumerator();
        }

        public void Apply()
        {
            if (_state.Data == 0)
                return;

            for (int i = 0; i < _textures.Length; i++)
            {
                if (!_state[1 << i])
                    continue;

                Texture texture = _textures[i] as Texture;

                if (texture == null)
                    continue;

                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.BindTexture(texture.TextureTarget, texture.TextureID);

                _lastActiveUnit = i;
                _state[1 << i] = false;
            }
        }

        public void SetLastUnitDirty()
        {
            _state[_lastActiveUnit] = true;
        }
    }
}
