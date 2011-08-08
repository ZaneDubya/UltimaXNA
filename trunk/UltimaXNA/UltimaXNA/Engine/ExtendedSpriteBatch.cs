using System;
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
            _sb.Flush(false);
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
