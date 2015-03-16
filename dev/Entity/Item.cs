/***************************************************************************
 *   Item.cs
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
using UltimaXNA.UltimaWorld.View;
#endregion

namespace UltimaXNA.Entity
{
    public class Item : AEntity
    {
        public AEntity Parent = null;

        public override Position3D Position
        {
            get
            {
                if (Parent != null)
                    return Parent.Position;
                else
                    return base.Position;
            }
        }

        public Point InContainerPosition = Point.Zero;

        public Item(Serial serial)
            : base(serial)
        {

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

        protected override EntityViews.AEntityView CreateView()
        {
            return new EntityViews.ItemView(this);
        }

		private int m_amount;
        public int Amount
        {
            get { return m_amount; }
            set { m_amount = value; }
        }

        public UltimaData.ItemData ItemData;

		private int m_ItemID = 0;
        public int ItemID
        {
            get { return m_ItemID; }
            set
            {
				m_ItemID = value;
                ItemData = UltimaXNA.UltimaData.TileData.ItemData[m_ItemID & 0x3FFF];
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

		public bool NoDraw
		{
			get { return m_ItemID <= 1; } // no draw
		}

		public bool IsCoin
        {
			get { return m_ItemID == 0xEEA || m_ItemID == 0xEED || m_ItemID == 0xEF0; }
		}

        public int ContainerSlotIndex = 0;

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
            if (Position.X == x && Position.Y == y)
				return true;
			else
				return false;
		}

        public virtual bool TryPickUp()
        {
            if (ItemData.Weight == 255)
                return false;
            else
                return true;
        }

        // ======================================================================
        // Last Parent routines 
        // ======================================================================

        private AEntity m_lastParent;
        public bool HasLastParent
        {
            get { return (m_lastParent != null); }
        }

        public void SaveLastParent()
        {
            m_lastParent = Parent;
        }

        public void RestoreLastParent()
        {
            if (m_lastParent != null)
            {
                ((Container)m_lastParent).AddItem(this);
            }
        }
    }
}
