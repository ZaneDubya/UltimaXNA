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
using UltimaXNA.Ultima.World.Entities;

namespace UltimaXNA.Ultima.World.EntityViews
{
    public class DeferredView : AEntityView
    {
        Vector3 m_DrawPosition;
        AEntityView m_BaseView;

        public DeferredView(DeferredEntity entity, Vector3 drawPosition, AEntityView baseView)
            : base(entity)
        {
            m_DrawPosition = drawPosition;
            m_BaseView = baseView;
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (m_BaseView.Entity is Mobile)
            { 
                Mobile mobile = m_BaseView.Entity as Mobile;
                if (!mobile.IsAlive || mobile.IsDisposed || mobile.Body == 0)
                {
                    Entity.Dispose();
                    return false;
                }
            }
            /*m_BaseView.SetYClipLine(m_DrawPosition.Y - 22 -
                ((m_BaseView.Entity.Position.Z + m_BaseView.Entity.Position.Z_offset) * 4) +
                ((m_BaseView.Entity.Position.X_offset + m_BaseView.Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF));*/
            bool success = m_BaseView.DrawInternal(spriteBatch, m_DrawPosition, mouseOver, map, roofHideFlag);
            /*m_BaseView.ClearYClipLine();*/
            return success;
        }
    }
}
