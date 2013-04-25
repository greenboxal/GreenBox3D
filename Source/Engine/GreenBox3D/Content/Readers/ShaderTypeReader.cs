// ShaderLoader.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenBox3D.Graphics;
using GreenBox3D.Graphics.Detail;

namespace GreenBox3D.Content.Readers
{
    [ContentTypeReader(Extension = ".fx")]
    public class ShaderLoader : ContentTypeReader<IShader>
    {
        #region Constructors and Destructors

        public ShaderLoader()
        {
            Magic = "FX";
            Version = new Version(1, 0);
        }

        #endregion

        #region Methods

        protected override IShader Load(ContentManager manager, ContentReader reader)
        {
            CompiledShader shader = new CompiledShader(reader.ReadString(), reader.ReadInt32(), reader.ReadString(),
                                                       reader.ReadString(), reader.ReadString(), reader.ReadString(),
                                                       reader.ReadString());

            int inputCount = reader.ReadInt32();

            for (int j = 0; j < inputCount; j++)
                shader.Input.Add(new CompiledInputVariable(
                                     reader.ReadString(),
                                     (VertexElementUsage)reader.ReadInt32(),
                                     reader.ReadInt32()
                                     ));

            return manager.GraphicsDevice.ShaderManager.CreateShader(shader);
        }

        #endregion
    }
}
