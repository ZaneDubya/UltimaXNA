/***************************************************************************
 *   DynamicObject.cs
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
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    class DynamicObject : Item
    {
        public int CasterSerial = 0;
        public int Bytes0 = 0;
        public int SpellID = 0;

        public DynamicObject(Serial serial, World world)
            : base(serial, world)
        {

        }
    }
}
