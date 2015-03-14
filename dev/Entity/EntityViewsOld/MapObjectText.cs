/***************************************************************************
 *   MapObjectText.cs
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
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaData.Fonts;
#endregion

namespace UltimaXNA.UltimaWorld.View
{
    public class MapObjectText : AMapObject
    {
        public Texture2D Texture { get; internal set; }
        public int Hue;
        public int FontID;

        public MapObjectText(Position3D position, BaseEntity ownerEntity, string text, int hue, int fontID)
            : base(position)
        {
            
            OwnerEntity = ownerEntity;
            Hue = hue;
            FontID = fontID;

            // set up draw data
            m_draw_texture = Texture = ASCIIText.GetTextTexture(text, 1);
            m_draw_width = m_draw_texture.Width;
            m_draw_height = m_draw_texture.Height;
            m_draw_X = (m_draw_width >> 1) - 22 - (int)((Position.X_offset - Position.Y_offset) * 22);
            m_draw_Y = ((int)Position.Z_offset * 4) + m_draw_height - 44 - (int)((Position.X_offset + Position.Y_offset) * 22);
            m_draw_hue = Utility.GetHueVector(Hue);
            m_pickType = PickTypes.PickObjects;
            m_draw_flip = false;
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            return base.Draw(sb, drawPosition, molist, pickType, maxAlt);
        }

        public override string ToString()
        {
            return string.Format("MapObjectText\n   Hue:{1} Font:{2}\n{0}", base.ToString(), Hue, FontID);
        }
    }
}
