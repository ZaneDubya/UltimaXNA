/***************************************************************************
 *   PartyMember.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Player.Partying
{
    public class PartyMember
    {
        public readonly Serial Serial;
        string m_CachedName;
        public Mobile Mobile => WorldModel.Entities.GetObject<Mobile>(Serial, false);

        public string Name
        {
            get
            {
                Mobile mobile = Mobile;
                if (Mobile != null)
                {
                    m_CachedName = Mobile.Name;
                }
                return m_CachedName;
            }
        }

        public PartyMember(Serial serial)
        {
            Serial = serial;
            m_CachedName = Name;
        }
    }
}
