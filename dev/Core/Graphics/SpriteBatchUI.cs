/***************************************************************************
 *   SpriteBatch3D.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Core.Graphics
{
    public class SpriteBatchUI : SpriteBatch3D
    {
        public SpriteBatchUI(Game game)
            : base(game)
        {

        }

        public bool Draw2D(Texture2D texture, Vector3 position, Vector3 hue)
        {
            VertexPositionNormalTextureHue[] v =
            {
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y, 0), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + texture.Width, position.Y, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y + texture.Height, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + texture.Width, position.Y + texture.Height, 0), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
            };

            v[0].Hue = v[1].Hue = v[2].Hue = v[3].Hue = hue;
            return Draw(texture, v);
        }

        public bool Draw2D(Texture2D texture, Vector3 position, Rectangle sourceRect, Vector3 hue)
        {
            float minX = sourceRect.X / (float)texture.Width;
            float maxX = (sourceRect.X + sourceRect.Width) / (float)texture.Width;
            float minY = sourceRect.Y / (float)texture.Height;
            float maxY = (sourceRect.Y + sourceRect.Height) / (float)texture.Height;

            VertexPositionNormalTextureHue[] v =
            {
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y, 0), new Vector3(0, 0, 1), new Vector3(minX, minY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + sourceRect.Width, position.Y, 0), new Vector3(0, 0, 1), new Vector3(maxX, minY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y + sourceRect.Height, 0), new Vector3(0, 0, 1), new Vector3(minX, maxY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + sourceRect.Width, position.Y + sourceRect.Height, 0), new Vector3(0, 0, 1), new Vector3(maxX, maxY, 0))
            };

            v[0].Hue = v[1].Hue = v[2].Hue = v[3].Hue = hue;
            return Draw(texture, v);
        }

        public bool Draw2D(Texture2D texture, Rectangle destRect, Rectangle sourceRect, Vector3 hue)
        {
            float minX = sourceRect.X / (float)texture.Width, maxX = (sourceRect.X + sourceRect.Width) / (float)texture.Width;
            float minY = sourceRect.Y / (float)texture.Height, maxY = (sourceRect.Y + sourceRect.Height) / (float)texture.Height;

            VertexPositionNormalTextureHue[] v =
            {
                new VertexPositionNormalTextureHue(new Vector3(destRect.X, destRect.Y, 0), new Vector3(0, 0, 1), new Vector3(minX, minY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X + destRect.Width, destRect.Y, 0), new Vector3(0, 0, 1), new Vector3(maxX, minY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X, destRect.Y + destRect.Height, 0), new Vector3(0, 0, 1), new Vector3(minX, maxY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X + destRect.Width, destRect.Y + destRect.Height, 0), new Vector3(0, 0, 1), new Vector3(maxX, maxY, 0))
            };

            v[0].Hue = v[1].Hue = v[2].Hue = v[3].Hue = hue;
            return Draw(texture, v);
        }

        public bool Draw2D(Texture2D texture, Rectangle destRect, Vector3 hue)
        {
            VertexPositionNormalTextureHue[] v =
            {
                new VertexPositionNormalTextureHue(new Vector3(destRect.X, destRect.Y, 0), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X + destRect.Width, destRect.Y, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X, destRect.Y + destRect.Height, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X + destRect.Width, destRect.Y + destRect.Height, 0), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
            };

            v[0].Hue = v[1].Hue = v[2].Hue = v[3].Hue = hue;
            return Draw(texture, v);
        }

        public bool Draw2DTiled(Texture2D texture, Rectangle destRect, Vector3 hue)
        {
            int y = destRect.Y;
            int h = destRect.Height;
            Rectangle sRect;

            while (h > 0)
            {
                int x = destRect.X;
                int w = destRect.Width;
                if (h < texture.Height)
                {
                    sRect = new Rectangle(0, 0, texture.Width, h);
                }
                else
                {
                    sRect = new Rectangle(0, 0, texture.Width, texture.Height);
                }
                while (w > 0)
                {
                    if (w < texture.Width)
                    {
                        sRect.Width = w;
                    }
                    Draw2D(texture, new Vector3(x, y, 0), sRect, hue);
                    w -= texture.Width;
                    x += texture.Width;
                }
                h -= texture.Height;
                y += texture.Height;
            }

            return true;
        }
    }
}
