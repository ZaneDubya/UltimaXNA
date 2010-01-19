﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy
{
    public class ExtendedSpriteBatch
    {
        SpriteBatch3D _sb;

        public GraphicsDevice GraphicsDevice
        {
            get { return _sb.Game.GraphicsDevice; }
        }

        public ExtendedSpriteBatch(Game game)
        {
            _sb = new SpriteBatch3D(game);
        }

        public void Flush()
        {
            _sb.FlushOld(false);
        }

        Vector2 hueVector(int hue, bool hueOnlyGreyPixels)
        {
            int hueFlag = 0;
            if (hue != 0)
            {
                if (hueOnlyGreyPixels)
                    hueFlag = 2;
                else
                    hueFlag = 1;
            }
            return new Vector2(hue, hueFlag);
        }

        public void Draw(Texture2D texture, Vector2 position, int hue, bool hueOnlyGreyPixels)
        {
            _sb.DrawSimple(texture, new Vector3(position.X, position.Y, SpriteBatch3D.Z), hueVector(hue, hueOnlyGreyPixels));
            SpriteBatch3D.Z += 1000;
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle sourceRect, int hue, bool hueOnlyGreyPixels)
        {
            _sb.DrawSimple(texture, new Vector3(position.X, position.Y, SpriteBatch3D.Z), sourceRect, hueVector(hue, hueOnlyGreyPixels));
            SpriteBatch3D.Z += 1000;
        }

        public void Draw(Texture2D texture, Rectangle destRect, Rectangle sourceRect, int hue, bool hueOnlyGreyPixels)
        {
            _sb.DrawSimple(texture, destRect, SpriteBatch3D.Z, sourceRect, hueVector(hue, hueOnlyGreyPixels));
            SpriteBatch3D.Z += 1000;
        }

        public void Draw(Texture2D texture, Rectangle destRect, int hue, bool hueOnlyGreyPixels)
        {
            _sb.DrawSimple(texture, destRect, SpriteBatch3D.Z, hueVector(hue, hueOnlyGreyPixels));
            SpriteBatch3D.Z += 1000;
        }

        internal void DrawTiled(Texture2D texture, Rectangle destRect, int hue, bool hueOnlyGreyPixels)
        {
            _sb.DrawSimpleTiled(texture, destRect, SpriteBatch3D.Z, hueVector(hue, hueOnlyGreyPixels));
            SpriteBatch3D.Z += 1000;
        }
    }
}
