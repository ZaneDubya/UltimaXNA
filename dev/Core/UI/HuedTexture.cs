﻿/***************************************************************************
 *   HuedTexture.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public class HuedTexture
    {
        private readonly Texture2D m_Texture;
        private readonly Rectangle m_SourceRect = Rectangle.Empty;

        private Point m_Offset;
        public Point Offset
        {
            set { m_Offset = value; }
            get { return m_Offset; }
        }
        
        private int m_Hue;
        public int Hue
        {
            set { m_Hue = value; }
            get { return m_Hue; }
        }

        private bool m_PartialHue;

        public HuedTexture(Texture2D texture, Point offset, Rectangle source, int hue, bool partialhue=false)
        {
            m_Texture = texture;
            m_Offset = offset;
            m_SourceRect = source;
            m_Hue = hue;
            m_PartialHue = partialhue;
        }

        public void Draw(SpriteBatchUI sb, Point position)
        {
            Vector3 v = new Vector3(position.X - m_Offset.X, position.Y - m_Offset.Y, 0);
            sb.Draw2D(m_Texture, v, m_SourceRect, Utility.GetHueVector(m_Hue, m_PartialHue, false, false));
        }
    }
}
