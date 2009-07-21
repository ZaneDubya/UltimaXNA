/***************************************************************************
 *   CharacterListEntry.cs
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
#endregion

namespace UltimaXNA.Network
{
    public class CharacterListEntry
    {
        string name;
        string password;

        public string Name
        {
            get { return name; }
        }

        public string Password
        {
            get { return password; }
        }

        public CharacterListEntry(PacketReader reader)
        {
            this.name = reader.ReadString(30);
            this.password = reader.ReadString(30);
        }
    }
}
