/***************************************************************************
 *   OpenWebBrowserPacket.cs
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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class OpenWebBrowserPacket : RecvPacket
    {
        readonly string m_url;

        public string WebsiteUrl
        {
            get { return m_url; }
        }

        public OpenWebBrowserPacket(PacketReader reader)
            : base(0xA5, "Open Web Browser")
        {
            m_url = reader.ReadString(reader.Size);
        }
    }
}