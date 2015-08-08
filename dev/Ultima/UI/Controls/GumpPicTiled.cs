/***************************************************************************
 *   GumpPicTiled.cs
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
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    class GumpPicTiled : AControl
    {
        Texture2D m_bgGump = null;
        int m_gumpID;

        public GumpPicTiled(AControl parent)
            : base(parent)
        {

        }

        public GumpPicTiled(AControl parent, string[] arguements)
            : this(parent)
        {
            int x, y, gumpID, width, height;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            gumpID = Int32.Parse(arguements[5]);
            buildGumpling(x, y, width, height, gumpID);
        }

        public GumpPicTiled(AControl parent, int x, int y, int width, int height, int gumpID)
            : this(parent)
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
                IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                m_bgGump = provider.GetUITexture(m_gumpID);
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
