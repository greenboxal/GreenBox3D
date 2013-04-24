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

namespace GreenBox3D.Content.Loaders
{
    /*  [ContentTypeReader(Extension = ".tex")]
    public class Texture2DLoader : ContentTypeReader<Texture2D>
    {
        #region Constructors and Destructors

        public Texture2DLoader()
        {
            Magic = "TEX2";
            Version = new Version(1, 0);
        }

        #endregion

        #region Methods

        protected override Texture2D Load(ContentManager manager, ContentReader reader)
        {
            SurfaceFormat format = (SurfaceFormat)reader.ReadInt32();
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            int mipmapCount = reader.ReadInt32();

            Texture2D texture = new Texture2D(manager.GraphicsDevice, format, width, height);

            for (int i = 0; i < mipmapCount; i++)
                texture.SetData(i, reader.ReadBytes(reader.ReadInt32()), 0);

            return texture;
        }

        #endregion
    }*/
}
