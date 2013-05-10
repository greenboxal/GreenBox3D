// GraphicBatch.cs
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

namespace GreenBox3D.Graphics
{
    public class GraphicBatch : GraphicsResource
    {
        internal Shader StandardShader;
        internal ShaderParameter StandardShaderMatrixParameter;
        internal Matrix4 StandardViewProjection;

        private List<GraphicOperation> _opList;

        public GraphicBatch()
        {
            CreateSharedResources();
        }

        private void CreateSharedResources()
        {
            if (StandardShader == null)
            {
                ContentManager temp = new ContentManager(GraphicsDevice);
                StandardShader = temp.LoadContent<Shader>("Shaders/SpriteTexture");
                StandardShaderMatrixParameter = StandardShader.Parameters["WVP"];
            }
        }

        public void Begin()
        {
            _opList = new List<GraphicOperation>();
            StandardViewProjection = Matrix4.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
                                                                GraphicsDevice.Viewport.Height, 0, -1, 1);
        }
        
        public void Push(GraphicOperation op)
        {
            _opList.Add(op);
        }

        public void End()
        {
            foreach (GraphicOperation t in _opList)
            {
                t.Render();
                t.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StandardShader.Dispose();
            }
        }
    }
}
