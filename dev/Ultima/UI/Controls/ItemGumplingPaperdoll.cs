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
        private bool m_isBuilt = false; // what is this doing?
        private int m_HueOverride = 0;

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
            if (m_Texture == null)
            {
                IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                if (IsFemale)
                {
                    int index = Item.ItemData.AnimID + 60000;
                    int indexTranslated, hueTranslated;
                    if (GumpDefTranslator.ItemHasGumpTranslation(index, out indexTranslated, out hueTranslated))
                    {
                        index = indexTranslated;
                        m_HueOverride = hueTranslated;
                        m_Texture = provider.GetUITexture(index);
                    }
                }
                if (m_Texture == null)
                {
                    int index = Item.ItemData.AnimID + 50000;
                    int indexTranslated, hueTranslated;
                    if (GumpDefTranslator.ItemHasGumpTranslation(index, out indexTranslated, out hueTranslated))
                    {
                        index = indexTranslated;
                        m_HueOverride = hueTranslated;
                        m_Texture = provider.GetUITexture(index);
                    }
                    m_Texture = provider.GetUITexture(index);
                }
                Size = new Point(m_Texture.Width, m_Texture.Height);
            }

            int hue = (Item.Hue == 0 & m_HueOverride != 0) ? m_HueOverride : Item.Hue;
            spriteBatch.Draw2D(m_Texture, new Vector3(position.X, position.Y, 0), Utility.GetHueVector(hue));
            base.Draw(spriteBatch, position);
        }
    }
}
