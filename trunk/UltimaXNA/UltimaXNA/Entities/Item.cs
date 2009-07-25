/***************************************************************************
 *   Item.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using System.Collections.Generic;
#endregion

namespace UltimaXNA.Entities
{
    public class Item : Entity
    {
        public Item(Serial serial)
            : base(serial)
        {
        }

        public bool AtWorldPoint(int x, int y)
        {
            if (Movement.DrawPosition.TileX == x && Movement.DrawPosition.TileY == y)
                return true;
            else
                return false;
        }

        internal override void Draw(UltimaXNA.TileEngine.MapCell cell, Vector3 nLocation, Vector3 nOffset)
        {
            cell.Add(new TileEngine.MapObjectItem(_ItemID, nLocation, Movement.DrawFacing, this, Hue));
        }

        public override void Dispose()
        {
            // if is worn, let the wearer know we are disposing.
            if (Wearer != null)
                Wearer.UnWearItem(Serial);
            base.Dispose();
        }

        public Mobile Wearer;
        public int Amount = 0;
        public Serial Item_ContainedWithinSerial = 0;
        public int AnimationDisplayID = 0;

        private int _hue;
        public int Hue
        {
            get { return _hue; }
            set
            {
                _hue = value;
            }
        }

        private int _ItemID = 0;
        public Data.ItemData ItemData;

        public int ItemID
        {
            get { return _ItemID; }
            set
            {
                if (value == 0xEED) // gold coin.
                {
                    if (Amount <= 1)
                        _ItemID = 0xEED;
                    else if (Amount <= 5)
                        _ItemID = 0xEEE;
                    else
                        _ItemID = 0xEEF;
                }
                else
                {
                    _ItemID = value;
                }
                _hasBeenDrawn = false;
                
                ItemData = UltimaXNA.Data.TileData.ItemData[_ItemID];
                AnimationDisplayID = ItemData.AnimID;
            }
        }

        // Inventory position is handled differently in this client than in the legacy UO client.
        // All items have both an X and a Y position within the container. We use X for the SlotIndex
        // which this item occupies, and the Y as a Checksum for the X value: if the Y checksum validates,
        // then we know this item belongs in slot X.
        public int Item_InvX = 0, Item_InvY = 0, Item_InvSlot = 0;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override string ToString()
        {
            return base.ToString() + " | " + ItemData.Name;
        }
    }
}
