/***************************************************************************
 *   UnicodeMessagePacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
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
using UltimaXNA.Ultima.Data;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class UnicodeMessagePacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly short Model;
        public readonly MessageTypes MsgType;
        public readonly short Hue;
        public readonly short Font;
        public readonly string Language;
        public readonly string SpeakerName;
        public readonly string Text;
        
        public UnicodeMessagePacket(PacketReader reader)
            : base(0xAE, "Unicode Message")
        {
            Serial = reader.ReadInt32();
            Model = reader.ReadInt16();
            MsgType = (MessageTypes)reader.ReadByte();
            Hue = reader.ReadInt16();
            Font = reader.ReadInt16();
            Language = reader.ReadString(4).Trim();
            SpeakerName = reader.ReadString(30).Trim();
            Text = reader.ReadUnicodeString((reader.Buffer.Length - 48) / 2);
        }
    }
}
