/***************************************************************************
 *   ContainerItem.cs
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

        public ContainerItem(Serial serial)
            : base(serial)
        {
            m_ContainerObject = new GameObject_Container(this);
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            m_ContainerObject.Update(frameMS);
        }
    }
}
