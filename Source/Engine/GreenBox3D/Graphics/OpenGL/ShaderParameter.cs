// ShaderParameter.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
#if DESKTOP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public class ShaderParameter
    {
        private readonly int _location;
        private readonly string _name;
        private readonly int _size;

        public ShaderParameter(Shader owner, int programID, int index)
        {
            ActiveUniformType type;

            _name = GL.GetActiveUniform(programID, index, out _size, out type);
            _location = GL.GetUniformLocation(programID, _name);
        }

        public string Name
        {
            get { return _name; }
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

#endif
