/***************************************************************************
 *   SpriteBatch3D.cs
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
    public class SpriteBatchUI
    {
        SpriteBatch3D m_SpriteBatch;

        public GraphicsDevice GraphicsDevice
        {
            get { return m_SpriteBatch.GraphicsDevice; }
        }

        public SpriteBatchUI(Game game)
        {
            m_SpriteBatch = new SpriteBatch3D(game);
        }

        public void Prepare()
        {
            m_SpriteBatch.Prepare(false, true);
        }

        public void Flush()
        {
            m_SpriteBatch.Flush();
        }

        public void Draw2D(Texture2D texture, Point position, int hue, bool partial, bool transparent)
        {
            m_SpriteBatch.DrawSimple(texture, new Vector3(position.X, position.Y, 0), Utility.GetHueVector(hue, partial, transparent));
        }

        public void Draw2D(Texture2D texture, Point position, Rectangle sourceRect, int hue, bool partial, bool transparent)
        {
            m_SpriteBatch.DrawSimple(texture, new Vector3(position.X, position.Y, 0), sourceRect, Utility.GetHueVector(hue, partial, transparent));
        }

        public void Draw2D(Texture2D texture, Rectangle destRect, Rectangle sourceRect, int hue, bool partial, bool transparent)
        {
            m_SpriteBatch.DrawSimple(texture, destRect, sourceRect, Utility.GetHueVector(hue, partial, transparent));
        }

        public void Draw2D(Texture2D texture, Rectangle destRect, int hue, bool partial, bool transparent)
        {
            m_SpriteBatch.DrawSimple(texture, destRect, Utility.GetHueVector(hue, partial, transparent));
        }

        internal void Draw2DTiled(Texture2D texture, Rectangle destRect, int hue, bool partial, bool transparent)
        {
            m_SpriteBatch.DrawSimpleTiled(texture, destRect, Utility.GetHueVector(hue, partial, transparent));
        }
    }
}
