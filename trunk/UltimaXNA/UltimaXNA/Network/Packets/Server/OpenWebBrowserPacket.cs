﻿/***************************************************************************
 *   OpenWebBrowserPacket.cs
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
#endregion

namespace UltimaXNA.Network.Packets.Server
{
    public class OpenWebBrowserPacket : RecvPacket
    {
        readonly string _url;

        public string WebsiteUrl
        {
            get { return _url; }
        }

        public OpenWebBrowserPacket(PacketReader reader)
            : base(0xA5, "Open Web Browser")
        {
            _url = reader.ReadString(reader.Size);
        }
    }
}