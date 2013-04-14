// GLShaderParameter.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics.Shading
{
    public class GLShaderParameter : GreenBox3D.Graphics.Detail.ShaderParameter
    {
        public unsafe delegate void ApplyDelegate(byte* ptr);

        public ApplyDelegate Apply;

        public GLShaderParameter(GreenBox3D.Graphics.Detail.ShaderParameter original)
            : base(original)
        {
            unsafe
            {
                if (Count == 0)
                {
                    if (RowCount > 1)
                    {
                        switch (Type)
                        {
                            case EffectParameterType.Single:
                                switch (RowCount)
                                {
                                    case 3:
                                        Apply = ApplyMatrix3F;
                                        break;
                                    case 4:
                                        Apply = ApplyMatrix4F;
                                        break;
                                }
                                break;
                            case EffectParameterType.Double:
                                switch (RowCount)
                                {
                                    case 3:
                                        Apply = ApplyMatrix3D;
                                        break;
                                    case 4:
                                        Apply = ApplyMatrix4D;
                                        break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (Type)
                        {
                            case EffectParameterType.Bool:
                                switch (ColumnCount)
                                {
                                    case 1:
                                        Apply = Apply1B;
                                        break;
                                    case 2:
                                        Apply = Apply2B;
                                        break;
                                    case 3:
                                        Apply = Apply3B;
                                        break;
                                    case 4:
                                        Apply = Apply4B;
                                        break;
                                }
                                break;
                            case EffectParameterType.Texture1D:
                            case EffectParameterType.Texture2D:
                            case EffectParameterType.Texture3D:
                            case EffectParameterType.TextureCube:
                            case EffectParameterType.Int32:
                                switch (ColumnCount)
                                {
                                    case 1:
                                        Apply = Apply1I;
                                        break;
                                    case 2:
                                        Apply = Apply2I;
                                        break;
                                    case 3:
                                        Apply = Apply3I;
                                        break;
                                    case 4:
                                        Apply = Apply4I;
                                        break;
                                }
                                break;
                            case EffectParameterType.Single:
                                switch (ColumnCount)
                                {
                                    case 1:
                                        Apply = Apply1F;
                                        break;
                                    case 2:
                                        Apply = Apply2F;
                                        break;
                                    case 3:
                                        Apply = Apply3F;
                                        break;
                                    case 4:
                                        Apply = Apply4F;
                                        break;
                                }
                                break;
                            case EffectParameterType.Double:
                                switch (ColumnCount)
                                {
                                    case 1:
                                        Apply = Apply1D;
                                        break;
                                    case 2:
                                        Apply = Apply2D;
                                        break;
                                    case 3:
                                        Apply = Apply3D;
                                        break;
                                    case 4:
                                        Apply = Apply4D;
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }

        #region Methods

        private unsafe void Apply1B(byte* ptr)
        {
            GL.Uniform1(Slot, Count == 0 ? 1 : Count, (int*)ptr);
        }

        private unsafe void Apply1D(byte* ptr)
        {
            GL.Uniform1(Slot, Count == 0 ? 1 : Count, (double*)ptr);
        }

        private unsafe void Apply1F(byte* ptr)
        {
            GL.Uniform1(Slot, Count == 0 ? 1 : Count, (float*)ptr);
        }

        private unsafe void Apply1I(byte* ptr)
        {
            GL.Uniform1(Slot, Count == 0 ? 1 : Count, (int*)ptr);
        }

        private unsafe void Apply2B(byte* ptr)
        {
            GL.Uniform2(Slot, Count == 0 ? 1 : Count, (int*)ptr);
        }

        private unsafe void Apply2D(byte* ptr)
        {
            GL.Uniform2(Slot, Count == 0 ? 1 : Count, (double*)ptr);
        }

        private unsafe void Apply2F(byte* ptr)
        {
            GL.Uniform2(Slot, Count == 0 ? 1 : Count, (float*)ptr);
        }

        private unsafe void Apply2I(byte* ptr)
        {
            GL.Uniform2(Slot, Count == 0 ? 1 : Count, (int*)ptr);
        }

        private unsafe void Apply3B(byte* ptr)
        {
            GL.Uniform3(Slot, Count == 0 ? 1 : Count, (int*)ptr);
        }

        private unsafe void Apply3D(byte* ptr)
        {
            GL.Uniform3(Slot, Count == 0 ? 1 : Count, (double*)ptr);
        }

        private unsafe void Apply3F(byte* ptr)
        {
            GL.Uniform3(Slot, Count == 0 ? 1 : Count, (float*)ptr);
        }

        private unsafe void Apply3I(byte* ptr)
        {
            GL.Uniform3(Slot, Count == 0 ? 1 : Count, (int*)ptr);
        }

        private unsafe void Apply4B(byte* ptr)
        {
            GL.Uniform4(Slot, Count == 0 ? 1 : Count, (int*)ptr);
        }

        private unsafe void Apply4D(byte* ptr)
        {
            GL.Uniform4(Slot, Count == 0 ? 1 : Count, (double*)ptr);
        }

        private unsafe void Apply4F(byte* ptr)
        {
            GL.Uniform4(Slot, Count == 0 ? 1 : Count, (float*)ptr);
        }

        private unsafe void Apply4I(byte* ptr)
        {
            GL.Uniform4(Slot, Count == 0 ? 1 : Count, (int*)ptr);
        }

        private unsafe void ApplyMatrix3D(byte* ptr)
        {
            GL.UniformMatrix3(Slot, Count == 0 ? 1 : Count, false, (double*)ptr);
        }

        private unsafe void ApplyMatrix3F(byte* ptr)
        {
            GL.UniformMatrix3(Slot, Count == 0 ? 1 : Count, false, (float*)ptr);
        }

        private unsafe void ApplyMatrix4D(byte* ptr)
        {
            GL.UniformMatrix4(Slot, Count == 0 ? 1 : Count, false, (double*)ptr);
        }

        private unsafe void ApplyMatrix4F(byte* ptr)
        {
            GL.UniformMatrix4(Slot, Count == 0 ? 1 : Count, false, (float*)ptr);
        }

        #endregion
    }
}
