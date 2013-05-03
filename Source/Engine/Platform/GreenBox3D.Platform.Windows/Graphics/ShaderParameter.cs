using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class ShaderParameter : IShaderParameter
    {
        private readonly int _location;
        private readonly string _name;
        private readonly int _size;
        private readonly int _textureUnit;

        public ShaderParameter(Shader owner, int programID, int index)
        {
            ActiveUniformType type;

            _name = GL.GetActiveUniform(programID, index, out _size, out type);
            _location = GL.GetUniformLocation(programID, _name);

            if ((type >= ActiveUniformType.Sampler1D && type <= ActiveUniformType.Sampler2DRectShadow) &&
                (type >= ActiveUniformType.IntSampler1D && type <= ActiveUniformType.UnsignedIntSamplerBuffer))
            {
                _textureUnit = owner.TextureUnitCounter;
                owner.TextureUnitCounter += _size;
            }
            else
                _textureUnit = -1;
        }

        public string Name
        {
            get { return _name; }
        }

        public void SetValue(ITexture value)
        {
            if (_location == -1)
                return;

            if (_textureUnit == -1)
                return;

            GL.Uniform1(_location, _textureUnit);
            (value as Texture).Bind(_textureUnit);
        }

        public void SetValue(bool value)
        {
            if (_location == -1)
                return;

            GL.Uniform1(_location, value ? 1 : 0);
        }

        public void SetValue(float value)
        {
            if (_location == -1)
                return;

            GL.Uniform1(_location, value);
        }

        public void SetValue(int value)
        {
            if (_location == -1)
                return;

            GL.Uniform1(_location, value);
        }

        public void SetValue(double value)
        {
            if (_location == -1)
                return;

            GL.Uniform1(_location, value);
        }

        public unsafe void SetValue(Vector2 value)
        {
            if (_location == -1)
                return;

            GL.Uniform2(_location, 1, (float*)&value);
        }

        public unsafe void SetValue(Vector3 value)
        {
            if (_location == -1)
                return;

            GL.Uniform3(_location, 1, (float*)&value);
        }

        public unsafe void SetValue(Vector4 value)
        {
            if (_location == -1)
                return;

            GL.Uniform4(_location, 1, (float*)&value);
        }

        public void SetValue(Color value)
        {
            if (_location == -1)
                return;

            GL.Uniform4(_location, value.R / 255.0f, value.G / 255.0f, value.B / 255.0f, value.A / 255.0f);
        }

        public unsafe void SetValue(Matrix4 value)
        {
            if (_location == -1)
                return;

            GL.UniformMatrix4(_location, 1, false, (float*)&value);
        }

        public void SetValue(ITexture[] value)
        {
            if (_location == -1)
                return;

            if (_textureUnit == -1)
                return;

            for (int i = 0; i < value.Length; i++)
            {
                if (i >= _size)
                    break;

                GL.Uniform1(_location, _textureUnit + i);
                (value[i] as Texture).Bind(_textureUnit + i);
            }
        }

        public unsafe void SetValue(bool[] value)
        {
            if (_location == -1)
                return;

            int count = value.Length;

            if (count > _size)
                count = _size;

            fixed (bool* p = &value[0])
                GL.Uniform1(_location, count, (int*)p);
        }

        public unsafe void SetValue(int[] value)
        {
            if (_location == -1)
                return;

            int count = value.Length;

            if (count > _size)
                count = _size;

            fixed (int* p = &value[0])
                GL.Uniform1(_location, count, p);
        }

        public unsafe void SetValue(float[] value)
        {
            if (_location == -1)
                return;

            int count = value.Length;

            if (count > _size)
                count = _size;

            fixed (float* p = &value[0])
                GL.Uniform1(_location, count, p);
        }

        public unsafe void SetValue(double[] value)
        {
            if (_location == -1)
                return;

            int count = value.Length;

            if (count > _size)
                count = _size;

            fixed (double* p = &value[0])
                GL.Uniform1(_location, count, (float*)p);
        }

        public unsafe void SetValue(Vector2[] value)
        {
            if (_location == -1)
                return;

            int count = value.Length;

            if (count > _size)
                count = _size;

            fixed (Vector2* p = &value[0])
                GL.Uniform2(_location, count, (float*)p);
        }

        public unsafe void SetValue(Vector3[] value)
        {
            if (_location == -1)
                return;

            int count = value.Length;

            if (count > _size)
                count = _size;

            fixed (Vector3* p = &value[0])
                GL.Uniform3(_location, count, (float*)p);
        }

        public unsafe void SetValue(Vector4[] value)
        {
            if (_location == -1)
                return;

            int count = value.Length;

            if (count > _size)
                count = _size;

            fixed (Vector4* p = &value[0])
                GL.Uniform4(_location, count, (float*)p);
        }

        public unsafe void SetValue(Matrix4[] value)
        {
            if (_location == -1)
                return;

            int count = value.Length;

            if (count > _size)
                count = _size;

            fixed (Matrix4* p = &value[0])
                GL.UniformMatrix4(_location, count, false, (float*)p);
        }

        public unsafe void SetValueTranspose(Matrix4 value)
        {
            if (_location == -1)
                return;

            GL.UniformMatrix4(_location, 1, true, (float*)&value);
        }

        public unsafe void SetValueTranspose(Matrix4[] value)
        {
            if (_location == -1)
                return;

            int count = value.Length;

            if (count > _size)
                count = _size;

            fixed (Matrix4* p = &value[0])
                GL.UniformMatrix4(_location, count, true, (float*)p);
        }
    }
}
