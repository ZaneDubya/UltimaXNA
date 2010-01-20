/***************************************************************************
 *   ContainerItem.cs
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
    class ContainerItem : Item
    {
        // ContainerItems have inventory (chests, for example).
        // The Serial for the container for this inventory is the same as the
        // GameObject's Serial.
        GameObject_Container m_ContainerObject = null;
        public GameObject_Container Contents
        {
            get
            {
                return m_ContainerObject;
            }
        }

        public ContainerItem(Serial serial, IWorld world)
            : base(serial, world)
        {
            m_ContainerObject = new GameObject_Container(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            m_ContainerObject.Update(gameTime);
        }
    }
}
