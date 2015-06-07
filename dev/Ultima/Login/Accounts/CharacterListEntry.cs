/***************************************************************************
 *   CharacterListEntry.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Network;

namespace UltimaXNA.Ultima.Login.Accounts
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
            name = reader.ReadString(30);
            password = reader.ReadString(30);
        }
    }
}
