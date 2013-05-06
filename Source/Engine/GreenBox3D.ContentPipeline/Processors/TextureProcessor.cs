// TextureProcessor.cs
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
using GreenBox3D.ContentPipeline.Graphics;

namespace GreenBox3D.ContentPipeline.Processors
{
    [ContentProcessor("TextureTypeWriter", DisplayName = "Texture Processor")]
    public class TextureProcessor : ContentProcessor<TextureContent, TextureContent>
    {
        protected override TextureContent Process(TextureContent input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
