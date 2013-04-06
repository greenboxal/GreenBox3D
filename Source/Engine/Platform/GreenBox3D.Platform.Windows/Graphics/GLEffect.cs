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
    public class GLEffect : IEffect
    {
        internal readonly GLShader Shader;

        private EffectParameterCollection _parameters;
        private EffectPassCollection _passes;

        public EffectParameterCollection Parameters
        {
            get { return _parameters; }
        }

        public EffectPassCollection Passes
        {
            get { return _passes; }
        }

        internal byte[] ParameterData { get; set; }

        internal GLEffect(GLShader shader)
        {
            Shader = shader;
            ParameterData = new byte[Shader.ParametersSize];

            EffectParameter[] parameters = new EffectParameter[Shader.Parameters.Count];
            for (int i = 0; i < Shader.Parameters.Count; i++)
                parameters[i] = new GLEffectParameter(this, Shader.Parameters[i] as GLShaderParameter);

            IEffectPass[] passes = new IEffectPass[Shader.Passes.Count];
            for (int i = 0; i < Shader.Passes.Count; i++)
                passes[i] = new GLEffectPass(this, Shader.Passes[i] as GLShaderPass);

            _parameters = new EffectParameterCollection(parameters);
            _passes = new EffectPassCollection(passes);

            foreach (GLEffectParameter parameter in Parameters)
            {
                if (parameter.Class == EffectParameterClass.Sampler)
                {
                    int count = parameter.Parameter.Count == 0 ? 1 : parameter.Parameter.Count;
                    int[] units = new int[count];

                    for (int i = 0; i < count; i++)
                        units[i] = parameter.Parameter.TextureUnit + i;

                    parameter.SetValue(count);
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
