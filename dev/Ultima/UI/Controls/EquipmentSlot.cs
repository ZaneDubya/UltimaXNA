/***************************************************************************
 *   EquipmentSlot.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using System;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.UI.Controls
{
    class EquipmentSlot : AControl
    {
        private WorldModel m_World;

        private Mobile m_Entity;
        private EquipLayer m_EquipLayer;
        private Item m_Item;
        private StaticPic m_ItemGraphic;

        private bool m_ClickedCanDrag = false;
        private float m_PickUpTime;
        private Point m_ClickPoint;
        private bool m_SendClickIfNoDoubleClick = false;
        private float m_SingleClickTime;

        public EquipmentSlot(AControl parent, int x, int y, Mobile entity, EquipLayer layer)
            : base(parent)
        {
            HandlesMouseInput = true;

            m_Entity = entity;
            m_EquipLayer = layer;
            
            Position = new Point(x, y);
            AddControl(new GumpPicTiled(this, 0, 0, 19, 20, 0x243A));
            AddControl(new GumpPic(this, 0, 0, 0x2344, 0));

            m_World = ServiceRegistry.GetService<WorldModel>();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_Item != null && m_Item.IsDisposed)
            {
                m_Item = null;
                m_ItemGraphic.Dispose();
                m_ItemGraphic = null;
            }

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

            if (m_Item != null)
            {
                if (m_ClickedCanDrag && UltimaGame.TotalMS >= m_PickUpTime)
                {
                    m_ClickedCanDrag = false;
                    AttemptPickUp();
                }

                if (m_SendClickIfNoDoubleClick && UltimaGame.TotalMS >= m_SingleClickTime)
                {
                    m_SendClickIfNoDoubleClick = false;
                    m_World.Interaction.SingleClick(m_Item);
                }
            }

            base.Update(totalMS, frameMS);

            if (m_ItemGraphic != null)
                m_ItemGraphic.Position = new Point(0 - 14, 0);
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            // if click, we wait for a moment before picking it up. This allows a single click.
            m_ClickedCanDrag = true;
            m_PickUpTime = (float)UltimaGame.TotalMS + Settings.World.Mouse.ClickAndPickupMS;
            m_ClickPoint = new Point(x, y);
        }

        protected override void OnMouseOver(int x, int y)
        {
            // if we have not yet picked up the item, AND we've moved more than 3 pixels total away from the original item, pick it up!
            if (m_ClickedCanDrag && (Math.Abs(m_ClickPoint.X - x) + Math.Abs(m_ClickPoint.Y - y) > 3))
            {
                m_ClickedCanDrag = false;
                AttemptPickUp();
            }
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (m_ClickedCanDrag)
            {
                m_ClickedCanDrag = false;
                m_SendClickIfNoDoubleClick = true;
                m_SingleClickTime = (float)UltimaGame.TotalMS + Settings.World.Mouse.DoubleClickMS;
            }
        }

        protected override void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
            m_World.Interaction.DoubleClick(m_Item);
            m_SendClickIfNoDoubleClick = false;
        }

        private void AttemptPickUp()
        {
            int w, h;
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            provider.GetItemDimensions(m_Item.DisplayItemID, out w, out h);
            Point clickPoint = new Point(w / 2, h / 2);
            m_World.Interaction.PickupItem(m_Item, clickPoint);
        }
    }
}
