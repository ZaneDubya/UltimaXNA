/***************************************************************************
 *   GumpPicTiled.cs
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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.UI.Controls
{
    class GumpPicTiled : AControl
    {
        Texture2D m_bgGump = null;
        int m_gumpID;

        public GumpPicTiled(AControl owner)
            : base(owner)
        {

        }

        public GumpPicTiled(AControl owner, string[] arguements)
            : this(owner)
        {
            int x, y, gumpID, width, height;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            gumpID = Int32.Parse(arguements[5]);
            buildGumpling(x, y, width, height, gumpID);
        }

        public GumpPicTiled(AControl owner, int x, int y, int width, int height, int gumpID)
            : this(owner)
        {
            buildGumpling(x, y, width, height, gumpID);
        }

        void buildGumpling(int x, int y, int width, int height, int gumpID)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            m_gumpID = gumpID;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_bgGump == null)
            {
                m_bgGump = GumpData.GetGumpXNA(m_gumpID);
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            spriteBatch.Draw2DTiled(m_bgGump, new Rectangle(position.X, position.Y, Width, Height), Vector3.Zero);
            base.Draw(spriteBatch, position);
        }
    }
}
