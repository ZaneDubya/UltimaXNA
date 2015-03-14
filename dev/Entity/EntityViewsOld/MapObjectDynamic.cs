/***************************************************************************
 *   MapObjectDynamic.cs
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
    public class MapObjectDynamic : AMapObject
    {
        private int m_frame;

        public MapObjectDynamic(DynamicObject entity, Position3D position, int itemID, int frame, int hue, bool useGumpArt)
            : base(position)
        {
            OwnerEntity = entity;
            ItemID = itemID;
            m_frame = frame;

            // set up draw variables
            if (useGumpArt)
            {
                m_draw_texture = UltimaData.GumpData.GetGumpXNA(ItemID + frame);
                m_draw_width = m_draw_texture.Width;
                m_draw_height = m_draw_texture.Height;
                m_draw_X = (m_draw_width >> 1);
                m_draw_Y = (int)(Z * 4) + m_draw_height - 22;
            }
            else
            {
                m_draw_texture = UltimaData.ArtData.GetStaticTexture(ItemID + frame);
                m_draw_width = m_draw_texture.Width;
                m_draw_height = m_draw_texture.Height;
                m_draw_X = (m_draw_width >> 1) - 22;
                m_draw_Y = (int)(Z * 4) + m_draw_height - 44;
            }
            m_draw_hue = Utility.GetHueVector(hue);
            m_pickType = PickTypes.PickNothing;
            m_draw_flip = false;
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            return base.Draw(sb, drawPosition, molist, pickType, maxAlt);
        }

        public override string ToString()
        {
            return string.Format("MapObjectDynamic\n   ItemID:{1} Frame:{2{\n{0}", base.ToString(), ItemID, m_frame);
        }
    }
}