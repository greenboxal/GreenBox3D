// Texture2DLoader.cs
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
using GreenBox3D.Graphics;

namespace GreenBox3D.Content.Readers
{
    [ContentTypeReader(Extension = ".tex")]
    public class Texture2DLoader : ContentTypeReader<ITexture2D>
    {
        #region Constructors and Destructors

        public Texture2DLoader()
        {
            Magic = "TEX2";
            Version = new Version(1, 0);
        }

        #endregion

        #region Methods

        protected override ITexture2D Load(ContentManager manager, ContentReader reader)
        {
            // A Texture2D has only 1 face
            reader.ReadInt32();

            int mipmapCount = reader.ReadInt32();
            ITexture2D texture = null;

            for (int i = 0; i < mipmapCount; i++)
            {
                SurfaceFormat format = (SurfaceFormat)reader.ReadInt32();
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();

                if (texture == null)
                    texture = manager.GraphicsDevice.TextureManager.CreateTexture2D(format, width, height);

                int len = reader.ReadInt32();
                byte[] data = reader.ReadBytes(len);

                texture.SetData(i, data, 0);
            }

            return texture;
        }

        #endregion
    }
}
