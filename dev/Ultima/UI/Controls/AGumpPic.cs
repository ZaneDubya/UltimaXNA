/***************************************************************************
 *   AGumpPic.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    abstract class AGumpPic : AControl
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

        public AGumpPic(AControl parent)
            : base(parent)
        {
            MakeThisADragger();
        }

        protected void BuildGumpling(int x, int y, int gumpID, int hue)
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
                IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                m_Texture = provider.GetUITexture(GumpID);
                Size = new Point(m_Texture.Width, m_Texture.Height);
            }
            base.Update(totalMS, frameMS);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            return provider.IsPointInUITexture(GumpID, x, y);
        }
    }
}