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
        public Mobile Wearer;

		private int _Amount;
		public int Amount
		{
			get { return _Amount; }
			set
			{
				// if the amount changes from or to one, we need to redraw the item, because of doubled drawing for amount > 1
				if ((_Amount == 1 && value != 1) || (_Amount != 1 && value == 1))
				{
					_hasBeenDrawn = false;
				}

				_Amount = value;

				// if there is a special drawing id for this item we need to redraw it
				if (_ItemID != DisplayItemID) 
				{
					_hasBeenDrawn = false;
				}
			}
		}
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

        public Data.ItemData ItemData;

		private int _ItemID = 0;
        public int ItemID
        {
            get { return _ItemID; }
            set
            {
				_ItemID = value;
				
                _hasBeenDrawn = false;
                
                ItemData = UltimaXNA.Data.TileData.ItemData[_ItemID];
                AnimationDisplayID = ItemData.AnimID;
            }
        }

		public int DisplayItemID
		{
			get
			{
				if (IsCoin)
				{
					if (Amount > 5)
					{
						return _ItemID + 2;
					}
					else if (Amount > 1)
					{
						return _ItemID + 1;
					}
				}

				return _ItemID;
			}
		}

		public bool Ignored
		{
			get { return _ItemID <= 1; } // no draw
		}

		public bool IsCoin {
			get { return _ItemID == 0xEEA || _ItemID == 0xEED || _ItemID == 0xEF0; }
		}

        // Inventory position is handled differently in this client than in the legacy UO client.
        // All items have both an X and a Y position within the container. We use X for the SlotIndex
        // which this item occupies, and the Y as a Checksum for the X value: if the Y checksum validates,
        // then we know this item belongs in slot X.
        public int Item_InvX = 0, Item_InvY = 0, Item_InvSlot = 0;

		public Item(Serial serial)
			: base(serial)
		{
		}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override string ToString()
        {
            return base.ToString() + " | " + ItemData.Name;
        }

		public bool AtWorldPoint(int x, int y)
		{
			if (Movement.DrawPosition.TileX == x && Movement.DrawPosition.TileY == y)
				return true;
			else
				return false;
		}

		internal override void Draw(UltimaXNA.TileEngine.MapCell cell, Vector3 position, Vector3 positionOffset)
		{
			if (Ignored)
			{
				return;
			}

			cell.Add(new TileEngine.MapObjectItem(DisplayItemID, position, Movement.DrawFacing, this, Hue));
			drawOverheads(cell, position, positionOffset);
		}

		public override void Dispose()
		{
			// if is worn, let the wearer know we are disposing.
			if (Wearer != null)
				Wearer.UnWearItem(Serial);
			base.Dispose();
		}
    }
}
