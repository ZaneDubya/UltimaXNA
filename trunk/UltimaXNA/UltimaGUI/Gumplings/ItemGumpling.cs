/***************************************************************************
 *   ItemGumpling.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.Interface.Graphics;
using UltimaXNA.Interface.Input;
using UltimaXNA.Interface.GUI;

namespace UltimaXNA.UltimaGUI.Gumplings
{
    class ItemGumpling : Control
    {
        protected Texture2D _texture = null;
        Item _item;
        public Item Item { get { return _item; } }
        public Serial ContainerSerial { get { return _item.Parent.Serial; } }
        public bool CanPickUp = true;

        bool clickedCanDrag = false;
        float pickUpTime; Point clickPoint;
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
            Position = new Point2D(item.X, item.Y);
            _item = item;
        }

        public override void Update(GameTime gameTime)
        {
            if (_item.IsDisposed)
            {
                Dispose();
                return;
            }
           

            if (clickedCanDrag && UltimaVars.EngineVars.TheTime >= pickUpTime)
            {
                clickedCanDrag = false;
                UltimaInteraction.PickUpItem(_item, clickPoint.X, clickPoint.Y);
            }

            if (sendClickIfNoDoubleClick && UltimaVars.EngineVars.TheTime >= singleClickTime)
            {
                sendClickIfNoDoubleClick = false;
                UltimaInteraction.SingleClick(_item);
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            if (_texture == null)
            {
                _texture = UltimaData.Art.GetStaticTexture(_item.DisplayItemID);
                Size = new Point2D(_texture.Width, _texture.Height);
            }
            spriteBatch.Draw2D(_texture, Position, _item.Hue, false, false);
            base.Draw(spriteBatch);
        }

        protected override bool _hitTest(int x, int y)
        {
            Color[] pixelData;
            pixelData = new Color[1];
            _texture.GetData<Color>(0, new Rectangle(x, y, 1, 1), pixelData, 0, 1);
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
            clickPoint = new Point(x, y);
        }

        protected override void mouseOver(int x, int y)
        {
            // if we have not yet picked up the item, AND we've moved more than X pixels total away from the original item, pick it up!
            if (clickedCanDrag && (Math.Abs(clickPoint.X - x) + Math.Abs(clickPoint.Y - y) > 3))
            {
                clickedCanDrag = false;
                if (CanPickUp)
                {
                    UltimaInteraction.PickUpItem(_item, clickPoint.X, clickPoint.Y);
                    _onPickUp();
                }
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
            UltimaInteraction.DoubleClick(_item);
            sendClickIfNoDoubleClick = false;
        }

        protected override void itemDrop(Item item, int x, int y)
        {
            // If we drop onto a container object, drop *inside* the container object.
            if (_item.ItemData.Container)
            {
                UltimaInteraction.DropItemToContainer(item, (Container)_item);
            }
        }

        protected virtual void _onPickUp()
        {

        }
    }
}
