/***************************************************************************
 *   MessageLocalizedPacket.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace UltimaXNA.Client.Packets.Server
{
    public class MessageLocalizedPacket : RecvPacket
    {
        public bool IsSystemMessage { get { return (Serial == 0xFFFF); } }
        public readonly Serial Serial;
        public readonly int Body;
        public readonly MessageType MessageType;
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
            MessageType = (MessageType)reader.ReadByte(); // 6 - lower left, 7 on player
            Hue = reader.ReadUInt16();
            Font = reader.ReadInt16();
            CliLocNumber = reader.ReadInt32();
            SpeakerName = reader.ReadString(30);
            Arguements = reader.ReadUnicodeStringSafeReverse();
        }
    }
}
