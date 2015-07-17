/***************************************************************************
 *   Characters.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Ultima.Network.Server;
#endregion

namespace UltimaXNA.Ultima.Login.Accounts
{
    public static class Characters
    {
        static CharacterListEntry[] m_characters;
        public static CharacterListEntry[] List { get { return m_characters; } }
        public static int Length { get { return m_characters.Length; } }

        static CharacterCityListPacket.StartingLocation[] m_locations;
        public static CharacterCityListPacket.StartingLocation[] StartingLocations { get { return m_locations; } }

        static int m_updateValue = 0;
        public static int UpdateValue { get { return m_updateValue; } }

        public static int FirstEmptySlot
        {
            get
            {
                for (int i = 0; i < m_characters.Length; i++)
                {
                    if (m_characters[i].Name == string.Empty)
                        return i;
                }
                return -1;
            }
        }

        public static void SetCharacterList(CharacterListEntry[] list)
        {
            m_characters = list;
            m_updateValue++;
        }

        public static void SetStartingLocations(CharacterCityListPacket.StartingLocation[] list)
        {
            m_locations = list;
            m_updateValue++;
        }
    }
}
