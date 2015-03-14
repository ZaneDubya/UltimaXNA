/***************************************************************************
 *   MapObjectItem.cs
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
#endregion

namespace UltimaXNA.UltimaWorld.View
{
    public class MapObjectItem : AMapObject
    {
        public int Hue { get; set; }
        public int Facing { get; set; }

        public MapObjectItem(int itemID, Position3D position, int direction, BaseEntity ownerEntity, int hue)
            : base(position)
        {
            ItemID = itemID;
            OwnerEntity = ownerEntity;
            Facing = direction;
            Hue = hue;

            // set up draw data
            m_draw_texture = UltimaData.ArtData.GetStaticTexture(ItemID);
            m_draw_width = m_draw_texture.Width;
            m_draw_height = m_draw_texture.Height;
            m_draw_X = (m_draw_width >> 1) - 22;
            m_draw_Y = (int)(Z * 4) + m_draw_height - 44;
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
            return string.Format("MapObjectItem\n   ItemID:{3} Serial{4}\n   Hue:{1} Facing:{2}\n{0}", base.ToString(), Hue, Facing, ItemID, OwnerSerial);
        }
    }
}
