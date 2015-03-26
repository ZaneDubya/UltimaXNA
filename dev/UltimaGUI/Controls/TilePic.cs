/***************************************************************************
 *   TilePic.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Rendering;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class TilePic : Control
    {
        Texture2D m_texture = null;
        int Hue;
        int m_tileID;

        public TilePic(Control owner, int page)
            : base(owner, page)
        {

        }

        public TilePic(Control owner, int page, string[] arguements)
            : this(owner, page)
        {
            int x, y, tileID, hue = 0;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            tileID = Int32.Parse(arguements[3]);
            if (arguements.Length > 4)
            {
                // has a HUE="XXX" arguement!
                hue = Int32.Parse(arguements[4]);
            }
            buildGumpling(x, y, tileID, hue);
        }

        public TilePic(Control owner, int page, int x, int y, int tileID, int hue)
            : this(owner, page)
        {
            buildGumpling(x, y, tileID, hue);
        }

        void buildGumpling(int x, int y, int tileID, int hue)
        {
            Position = new Point(x, y);
            Hue = hue;
            m_tileID = tileID;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_texture == null)
            {
                m_texture = UltimaData.ArtData.GetStaticTexture(m_tileID);
                Size = new Point(m_texture.Width, m_texture.Height);
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            spriteBatch.Draw2D(m_texture, Position, 0, false, false);
            base.Draw(spriteBatch);
        }
    }
}
