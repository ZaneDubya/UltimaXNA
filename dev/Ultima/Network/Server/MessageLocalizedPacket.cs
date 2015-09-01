/***************************************************************************
 *   MessageLocalizedPacket.cs
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
    public class MessageLocalizedPacket : RecvPacket
    {
        public bool IsSystemMessage { get { return (Serial == 0xFFFF); } }
        public readonly Serial Serial;
        public readonly int Body;
        public readonly MessageTypes MessageType;
        public readonly int Hue;
        public readonly int Font;
        public readonly int CliLocNumber;
        public readonly string SpeakerName;
        public readonly string Arguements;

        public MessageLocalizedPacket(PacketReader reader)
            : base(0xC1, "Message Localized")
        {
            Serial = reader.ReadInt32(); // 0xffff for system message
            Body = reader.ReadInt16(); // (0xff for system message
            MessageType = (MessageTypes)reader.ReadByte(); // 6 - lower left, 7 on player
            Hue = reader.ReadUInt16();
            Font = reader.ReadInt16();
            CliLocNumber = reader.ReadInt32();
            SpeakerName = reader.ReadString(30);
            Arguements = reader.ReadUnicodeStringSafeReverse();
        }
    }
}
