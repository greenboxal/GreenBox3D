// ShaderTypeWriter.cs
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
using GreenBox3D.Content;
using GreenBox3D.Graphics.Detail;
using GreenBox3D.ContentPipeline.Graphics;
using GreenBox3D.ContentPipeline.CompilerServices;

namespace GreenBox3D.ContentPipeline.Writers
{
    [ContentTypeWriter(Extension = ".fx")]
    public class ShaderTypeWriter : ContentTypeWriter<CompiledShaderCollection>
    {
        protected override ContentHeader GetHeader(CompiledShaderCollection input)
        {
            return new ContentHeader("FX", new Version(1, 0));
        }

        protected override bool ShouldCompressContent(CompiledShaderCollection input)
        {
            // We use this to semi-encrypt our shaders
            return true;
        }

        protected override void Write(ContentWriter stream, CompiledShaderCollection shaders)
        {
            stream.Write(shaders.Count);

            foreach (CompiledShader shader in shaders)
            {
                stream.Write(shader.Name);
                stream.Write(shader.Version);
                stream.Write(shader.Fallback);
                stream.Write(shader.Input.Count);
                stream.Write(shader.Parameters.Count);
                stream.Write(shader.Globals.Count);
                stream.Write(shader.Passes.Count);

                foreach (CompiledInputVariable ci in shader.Input)
                {
                    WriteVariable(stream, ci);
                    stream.Write((int)ci.Usage);
                    stream.Write(ci.UsageIndex);
                }

                foreach (CompiledVariable v in shader.Parameters)
                    WriteVariable(stream, v);

                foreach (CompiledVariable v in shader.Globals)
                    WriteVariable(stream, v);

                foreach (CompiledPass p in shader.Passes)
                {
                    stream.Write(p.GlslVertexCode ?? "");
                    stream.Write(p.GlslPixelCode ?? "");
                    stream.Write(p.HlslVertexCode ?? "");
                    stream.Write(p.HlslPixelCode ?? "");
                }
            }
        }

        private static void WriteVariable(ContentWriter stream, CompiledVariable v)
        {
            stream.Write(v.Name);
            stream.Write(v.Count);
            stream.Write(v.RowCount);
            stream.Write(v.ColumnCount);
            stream.Write((int)v.EffectParameterClass);
            stream.Write((int)v.EffectParameterType);
        }
    }
}
