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
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    /// <summary>
    /// A gump that shows a static item.
    /// </summary>
    class StaticPic : AControl
    {
        Texture2D m_texture = null;
        int Hue;
        int m_tileID;

        public StaticPic(AControl parent)
            : base(parent)
        {

        }

        public StaticPic(AControl parent, string[] arguements)
            : this(parent)
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

        public StaticPic(AControl parent, int x, int y, int itemID, int hue)
            : this(parent)
        {
            buildGumpling(x, y, itemID, hue);
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
                IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                m_texture = provider.GetItemTexture(m_tileID);
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
