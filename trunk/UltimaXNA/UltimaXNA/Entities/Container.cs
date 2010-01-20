/***************************************************************************
 *   Container.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    class Container : Item
    {
        static List<Item> NullItems = new List<Item>();

        public int UpdateTicker { get; internal set; }

        List<Item> _contents;

        public List<Item> Contents
        {
            get
            {
                if (_contents == null)
                    _contents = NullItems;
                return _contents;
            }
        }

        public Container(Serial serial, IWorld world)
            : base(serial, world)
        {
            UpdateTicker = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void AddItem(Item item)
        {
            Contents.Add(item);
            UpdateTicker++;
        }

        public void RemoveItem(Item item)
        {
            Contents.Remove(item);
            UpdateTicker++;
        }
    }
}
