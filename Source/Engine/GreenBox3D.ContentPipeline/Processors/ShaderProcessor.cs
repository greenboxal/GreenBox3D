using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GreenBox3D.Graphics.Detail;
using GreenBox3D.ContentPipeline.Graphics;
using GreenBox3D.ContentPipeline.Graphics.Shading;
using GreenBox3D.ContentPipeline.CompilerServices;

using Ast = GreenBox3D.ContentPipeline.Graphics.Shading.Ast;

namespace GreenBox3D.ContentPipeline.Processors
{
    [ContentProcessor("ShaderTypeWriter", DisplayName = "Shader Processor")]
    public class ShaderProcessor : ContentProcessor<Ast.CompilationUnit, CompiledShaderCollection>
    {
        protected override CompiledShaderCollection Process(Ast.CompilationUnit input, ContentProcessorContext context)
        {
            CompiledShaderCollection shaders = new CompiledShaderCollection();

            foreach (Ast.Shader ash in input)
            {
                CompiledShader shader = new CompiledShader(ash.Name, ash.Version, ash.Fallback);

                foreach (Ast.InputVariable aiv in ash.Input)
                    shader.Input.Add(CompiledShaderUtils.InputVariableFromAst(aiv));
                
                foreach (Ast.Variable av in ash.Globals)
                    shader.Globals.Add(CompiledShaderUtils.VariableFromAst(av));

                foreach (Ast.Variable av in ash.Parameters)
                    shader.Parameters.Add(CompiledShaderUtils.VariableFromAst(av));

                foreach (Ast.Pass ap in ash.Passes)
                {
                    CompiledPass pass = CompilePass(shader, ash, ap, context);

                    if (pass == null)
                        return null;

                    shader.Passes.Add(pass);
                }

                shaders.Add(shader);
            }

            return shaders;
        }

        public CompiledPass CompilePass(CompiledShader shader, Ast.Shader ash, Ast.Pass ap, ContentProcessorContext context)
        {
            string glslVertex, glslPixel;

            context.AddDependency(ap.VertexGlsl);
            context.AddDependency(ap.PixelGlsl);

            glslVertex = CompileGlslStub(ash, CompileGlsl(ap.VertexGlsl, context, 0), ShaderType.Vertex);
            glslPixel = CompileGlslStub(ash, CompileGlsl(ap.PixelGlsl, context, 0), ShaderType.Pixel);

            if (glslVertex == null || glslPixel == null)
                return null;

            // TODO: Implement HLSL
            return new CompiledPass(glslVertex, glslPixel, "", "");
        }

        public string CompileGlsl(string filename, ContentProcessorContext context, int includedLine)
        {
            if (!File.Exists(filename))
            {
                context.Logger.Log(MessageLevel.Error, "FX0001", filename, includedLine, 0, 0, 0, "File doesn't exist");
                return null;
            }

            StringBuilder builder = new StringBuilder();
            StreamReader reader = new StreamReader(filename);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.TrimStart(' ', '\t');

                if (line.StartsWith("#include"))
                    context.Logger.Log(MessageLevel.Warning, "FX0002", filename, includedLine, 0, 0, 0, "Includes are not yet supported");
                else
                    builder.AppendLine(line);
            }

            return builder.ToString();
        }

        private string CompileGlslStub(Ast.Shader shader, string code, ShaderType type)
        {
            if (code == null)
                return null;

            StringBuilder c = new StringBuilder();
            string attribute;
            string varying;

            if (shader.Version >= 130)
            {
                attribute = "in";
                varying = type == ShaderType.Vertex ? "out" : "in";
            }
            else
            {
                attribute = "attribute";
                varying = "varying";
            }

            c.AppendFormat("#version {0}\n", shader.Version);

            if (type == ShaderType.Vertex)
            {
                for (int i = 0; i < shader.Input.Count; i++)
                    c.AppendFormat("layout(location = {0}) {1} {2};\n", i, attribute, BuildVariable(shader.Input[i]));
            }

            foreach (Ast.Variable parameter in shader.Parameters)
                c.AppendFormat("uniform {0};\n", BuildVariable(parameter));

            foreach (Ast.Variable global in shader.Globals)
                c.AppendFormat("{0} {1};\n", varying, BuildVariable(global));

            c.AppendLine(code);

            return c.ToString();
        }

        public string BuildVariable(Ast.Variable var)
        {
            string code = var.Type + " " + var.Name;

            if (var.Count > 0)
                code += "[" + var.Count + "]";

            return code;
        }
    }
}
