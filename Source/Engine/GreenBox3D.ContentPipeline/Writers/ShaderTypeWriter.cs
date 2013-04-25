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
            foreach (CompiledShader shader in shaders)
            {
                stream.Write(shader.Name);
                stream.Write(shader.Version);
                stream.Write(shader.Fallback);

                stream.Write(shader.GlslVertexCode ?? "");
                stream.Write(shader.GlslPixelCode ?? "");
                stream.Write(shader.HlslVertexCode ?? "");
                stream.Write(shader.HlslPixelCode ?? "");

                stream.Write(shader.Input.Count);

                foreach (CompiledInputVariable ci in shader.Input)
                {
                    stream.Write(ci.Name);
                    stream.Write((int)ci.Usage);
                    stream.Write(ci.UsageIndex);
                }

                // FIXME: Just one shader per file
                break;
            }
        }
    }
}
