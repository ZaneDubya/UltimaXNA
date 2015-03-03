/***************************************************************************
 *   Container.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld;
#endregion

namespace UltimaXNA.Entity
{
    public class Container : Item
    {
        public int UpdateTicker { get; internal set; }
        List<Item> m_contents;

        public List<Item> Contents
        {
            get
            {
                if (m_contents == null)
                    m_contents = new List<Item>();
                return m_contents;
            }
        }

        public Container(Serial serial)
            : base(serial)
        {
            UpdateTicker = 0;
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
        }

        public override void Dispose()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                Contents[i].Dispose();
            }
            base.Dispose();
        }

        public void AddItem(Item item)
        {
            if (!Contents.Contains(item))
            {
                Contents.Add(item);
                item.Parent = this;
            }
            UpdateTicker++;
        }

        public void RemoveItem(Serial serial)
        {
            foreach (Item item in Contents)
            {
                if (item.Serial == serial)
                {
                    item.SaveLastParent();
                    Contents.Remove(item);
                    break;
                }
            }
            UpdateTicker++;
        }
    }
}
