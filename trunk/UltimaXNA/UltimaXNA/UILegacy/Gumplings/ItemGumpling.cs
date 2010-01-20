﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;

namespace UltimaXNA.UILegacy.Gumplings
{
    class ItemGumpling : Control
    {
        protected Texture2D _texture = null;
        Item _item;

        public ItemGumpling(Control owner, Item item)
            : base(owner, 0)
        {
            buildGumpling(item);
        }

        void buildGumpling(Item item)
        {
            Position = new Vector2(item.X, item.Y);
            _item = item;
        }

        public override void Update(GameTime gameTime)
        {
            if (_texture == null)
            {
                _texture = Data.Art.GetStaticTexture(_item.DisplayItemID);
                Size = new Vector2(_texture.Width, _texture.Height);
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
    }
}
