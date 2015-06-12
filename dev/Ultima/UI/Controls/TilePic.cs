/***************************************************************************
 *   TilePic.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    class TilePic : AControl
    {
        Texture2D m_texture = null;
        int Hue;
        int m_tileID;

        public TilePic(AControl owner)
            : base(owner)
        {

        }

        public TilePic(AControl owner, string[] arguements)
            : this(owner)
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

        public TilePic(AControl owner, int x, int y, int tileID, int hue)
            : this(owner)
        {
            buildGumpling(x, y, tileID, hue);
        }

        void buildGumpling(int x, int y, int tileID, int hue)
        {
            Position = new Point(x, y);
            Hue = hue;
            m_tileID = tileID;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_texture == null)
            {
                m_texture = IO.ArtData.GetStaticTexture(m_tileID);
                Size = new Point(m_texture.Width, m_texture.Height);
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            spriteBatch.Draw2D(m_texture, new Vector3(position.X, position.Y, 0), Utility.GetHueVector(0, false, false));
            base.Draw(spriteBatch, position);
        }
    }
}
