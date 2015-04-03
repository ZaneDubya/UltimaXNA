/***************************************************************************
 *   HuedTexture.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Rendering;
#endregion

namespace UltimaXNA.UltimaGUI
{
    public class HuedTexture
    {
        private Texture2D m_Texture = null;
        private Rectangle m_SourceRect = Rectangle.Empty;

        private Point m_Offset;
        public Point Offset
        {
            set { m_Offset = value; }
            get { return m_Offset; }
        }
        
        private int m_Hue = 0;
        public int Hue
        {
            set { m_Hue = value; }
            get { return m_Hue; }
        }

        public HuedTexture(Texture2D texture, Point offset, Rectangle source, int hue)
        {
            m_Texture = texture;
            m_Offset = offset;
            m_SourceRect = source;
            m_Hue = hue;
        }

        public void Draw(SpriteBatchUI sb, Point position)
        {
            Point p = new Point(position.X - m_Offset.X, position.Y - m_Offset.Y);
            sb.Draw2D(m_Texture, p, m_SourceRect, m_Hue, false, false);
        }
    }
}
