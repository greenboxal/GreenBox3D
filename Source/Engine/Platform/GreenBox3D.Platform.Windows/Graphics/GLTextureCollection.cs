using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GLTextureCollection : TextureCollection
    {
        #region Fields

        private readonly BitArray _array;
        private readonly ITexture[] _textures;

        #endregion

        #region Constructors and Destructors

        internal GLTextureCollection()
        {
            int size = 0;
            GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out size);

            _array = new BitArray(size, false);
            _textures = new ITexture[size];
        }

        #endregion

        public override ITexture this[int index]
        {
            get
            {
                lock (_textures)
                {
                    return _textures[index];
                }
            }
            set
            {
                if (index >= _textures.Length)
                    throw new IndexOutOfRangeException();

                lock (_textures)
                {
                    if (_textures[index] == value)
                        return;

                    _textures[index] = value;
                    _array[index] = true;
                }
            }
        }

        public override int Count
        {
            get { return _textures.Length; }
        }

        public override IEnumerator<ITexture> GetEnumerator()
        {
            return ((IEnumerable<ITexture>)_textures).GetEnumerator();
        }

        public override void Apply()
        {
            for (int i = 0; i < _textures.Length; i++)
            {
                if (!_array[i])
                    continue;

                ITexture tex = _textures[i];

               // tex.Create(true);
                //GL.ActiveTexture(TextureUnit.Texture0 + i);
                //GL.BindTexture(_textures[i].TextureTarget, _textures[i].TextureID);

                _array[i] = false;
            }
        }
    }
}
