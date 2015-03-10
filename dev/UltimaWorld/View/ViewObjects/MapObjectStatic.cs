/***************************************************************************
 *   MapObjectStatic.cs
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
    public class MapObjectStatic : AMapObject
    {
        bool m_noDraw = false;
        public bool NoDraw
        {
            get { return (m_noDraw); }
        }

        UltimaData.ItemData m_itemData;
        public UltimaData.ItemData ItemData
        {
            get { return m_itemData; }
        }

        public MapObjectStatic(int staticTileID, int sortInfluence, Position3D position)
            : base(position)
        {
            ItemID = staticTileID;
            SortTiebreaker = sortInfluence;

            m_itemData = UltimaData.TileData.ItemData[ItemID & 0x3FFF];

            // Set threshold.
            int background = (m_itemData.IsBackground) ? 0 : 1;
            if (!m_itemData.IsBackground)
                SortThreshold++;
            if (!(m_itemData.Height == 0))
                SortThreshold++;
            if (m_itemData.IsSurface)
                SortThreshold--;

            // get no draw flag
            if (m_itemData.Name == "nodraw" || ItemID <= 0)
                m_noDraw = true;

            // set up draw variables
            m_draw_texture = UltimaData.ArtData.GetStaticTexture(ItemID);
            m_draw_width = m_draw_texture.Width;
            m_draw_height = m_draw_texture.Height;
            m_draw_X = (m_draw_width >> 1) - 22;
            m_draw_Y = (int)(Z * 4) + m_draw_height - 44;
            m_draw_hue = Vector2.Zero;
            m_pickType = PickTypes.PickStatics;
            m_draw_flip = false;
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            m_draw_Y = (int)(Z * 4) + m_draw_height - 44;
            if (m_noDraw)
                return false;
            return base.Draw(sb, drawPosition, molist, pickType, maxAlt);
        }

        public override string ToString()
        {
            return string.Format("MapObjectStatic\n   ItemID:{1}\n{0}", base.ToString(), ItemID);
        }
    }
}