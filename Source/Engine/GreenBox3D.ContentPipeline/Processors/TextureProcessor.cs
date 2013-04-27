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
