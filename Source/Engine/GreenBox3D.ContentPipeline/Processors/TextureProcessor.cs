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
using GreenBox3D.Graphics;

namespace GreenBox3D.ContentPipeline.Processors
{
    [ContentProcessor("TextureTypeWriter", DisplayName = "Texture Processor")]
    public class TextureProcessor : ContentProcessor<TextureContent, TextureContent>
    {
        protected override TextureContent Process(TextureContent input, ContentProcessorContext context)
        {
            ChooseSurfaceFormat(input, context);

            return input;
        }

        private void ChooseSurfaceFormat(TextureContent input, ContentProcessorContext context)
        {
            object value;
            SurfaceFormat format = SurfaceFormat.Color;

            if (context.Parameters.TryGetValue("SurfaceFormat", out value))
            {
                Enum.TryParse(value.ToString(), true, out format);
            }
            else
            {
                if (input.Faces.Count > 0)
                {
                    if (input.Faces[0].Count > 0)
                    {
                        PixelDataType dt;

                        if (input.Faces[0][0].TryGetFormat(out dt))
                        {
                            switch (dt.Format)
                            {
                                case PixelFormat.R:
                                case PixelFormat.Depth:
                                    format = SurfaceFormat.R;
                                    break;
                                case PixelFormat.Rg:
                                case PixelFormat.DepthStencil:
                                    format = SurfaceFormat.Rg;
                                    break;
                                case PixelFormat.Rgb:
                                case PixelFormat.Bgr:
                                    format = SurfaceFormat.Rgb;
                                    break;
                                case PixelFormat.Rgba:
                                case PixelFormat.Bgra:
                                    format = SurfaceFormat.Color;
                                    break;
                            }
                        }
                    }
                }
            }

            input.Format = format;
        }
    }
}
