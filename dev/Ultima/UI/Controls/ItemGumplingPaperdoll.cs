/***************************************************************************
 *   ItemGumplingPaperdoll.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities.Items;

namespace UltimaXNA.Ultima.UI.Controls
{
    class ItemGumplingPaperdoll : ItemGumpling
    {
        public int SlotIndex = 0;
        public bool IsFemale = false;

        private int m_x, m_y;
        private bool m_isBuilt = false;

        public ItemGumplingPaperdoll(AControl parent, int x, int y, Item item)
            : base(parent, item)
        {
            m_x = x;
            m_y = y;
            HighlightOnMouseOver = false;
        }

        protected override Point InternalGetPickupOffset(Point offset)
        {
            // fix this to be more centered on the object.
            return offset;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            if (!m_isBuilt)
            {
                Position = new Point(m_x, m_y);
            }
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            if (m_texture == null)
            {
                IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                if (IsFemale)
                    m_texture = provider.GetUITexture(Item.ItemData.AnimID + 60000);
                if (m_texture == null)
                    m_texture = provider.GetUITexture(Item.ItemData.AnimID + 50000);
                Size = new Point(m_texture.Width, m_texture.Height);
            }
            spriteBatch.Draw2D(m_texture, new Vector3(position.X, position.Y, 0), Utility.GetHueVector(Item.Hue));
            base.Draw(spriteBatch, position);
        }
    }
}
