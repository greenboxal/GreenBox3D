// GraphicOperations.cs
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

namespace GreenBox3D.Graphics
{
    public static class GraphicOperations
    {
        public static void Draw(this GraphicBatch self, Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            Draw(self, texture, destinationRectangle, null, color);
        }

        public static void Draw(this GraphicBatch self, Texture2D texture,
                                Vector2 position,
                                Color color)
        {
            Draw(self, texture, new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height), null,
                 color);
        }

        public static void Draw(this GraphicBatch self, Texture2D texture,
                                Vector2 position, Rectangle? sourceRectangle,
                                Color color)
        {
            Draw(self, texture, new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height),
                 sourceRectangle, color);
        }

        public static void Draw(this GraphicBatch self, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            Rectangle src;

            if (sourceRectangle.HasValue)
                src = sourceRectangle.Value;
            else
                src = new Rectangle(0, 0, texture.Width, texture.Height);

            self.Push(new TextureGraphicOperation(self, texture, destinationRectangle, src, color));
        }
    }
}
