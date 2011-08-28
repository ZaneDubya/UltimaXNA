/***************************************************************************
 *   SpriteBatch3D.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Graphics
{
    public class SpriteBatchUI
    {
        SpriteBatch3D _sb;

        public GraphicsDevice GraphicsDevice
        {
            get { return _sb.Game.GraphicsDevice; }
        }

        public SpriteBatchUI(Game game)
        {
            _sb = new SpriteBatch3D(game);
        }

        public void Prepare()
        {
            _sb.Prepare(false, false);
        }

        public void Flush()
        {
            _sb.Flush();
        }

        Vector2 hueVector(int hue, bool partial, bool transparent)
        {
            int hueFlag = 0;
            if (hue != 0)
            {
                if (partial)
                {
                    hueFlag = 2;
                    hue -= 1;
                }
                else
                    hueFlag = 1;
            }
            if (transparent)
                hueFlag |= 4;
            return new Vector2(hue, hueFlag);
        }

        public void Draw2D(Texture2D texture, Point2D position, int hue, bool partial, bool transparent)
        {
            _sb.DrawSimple(texture, new Vector3(position.X, position.Y, 0), hueVector(hue, partial, transparent));
        }

        public void Draw2D(Texture2D texture, Point2D position, Rectangle sourceRect, int hue, bool partial, bool transparent)
        {
            _sb.DrawSimple(texture, new Vector3(position.X, position.Y, 0), sourceRect, hueVector(hue, partial, transparent));
        }

        public void Draw2D(Texture2D texture, Rectangle destRect, Rectangle sourceRect, int hue, bool partial, bool transparent)
        {
            _sb.DrawSimple(texture, destRect, sourceRect, hueVector(hue, partial, transparent));
        }

        public void Draw2D(Texture2D texture, Rectangle destRect, int hue, bool partial, bool transparent)
        {
            _sb.DrawSimple(texture, destRect, hueVector(hue, partial, transparent));
        }



        internal void Draw2DTiled(Texture2D texture, Rectangle destRect, int hue, bool partial, bool transparent)
        {
            _sb.DrawSimpleTiled(texture, destRect, hueVector(hue, partial, transparent));
        }
    }
}
