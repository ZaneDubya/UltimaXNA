/***************************************************************************
 *   ChatPacket.cs
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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Server
{
    public class ChatPacket : RecvPacket
    {
        readonly string _language;
        readonly byte _commandtype;

        public string Language
        {
            get { return _language; }
        }

        public byte CommandType
        {
            get { return _commandtype; }
        } 

        public ChatPacket(PacketReader reader)
            : base(0xB3, "Chat Packet")
        {

            _language = reader.ReadString(3);
            reader.ReadInt16(); // unknown.
            _commandtype = reader.ReadByte();
        }
    }
}
