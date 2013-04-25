using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public interface IShaderParameter
    {
        string Name { get; }

        void SetValue(ITexture value);
        void SetValue(bool value);
        void SetValue(float value);
        void SetValue(int value);
        void SetValue(double value);
        void SetValue(Vector2 value);
        void SetValue(Vector3 value);
        void SetValue(Vector4 value);
        void SetValue(Matrix4 value);
        void SetValue(ITexture[] value);
        void SetValue(bool[] value);
        void SetValue(int[] value);
        void SetValue(float[] value);
        void SetValue(double[] value);
        void SetValue(Vector2[] value);
        void SetValue(Vector3[] value);
        void SetValue(Vector4[] value);
        void SetValue(Matrix4[] value);
        void SetValueTranspose(Matrix4 value);
        void SetValueTranspose(Matrix4[] value);
    }
}
