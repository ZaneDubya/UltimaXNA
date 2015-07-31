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
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.UI.Controls
{
    class GumpPic : AControl
    {
        protected Texture2D m_Texture = null;
        private int m_LastFrameGumpID = -1;

        internal int GumpID
        {
            get;
            set;
        }

        internal int Hue
        {
            get;
            set;
        }

        internal bool IsPaperdoll
        {
            get;
            set;
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
                // has a HUE=XXX arguement (and potentially a CLASS=XXX argument).
                string hueArgument = arguements[4].Substring(arguements[4].IndexOf('=') + 1);
                hue = Int32.Parse(hueArgument);
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
            GumpID = gumpID;
            Hue = hue;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_Texture == null || GumpID != m_LastFrameGumpID)
            {
                m_LastFrameGumpID = GumpID;
                m_Texture = GumpData.GetGumpXNA(GumpID);
                Size = new Point(m_Texture.Width, m_Texture.Height);
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            Vector3 hueVector = Utility.GetHueVector(Hue);
            spriteBatch.Draw2D(m_Texture, new Vector3(position.X, position.Y, 0), hueVector);
            base.Draw(spriteBatch, position);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            ushort[] pixelData;
            pixelData = new ushort[1];
            m_Texture.GetData<ushort>(0, new Rectangle(x, y, 1, 1), pixelData, 0, 1);
            if (pixelData[0] > 0)
                return true;
            else
                return false;
        }
    }
}
