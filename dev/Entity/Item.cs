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
using UltimaXNA.UltimaWorld;
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
        private BaseEntity m_lastParent;
        private int m_lastParent_X, m_lastParent_Y;
        public bool HasLastParent
        {
            get { return (m_lastParent != null); }
        }

		private int m_amount;
		public int Amount
		{
			get { return m_amount; }
			set
			{
				// if the amount changes from or to one, we need to redraw the item, because of doubled drawing for amount > 1
				if ((m_amount == 1 && value != 1) || (m_amount != 1 && value == 1))
					HasBeenDrawn = false;

				m_amount = value;

				// if there is a special drawing id for this item we need to redraw it
				if (m_ItemID != DisplayItemID) 
                    HasBeenDrawn = false;
			}
		}
        public int AnimationDisplayID = 0;

        public int Hue
        {
            get; internal set;
        }

        public UltimaData.ItemData ItemData;

		private int m_ItemID = 0;
        public int ItemID
        {
            get { return m_ItemID; }
            set
            {
				m_ItemID = value;
                HasBeenDrawn = false;
                ItemData = UltimaXNA.UltimaData.TileData.ItemData[m_ItemID];
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
						return m_ItemID + 2;
					}
					else if (Amount > 1)
					{
						return m_ItemID + 1;
					}
				}

				return m_ItemID;
			}
		}

		public bool Ignored
		{
			get { return m_ItemID <= 1; } // no draw
		}

		public bool IsCoin {
			get { return m_ItemID == 0xEEA || m_ItemID == 0xEED || m_ItemID == 0xEF0; }
		}

        public int SlotIndex = 0;

        public Item(Serial serial)
			: base(serial)
		{
		}

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
        }

        public override string ToString()
        {
            return base.ToString() + " | " + ItemData.Name;
        }

		public bool AtWorldPoint(int x, int y)
		{
            if (m_movement.Position.X == x && m_movement.Position.Y == y)
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
                m_lastParent = Parent;
                m_lastParent_X = X;
                m_lastParent_Y = Y;
            }
            else
            {
                m_lastParent = null;
            }
        }

        public void RestoreLastParent()
        {
            this.X = m_lastParent_X;
            this.Y = m_lastParent_Y;
            ((Container)m_lastParent).AddItem(this);
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
