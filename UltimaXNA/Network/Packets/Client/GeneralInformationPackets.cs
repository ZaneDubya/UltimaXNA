/***************************************************************************
 *   GeneralInformationPackets.cs
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
#endregion

namespace UltimaXNA.Network.Packets.Client
{
    public class RequestCustomHousePacket : SendPacket
    {
        public RequestCustomHousePacket(Serial serial)
            : base(0xBF, "Request Custom House Packet")
        {
            Stream.Write((short)0x1E); // subcommand 0x1E, request custom house
            Stream.Write((int)serial);
        }
    }

    public class RequestContextMenuPacket : SendPacket
    {
        public RequestContextMenuPacket(Serial serial)
            : base(0xBF, "Context Menu Request")
        {
            Stream.Write((short)0x13); // subcommand 0x13, request context menu
            Stream.Write((int)serial);
        }
    }

    public class ContextMenuResponsePacket : SendPacket
    {
        public ContextMenuResponsePacket(Serial serial, short responseIndex)
            : base(0xBF, "Context Menu Response")
        {
            Stream.Write((short)0x15); // subcommand 0x15,  response to context menu
            Stream.Write((int)serial);
            Stream.Write((short)responseIndex);
        }
    }

    public class ReportClientScreenSizePacket : SendPacket
    {
        public ReportClientScreenSizePacket(int width, int height)
            : base(0xBF, "Report Screen Size Packet")
        {
            Stream.Write((short)0x05); // subcommand 0x05
            Stream.Write((ushort)width);
            Stream.Write((ushort)height);
            Stream.Write((ushort)0xFFA7); // unknown value. Might not always be this value.
        }
    }

    public class ReportClientLocalizationPacket : SendPacket
    {
        public ReportClientLocalizationPacket(string locale)
            : base(0xBF, "Report Client Localization Packet")
        {
            Stream.Write((short)0x0B); // subcommand 0x0B
            Stream.WriteAsciiNull(locale);
        }
    }
}
