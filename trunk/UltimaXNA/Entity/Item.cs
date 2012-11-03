/***************************************************************************
 *   Item.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.Interface.TileEngine;
#endregion

namespace UltimaXNA.Entity
{
    public class Item : BaseEntity
    {
        public BaseEntity Parent = null;
        public override Position3D WorldPosition
        {
            get
            {
                if (Parent != null)
                    return Parent.WorldPosition;
                else
                    return Position;
            }
        }
        private BaseEntity _lastParent;
        private int _lastParent_X, _lastParent_Y;
        public bool HasLastParent
        {
            get { return (_lastParent != null); }
        }

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
            if (_movement.Position.X == x && _movement.Position.Y == y)
				return true;
			else
				return false;
		}

        public virtual bool PickUp()
        {
            if (ItemData.Weight == 255)
                return false;
            else
                return true;
        }

        public void SaveLastParent()
        {
            if (Parent != null)
            {
                _lastParent = Parent;
                _lastParent_X = X;
                _lastParent_Y = Y;
            }
            else
            {
                _lastParent = null;
            }
        }

        public void RestoreLastParent()
        {
            this.X = _lastParent_X;
            this.Y = _lastParent_Y;
            ((Container)_lastParent).AddItem(this);
        }

        internal override void Draw(MapTile tile, Position3D position)
		{
			if (Ignored)
				return;

            tile.AddMapObject(new MapObjectItem(DisplayItemID, position, DrawFacing, this, Hue));
            drawOverheads(tile, new Position3D(position.Point_V3));
		}

		public override void Dispose()
		{
            base.Dispose();
			// if is worn, let the wearer know we are disposing.
            if (Parent is Mobile)
                ((Mobile)Parent).RemoveItem(Serial);
            else if (Parent is Container)
                ((Container)Parent).RemoveItem(Serial);
			
		}
    }
}
