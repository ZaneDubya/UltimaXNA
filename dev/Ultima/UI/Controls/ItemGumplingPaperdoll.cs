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
        public int SlotIndex;
        public bool IsFemale;

        int m_HueOverride;
        int m_GumpIndex;

        public ItemGumplingPaperdoll(AControl parent, int x, int y, Item item)
            : base(parent, item)
        {
            Position = new Point(x, y);
            HighlightOnMouseOver = false;
        }

        protected override Point InternalGetPickupOffset(Point offset)
        {
            return offset;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            if (m_Texture == null)
            {
                IResourceProvider provider = Services.Get<IResourceProvider>();
                m_GumpIndex = Item.ItemData.AnimID + (IsFemale ? 60000 : 50000);
                int indexTranslated, hueTranslated;
                if (GumpDefTranslator.ItemHasGumpTranslation(m_GumpIndex, out indexTranslated, out hueTranslated))
                {
                    m_GumpIndex = indexTranslated;
                    m_HueOverride = hueTranslated;
                }
                m_Texture = provider.GetUITexture(m_GumpIndex);
                Size = new Point(m_Texture.Width, m_Texture.Height);
            }
            int hue = (Item.Hue == 0 & m_HueOverride != 0) ? m_HueOverride : Item.Hue;
            spriteBatch.Draw2D(m_Texture, new Vector3(position.X, position.Y, 0), Utility.GetHueVector(hue));
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            IResourceProvider provider = Services.Get<IResourceProvider>();
            return provider.IsPointInUITexture(m_GumpIndex, x, y);
        }
    }
}
