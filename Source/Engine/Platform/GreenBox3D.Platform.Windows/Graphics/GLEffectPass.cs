using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GreenBox3D.Graphics;
using GreenBox3D.Platform.Windows.Graphics.Shading;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public class GLEffectPass : IEffectPass
    {
        #region Static Fields

        private static GLEffect _lastEffect;
        private static GLShaderPass _lastEffectPass;

        #endregion

        #region Fields

        private readonly GLEffect _owner;
        private readonly GLShaderPass _pass;

        #endregion

        #region Constructors and Destructors

        internal GLEffectPass(GLEffect owner, GLShaderPass pass)
        {
            _owner = owner;
            _pass = pass;
        }

        #endregion

        #region Public Methods and Operators

        public static void ResetActiveProgramCache()
        {
            _lastEffect = null;
            _lastEffectPass = null;
        }

        public void Apply()
        {
            bool applyTextures = false;

            if (_lastEffectPass != _pass)
            {
                GL.UseProgram(_pass.ProgramID);
                _lastEffectPass = _pass;
            }

            unsafe
            {
                fixed (byte* ptr = &_owner.ParameterData[0])
                {
                    foreach (GLEffectParameter parameter in _owner.Parameters)
                    {
                        if (parameter.Parameter.Slot == -1)
                            continue;

                        if (_lastEffect != _owner || parameter.Dirty)
                            parameter.Apply(ptr);

                        if (parameter.Class == EffectParameterClass.Sampler && parameter.Textures != null)
                        {
                            for (int i = 0; i < parameter.Textures.Length; i++)
                            {
                                _pass.GraphicsDevice.Textures[parameter.Parameter.TextureUnit + i] = parameter.Textures[i];
                                applyTextures = true;
                            }
                        }
                    }
                }
            }

            if (applyTextures)
                _pass.GraphicsDevice.Textures.Apply();

            (_pass.GraphicsDevice as WindowsGraphicsDevice).ActiveShader = _owner.Shader;
            _lastEffect = _owner;
        }

        #endregion
    }
}
