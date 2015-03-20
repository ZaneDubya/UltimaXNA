/***************************************************************************
 *   ItemGumpling.cs
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
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class ItemGumpling : Control
    {
        protected Texture2D m_texture = null;
        Item m_item;
        public Item Item { get { return m_item; } }
        public Serial ContainerSerial { get { return m_item.Parent.Serial; } }
        public bool CanPickUp = true;

        bool clickedCanDrag = false;
        float pickUpTime;
        Point m_ClickPoint;
        bool sendClickIfNoDoubleClick = false;
        float singleClickTime;

        public ItemGumpling(Control owner, Item item)
            : base(owner, 0)
        {
            buildGumpling(item);
            HandlesMouseInput = true;
        }

        void buildGumpling(Item item)
        {
            Position = item.InContainerPosition;
            m_item = item;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_item.IsDisposed)
            {
                Dispose();
                return;
            }
           

            if (clickedCanDrag && UltimaVars.EngineVars.TheTime >= pickUpTime)
            {
                clickedCanDrag = false;
                AttemptPickUp();
            }

            if (sendClickIfNoDoubleClick && UltimaVars.EngineVars.TheTime >= singleClickTime)
            {
                sendClickIfNoDoubleClick = false;
                UltimaInteraction.SingleClick(m_item);
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            if (m_texture == null)
            {
                m_texture = UltimaData.ArtData.GetStaticTexture(m_item.DisplayItemID);
                Size = new Point(m_texture.Width, m_texture.Height);
            }
            spriteBatch.Draw2D(m_texture, Position, m_item.Hue, false, false);
            base.Draw(spriteBatch);
        }

        protected override bool m_hitTest(int x, int y)
        {
            Color[] pixelData;
            pixelData = new Color[1];
            m_texture.GetData<Color>(0, new Rectangle(x, y, 1, 1), pixelData, 0, 1);
            if (pixelData[0].A > 0)
                return true;
            else
                return false;
        }

        protected override void mouseDown(int x, int y, MouseButton button)
        {
            // if click, we wait for a moment before picking it up. This allows a single click.
            clickedCanDrag = true;
            pickUpTime = UltimaVars.EngineVars.TheTime + UltimaVars.EngineVars.SecondsBetweenClickAndPickUp;
            m_ClickPoint = new Point(x, y);
        }

        protected override void mouseOver(int x, int y)
        {
            // if we have not yet picked up the item, AND we've moved more than 3 pixels total away from the original item, pick it up!
            if (clickedCanDrag && (Math.Abs(m_ClickPoint.X - x) + Math.Abs(m_ClickPoint.Y - y) > 3))
            {
                clickedCanDrag = false;
                AttemptPickUp();
            }
        }

        protected override void mouseClick(int x, int y, MouseButton button)
        {
            if (clickedCanDrag)
            {
                clickedCanDrag = false;
                sendClickIfNoDoubleClick = true;
                singleClickTime = UltimaVars.EngineVars.TheTime + UltimaVars.EngineVars.SecondsForDoubleClick;
            }
        }

        protected override void mouseDoubleClick(int x, int y, MouseButton button)
        {
            UltimaInteraction.DoubleClick(m_item);
            sendClickIfNoDoubleClick = false;
        }

        protected virtual Point InternalGetPickupOffset(Point offset)
        {
            return offset;
        }

        private void AttemptPickUp()
        {
            if (CanPickUp)
            {
                if (this is ItemGumplingPaperdoll)
                {
                    int w, h;
                    UltimaData.ArtData.GetStaticDimensions(this.Item.DisplayItemID, out w, out h);
                    Point click_point = new Point(w / 2, h / 2);
                    UltimaInteraction.PickupItem(m_item, InternalGetPickupOffset(click_point));
                }
                else
                {
                    UltimaInteraction.PickupItem(m_item, InternalGetPickupOffset(m_ClickPoint));
                }
            }
        }
    }
}
