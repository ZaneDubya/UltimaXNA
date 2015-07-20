/***************************************************************************
 *   EquipmentSlot.cs
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
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.World.Entities.Items;

namespace UltimaXNA.Ultima.UI.Controls
{
    class EquipmentSlot : AControl
    {
        private Mobile m_Entity;
        private EquipLayer m_EquipLayer;
        private Item m_Item;
        private StaticPic m_ItemGraphic;

        public EquipmentSlot(AControl owner, int x, int y, Mobile entity, EquipLayer layer)
            : base(owner)
        {
            m_Entity = entity;
            m_EquipLayer = layer;
            
            Position = new Point(x, y);
            AddControl(new GumpPicTiled(this, 0, 0, 19, 20, 0x243A));
            AddControl(new GumpPic(this, 0, 0, 0x2344, 0));
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_Item != m_Entity.Equipment[(int)m_EquipLayer])
            {
                if (m_ItemGraphic != null)
                {
                    m_ItemGraphic.Dispose();
                    m_ItemGraphic = null;
                }

                m_Item = m_Entity.Equipment[(int)m_EquipLayer];
                if (m_Item != null)
                {
                    m_ItemGraphic = (StaticPic)AddControl(new StaticPic(this, 0, 0, m_Item.ItemID, m_Item.Hue));
                }
            }

            base.Update(totalMS, frameMS);

            if (m_ItemGraphic != null)
                m_ItemGraphic.Position = new Point(0 - 14, 0);
        }
    }
}
