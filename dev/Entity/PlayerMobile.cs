/***************************************************************************
 *   PlayerMobile.cs
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
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Model;
#endregion

namespace UltimaXNA.Entity
{
    public class PlayerMobile : Mobile
    {
        public PlayerMobile(Serial serial, Map map)
            : base(serial, map)
        {

        }

        public override string ToString()
        {
            return base.ToString() + " | " + Name;
        }

        public override void Update(double frameMS)
        {
            m_movement.PlayerMobile_CheckForMoveEvent();

            base.Update(frameMS);
        }
    }
}
