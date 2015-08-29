/***************************************************************************
 *   DeferredView.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.World.WorldViews;

namespace UltimaXNA.Ultima.World.EntityViews
{
    public class DeferredView : AEntityView
    {
        Vector3 m_DrawPosition;
        AEntityView m_BaseView;

        public DeferredView(Vector3 drawPosition, AEntityView baseView)
            : base(baseView.Entity)
        {
            m_DrawPosition = drawPosition;
            m_BaseView = baseView;
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, bool roofHideFlag)
        {
            if (Entity is Mobile)
            { 
                Mobile mobile = Entity as Mobile;
                if (!mobile.IsAlive || mobile.IsDisposed || mobile.Body == 0)
                {
                    return false;
                }
            }

            m_BaseView.SetYClipLine(m_DrawPosition.Y - 22 - ((Entity.Position.Z + Entity.Position.Z_offset) * 4)  + ((Entity.Position.X_offset + Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF));
            bool success = m_BaseView.DrawInternal(spriteBatch, m_DrawPosition, mouseOverList, map, roofHideFlag);
            m_BaseView.ClearYClipLine();
            return success;
        }
    }
}
