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
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.Network.Client;

namespace UltimaXNA.Ultima.UI.Controls
{
    class ItemGumpling : AControl
    {
        protected Texture2D m_texture = null;
        public Serial ContainerSerial { get { return Item.Parent.Serial; } }
        public bool CanPickUp = true;
        public bool HighlightOnMouseOver = true;

        bool clickedCanDrag = false;
        float pickUpTime;
        Point m_ClickPoint;
        bool sendClickIfNoDoubleClick = false;
        float singleClickTime;

        private readonly WorldModel m_World;

        public Item Item
        {
            get;
            private set;
        }

        public ItemGumpling(AControl owner, Item item)
            : base(owner, 0)
        {
            buildGumpling(item);
            HandlesMouseInput = true;

            m_World = ServiceRegistry.GetService<WorldModel>();
        }

        void buildGumpling(Item item)
        {
            Position = item.InContainerPosition;
            Item = item;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (Item.IsDisposed)
            {
                Dispose();
                return;
            }
           

            if (clickedCanDrag && UltimaEngine.TotalMS >= pickUpTime)
            {
                clickedCanDrag = false;
                AttemptPickUp();
            }

            if (sendClickIfNoDoubleClick && UltimaEngine.TotalMS >= singleClickTime)
            {
                sendClickIfNoDoubleClick = false;
                m_World.Interaction.SingleClick(Item);
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            if (m_texture == null)
            {
                m_texture = IO.ArtData.GetStaticTexture(Item.DisplayItemID);
                Size = new Point(m_texture.Width, m_texture.Height);
            }
            Vector3 hue = Utility.GetHueVector(IsMouseOver && HighlightOnMouseOver ? EngineVars.MouseOverHue : Item.Hue);
            spriteBatch.Draw2D(m_texture, new Vector3(position.X, position.Y, 0), hue);
            base.Draw(spriteBatch, position);
        }

        protected override bool InternalHitTest(int x, int y)
        {
            // Allow selection if there is a non-transparent pixel below the mouse cursor or at an offset of
            // (-1,0), (0,-1), (1,0), or (1,1). This will allow selection even when the mouse cursor is directly
            // over a transparent pixel, and will also increase the 'selection space' of an item by one pixel in
            // each dimension - thus a very thin object (2-3 pixels wide) will be increased.
            Color[] pixelData;

            if (x == 0)
                x++;
            if (x == m_texture.Width - 1)
                x--;
            if (y == 0)
                y++;
            if (y == m_texture.Height - 1)
                y--;

            pixelData = new Color[9];
            m_texture.GetData<Color>(0, new Rectangle(x - 1, y - 1, 3, 3), pixelData, 0, 9);
            if ((pixelData[1].A > 0) || (pixelData[3].A > 0) ||
                (pixelData[4].A > 0) || (pixelData[5].A > 0) ||
                (pixelData[7].A > 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            // if click, we wait for a moment before picking it up. This allows a single click.
            clickedCanDrag = true;
            pickUpTime = (float)UltimaEngine.TotalMS + EngineVars.ClickAndPickUpMS;
            m_ClickPoint = new Point(x, y);
        }

        protected override void OnMouseOver(int x, int y)
        {
            // if we have not yet picked up the item, AND we've moved more than 3 pixels total away from the original item, pick it up!
            if (clickedCanDrag && (Math.Abs(m_ClickPoint.X - x) + Math.Abs(m_ClickPoint.Y - y) > 3))
            {
                clickedCanDrag = false;
                AttemptPickUp();
            }
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (clickedCanDrag)
            {
                clickedCanDrag = false;
                sendClickIfNoDoubleClick = true;
                singleClickTime = (float)UltimaEngine.TotalMS + EngineVars.DoubleClickMS;
            }
        }

        protected override void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
            m_World.Interaction.DoubleClick(Item);
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
                    IO.ArtData.GetStaticDimensions(Item.DisplayItemID, out w, out h);
                    Point click_point = new Point(w / 2, h / 2);
                    m_World.Interaction.PickupItem(Item, InternalGetPickupOffset(click_point));
                }
                else
                {
                    m_World.Interaction.PickupItem(Item, InternalGetPickupOffset(m_ClickPoint));
                }
            }
        }
    }
}
