/***************************************************************************
 *   Servers.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.UltimaPackets;

namespace UltimaXNA.UltimaVars
{
    class Servers
    {
        static ServerListEntry[] m_serverListPacket;
        public static ServerListEntry[] List { get { return m_serverListPacket; } set { m_serverListPacket = value; } }
    }
}
