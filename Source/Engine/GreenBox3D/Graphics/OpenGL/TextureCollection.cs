// TextureCollection.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
#if DESKTOP

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
    public sealed class TextureCollection : IReadOnlyCollection<Texture>
    {
        private readonly Texture[] _textures;
        private int _lastActiveUnit;
        private BitVector32 _state;

        public int Count
        {
            get { return _textures.Length; }
        }

        public Texture this[int index]
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

        public TextureCollection()
        {
            int count;

            GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out count);
            _textures = new Texture[count];
        }

        public IEnumerator<Texture> GetEnumerator()
        {
            return ((IEnumerable<Texture>)_textures).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

#endif
