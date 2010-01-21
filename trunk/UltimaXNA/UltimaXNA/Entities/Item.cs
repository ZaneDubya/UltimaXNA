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
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    public class Item : Entity
    {
        public Entity Parent = null;

		private int _amount;
		public int Amount
		{
			get { return _amount; }
			set
			{
				// if the amount changes from or to one, we need to redraw the item, because of doubled drawing for amount > 1
				if ((_amount == 1 && value != 1) || (_amount != 1 && value == 1))
					HasBeenDrawn = false;

				_amount = value;

				// if there is a special drawing id for this item we need to redraw it
				if (_ItemID != DisplayItemID) 
                    HasBeenDrawn = false;
			}
		}
        public int AnimationDisplayID = 0;

        public int Hue
        {
            get; internal set;
        }

        public Data.ItemData ItemData;

		private int _ItemID = 0;
        public int ItemID
        {
            get { return _ItemID; }
            set
            {
				_ItemID = value;
                HasBeenDrawn = false;
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

        public int SlotIndex = 0;

        public Item(Serial serial, IWorld world)
			: base(serial, world)
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
            if (_movement.Position.X == x && _movement.Position.Y == y)
				return true;
			else
				return false;
		}

        public virtual bool PickUp()
        {
            return true;
        }

        internal override void Draw(MapTile tile, Vector3 offset)
		{
			if (Ignored)
				return;

            tile.Add(new TileEngine.MapObjectItem(DisplayItemID, offset, DrawFacing, this, Hue));
            drawOverheads(tile, offset);
		}

		public override void Dispose()
		{
			// if is worn, let the wearer know we are disposing.
            if (Parent is Mobile)
                ((Mobile)Parent).RemoveItem(Serial);
            else if (Parent is Container)
                ((Container)Parent).RemoveItem(Serial);
			base.Dispose();
		}
    }
}
