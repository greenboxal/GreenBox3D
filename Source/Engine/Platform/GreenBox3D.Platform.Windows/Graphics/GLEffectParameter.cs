using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using GreenBox3D.Platform.Windows.Graphics.Shading;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GLEffectParameter : EffectParameter
    {
         #region Fields

        internal bool Dirty;
        internal GLShaderParameter Parameter;
        internal ITexture[] Textures;

        private readonly string _name;
        private readonly GLEffect _owner;

        #endregion

        #region Constructors and Destructors

        internal GLEffectParameter(GLEffect owner, GLShaderParameter parameter)
        {
            _owner = owner;

            _name = parameter.Name;
            Parameter = parameter;

            Dirty = true;
        }

        #endregion

        #region Public Properties

        public override EffectParameterClass Class { get { return Parameter.Class; } }
        public override int ColumnCount { get { return Parameter.ColumnCount; } }
        public override string Name { get { return _name; } }
        public override int RowCount { get { return Parameter.RowCount; } }
        public override EffectParameterType Type { get { return Parameter.Type; } }

        #endregion

        #region Public Methods and Operators

        public override void SetValue(ITexture value)
        {
            if (Class != EffectParameterClass.Sampler)
                throw new InvalidOperationException();

            Textures = new[] { value };
        }

        public override unsafe void SetValue(bool value)
        {
            // Bool has 4 bytes on the GPU
            if (sizeof(int) > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
                *(bool*)ptr = value;

            Dirty = true;
        }

        public override unsafe void SetValue(float value)
        {
            if (sizeof(float) > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
                *(float*)ptr = value;

            Dirty = true;
        }

        public override unsafe void SetValue(int value)
        {
            if (sizeof(int) > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
                *(int*)ptr = value;

            Dirty = true;
        }

        public override unsafe void SetValue(double value)
        {
            if (sizeof(double) > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
                *(double*)ptr = value;

            Dirty = true;
        }

        public override unsafe void SetValue(Vector2 value)
        {
            if (sizeof(Vector2) > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
                *(Vector2*)ptr = value;

            Dirty = true;
        }

        public override unsafe void SetValue(Vector3 value)
        {
            if (sizeof(Vector3) > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
                *(Vector3*)ptr = value;

            Dirty = true;
        }

        public override unsafe void SetValue(Vector4 value)
        {
            if (sizeof(Vector4) > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
                *(Vector4*)ptr = value;

            Dirty = true;
        }

        public override unsafe void SetValue(Matrix4 value)
        {
            if (sizeof(Matrix4) > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
                *(Matrix4*)ptr = value;

            Dirty = true;
        }

        public override void SetValue(ITexture[] value)
        {
            if (Class != EffectParameterClass.Sampler)
                throw new InvalidOperationException();

            Textures = (ITexture2D[])value.Clone();
        }

        public override unsafe void SetValue(bool[] value)
        {
            // Bool has 4 bytes on the GPU
            if (sizeof(int) * value.Length > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
            {
                for (int i = 0; i < value.Length; i++)
                    *(bool*)&ptr[i * sizeof(bool)] = value[i];
            }

            Dirty = true;
        }

        public override unsafe void SetValue(int[] value)
        {
            if (sizeof(int) * value.Length > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
            {
                for (int i = 0; i < value.Length; i++)
                    *(int*)&ptr[i * sizeof(int)] = value[i];
            }

            Dirty = true;
        }

        public override unsafe void SetValue(float[] value)
        {
            if (sizeof(float) * value.Length > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
            {
                for (int i = 0; i < value.Length; i++)
                    *(float*)&ptr[i * sizeof(float)] = value[i];
            }

            Dirty = true;
        }

        public override unsafe void SetValue(double[] value)
        {
            if (sizeof(double) * value.Length > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
            {
                for (int i = 0; i < value.Length; i++)
                    *(double*)&ptr[i * sizeof(double)] = value[i];
            }

            Dirty = true;
        }

        public override unsafe void SetValue(Vector2[] value)
        {
            if (sizeof(Vector2) * value.Length > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
            {
                for (int i = 0; i < value.Length; i++)
                    *(Vector2*)&ptr[i * sizeof(Vector2)] = value[i];
            }

            Dirty = true;
        }

        public override unsafe void SetValue(Vector3[] value)
        {
            if (sizeof(Vector3) * value.Length > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
            {
                for (int i = 0; i < value.Length; i++)
                    *(Vector3*)&ptr[i * sizeof(Vector3)] = value[i];
            }

            Dirty = true;
        }

        public override unsafe void SetValue(Vector4[] value)
        {
            if (sizeof(Vector4) * value.Length > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
            {
                for (int i = 0; i < value.Length; i++)
                    *(Vector4*)&ptr[i * sizeof(Vector4)] = value[i];
            }

            Dirty = true;
        }

        public override unsafe void SetValue(Matrix4[] value)
        {
            if (sizeof(Matrix4) * value.Length > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
            {
                for (int i = 0; i < value.Length; i++)
                    *(Matrix4*)&ptr[i * sizeof(Matrix4)] = value[i];
            }

            Dirty = true;
        }

        public override unsafe void SetValueTranspose(Matrix4 value)
        {
            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
                *(Matrix4*)ptr = Matrix4.Transpose(value);

            Dirty = true;
        }

        public override unsafe void SetValueTranspose(Matrix4[] value)
        {
            if (sizeof(Matrix4) * value.Length > Parameter.ByteSize)
                throw new InvalidOperationException();

            fixed (byte* ptr = &_owner.ParameterData[Parameter.Offset])
            {
                for (int i = 0; i < value.Length; i++)
                    *(Matrix4*)&ptr[i * sizeof(Matrix4)] = Matrix4.Transpose(value[i]);
            }

            Dirty = true;
        }

        #endregion

        #region Methods

        internal unsafe void Apply(byte* ptr)
        {
            Parameter.Apply(&ptr[Parameter.Offset]);
            Dirty = false;
        }

        #endregion
    }
}
