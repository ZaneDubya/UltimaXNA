using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;

namespace UltimaXNA.UILegacy.Gumplings
{
    class ItemGumpling : Control
    {
        protected Texture2D _texture = null;
        Item _item;
        public Item Item { get { return _item; } }
        public Serial ContainerSerial { get { return _item.Parent.Serial; } }

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
            Position = new Vector2(item.X, item.Y);
            _item = item;
        }

        public override void Update(GameTime gameTime)
        {
            if (_item.IsDisposed)
            {
                Dispose();
                return;
            }
            if (_texture == null)
            {
                _texture = Data.Art.GetStaticTexture(_item.DisplayItemID);
                Size = new Vector2(_texture.Width, _texture.Height);
            }

            if (clickedCanDrag && ClientVars.TheTime >= pickUpTime)
            {
                clickedCanDrag = false;
                Interaction.PickUpItem(_item, clickPoint.X, clickPoint.Y);
            }

            if (sendClickIfNoDoubleClick && ClientVars.TheTime >= singleClickTime)
            {
                sendClickIfNoDoubleClick = false;
                Interaction.SingleClick(_item);
            }
            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, _item.Hue, false);
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

        protected override void _mouseDown(int x, int y, UltimaXNA.Input.MouseButtons button)
        {
            // if click, we wait for a moment before picking it up. This allows a single click.
            clickedCanDrag = true;
            pickUpTime = ClientVars.TheTime + ClientVars.SecondsBetweenClickAndPickUp;
            clickPoint = new Point(x, y);
        }

        protected override void _mouseOver(int x, int y)
        {
            // if we have not yet picked up the item, AND we've moved more than X pixels total away from the original item, pick it up!
            if (clickedCanDrag && (Math.Abs(clickPoint.X - x) + Math.Abs(clickPoint.Y - y) > 3))
            {
                clickedCanDrag = false;
                Interaction.PickUpItem(_item, clickPoint.X, clickPoint.Y);
            }
        }

        protected override void _mouseClick(int x, int y, UltimaXNA.Input.MouseButtons button)
        {
            if (clickedCanDrag)
            {
                clickedCanDrag = false;
                sendClickIfNoDoubleClick = true;
                singleClickTime = ClientVars.TheTime + ClientVars.SecondsForDoubleClick;
            }
        }

        protected override void _mouseDoubleClick(int x, int y, UltimaXNA.Input.MouseButtons button)
        {
            Interaction.DoubleClick(_item);
            sendClickIfNoDoubleClick = false;
        }
    }
}
