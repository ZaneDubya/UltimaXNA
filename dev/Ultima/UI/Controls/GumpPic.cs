/***************************************************************************
 *   GumpPic.cs
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
using UltimaXNA.Ultima.UI;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    class GumpPic : AControl
    {
        protected Texture2D m_texture = null;
        int m_gumpID;
        int m_hue;

        private bool m_IsPaperdoll = false;
        internal bool IsPaperdoll
        {
            get { return m_IsPaperdoll; }
            set { m_IsPaperdoll = value; }
        }

        public GumpPic(AControl owner)
            : base(owner)
        {
            MakeThisADragger();
        }

        public GumpPic(AControl owner, string[] arguements)
            : this(owner)
        {
            int x, y, gumpID, hue = 0;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            gumpID = Int32.Parse(arguements[3]);
            if (arguements.Length > 4)
            {
                // has a HUE="XXX" arguement!
                hue = Int32.Parse(arguements[4]);
            }
            buildGumpling(x, y, gumpID, hue);
        }

        public GumpPic(AControl owner, int x, int y, int gumpID, int hue)
            : this(owner)
        {
            buildGumpling(x, y, gumpID, hue);
        }

        void buildGumpling(int x, int y, int gumpID, int hue)
        {
            Position = new Point(x, y);
            m_gumpID = gumpID;
            m_hue = hue;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_texture == null)
            {
                m_texture = IO.GumpData.GetGumpXNA(m_gumpID);
                Size = new Point(m_texture.Width, m_texture.Height);
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            Vector3 hueVector = Utility.GetHueVector(m_hue);
            spriteBatch.Draw2D(m_texture, new Vector3(position.X, position.Y, 0), hueVector);
            base.Draw(spriteBatch, position);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            Color[] pixelData;
            pixelData = new Color[1];
            m_texture.GetData<Color>(0, new Rectangle(x, y, 1, 1), pixelData, 0, 1);
            if (pixelData[0].A > 0)
                return true;
            else
                return false;
        }
    }
}
