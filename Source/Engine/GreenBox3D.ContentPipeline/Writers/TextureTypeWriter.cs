// TextureTypeWriter.cs
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
using GreenBox3D.ContentPipeline.Graphics;
using GreenBox3D.Graphics;

namespace GreenBox3D.ContentPipeline.Writers
{
    [ContentTypeWriter(Extension = ".tex")]
    public class TextureTypeWriter : ContentTypeWriter<TextureContent>
    {
        protected override ContentHeader GetHeader(TextureContent input)
        {
            string magic = "TEX";

            if (input.GetType() == typeof(Texture2DContent))
                magic = "TEX2";

            return new ContentHeader(magic, new Version(1, 0));
        }

        protected override void Write(ContentWriter stream, TextureContent input)
        {
            stream.Write(input.Faces.Count);

            foreach (TextureFace face in input.Faces)
            {
                stream.Write(face.Count);

                foreach (BitmapContent bitmap in face)
                {
                    SurfaceFormat format;

                    if (!bitmap.TryGetFormat(out format))
                        continue;

                    stream.Write((int)format);
                    stream.Write(bitmap.Width);
                    stream.Write(bitmap.Height);

                    byte[] data = bitmap.GetPixelData();

                    stream.Write(data.Length);
                    stream.Write(data);
                }
            }
        }

        protected override bool ShouldCompressContent(TextureContent input)
        {
            return true;
        }
    }
}
