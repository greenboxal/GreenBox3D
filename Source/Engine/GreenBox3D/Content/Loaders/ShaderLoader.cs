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
using System.Threading.Tasks;
using GreenBox3D.Graphics;

//using GreenBox3D.Graphics.Shading;

namespace GreenBox3D.Content.Loaders
{
    /*  [ContentTypeReader(Extension = ".fx")]
    public class ShaderLoader : ContentTypeReader<ShaderCollection>
    {
        #region Constructors and Destructors

        public ShaderLoader()
        {
            Magic = "FX";
            Version = new Version(1, 0);
        }

        #endregion

        #region Methods

        protected override ShaderCollection Load(ContentManager manager, ContentReader reader)
        {
            ShaderCollection shaders = new ShaderCollection();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Shader shader = new Shader(manager.GraphicsDevice, reader.ReadString());

                shader.Version = reader.ReadInt32();
                shader.Fallback = reader.ReadString();

                int inputCount = reader.ReadInt32();
                int uniformCount = reader.ReadInt32();
                int globalCount = reader.ReadInt32();
                int passCount = reader.ReadInt32();

                shader.Input = new ShaderInput[inputCount];
                for (int j = 0; j < inputCount; j++)
                {
                    reader.ReadString();
                    reader.ReadString();
                    reader.ReadInt32();
                    shader.Input[j] = new ShaderInput(j, (VertexElementUsage)reader.ReadInt32(), reader.ReadInt32());
                }

                for (int j = 0; j < uniformCount; j++)
                {
                    string name = reader.ReadString();
                    string type = reader.ReadString();
                    int size = reader.ReadInt32();

                    shader.Parameters.Add(new ShaderParameter(name, type, size));
                }

                for (int j = 0; j < globalCount; j++)
                {
                    reader.ReadString();
                    reader.ReadString();
                    reader.ReadInt32();
                }

                for (int j = 0; j < passCount; j++)
                    shader.Passes.Add(new ShaderPass(manager.GraphicsDevice, reader.ReadString(), reader.ReadString()));

                shaders.Add(shader);
            }

            return shaders;
        }

        #endregion
    }*/
}
