// ShaderProcessor.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

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
                string glslVertex, glslPixel;

                context.AddDependency(ash.GlslVertexCode);
                context.AddDependency(ash.GlslPixelCode);

                glslVertex = CompileGlslStub(ash, CompileGlsl(ash.GlslVertexCode, context, 0));
                glslPixel = CompileGlslStub(ash, CompileGlsl(ash.GlslPixelCode, context, 0));

                CompiledShader shader = new CompiledShader(ash.Name, ash.Version, ash.Fallback, glslVertex, glslPixel,
                                                           null, null);

                foreach (Ast.InputVariable aiv in ash.Input)
                    shader.Input.Add(CompiledShaderUtils.InputVariableFromAst(aiv));

                shaders.Add(shader);
            }

            return shaders;
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
                    context.Logger.Log(MessageLevel.Warning, "FX0002", filename, includedLine, 0, 0, 0,
                                       "Includes are not yet supported");
                else
                    builder.AppendLine(line);
            }

            return builder.ToString();
        }

        private string CompileGlslStub(Ast.Shader shader, string code)
        {
            if (code == null)
                return null;

            StringBuilder c = new StringBuilder();

            c.AppendFormat("#version {0}\n", shader.Version);
            c.AppendLine(code);

            return c.ToString();
        }
    }
}
