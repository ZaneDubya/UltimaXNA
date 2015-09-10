/***************************************************************************
 *   AsciiMessagePacket.cs
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
    public class AsciiMessagePacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly short Model;
        public readonly MessageTypes MsgType;
        public readonly ushort Hue;
        public readonly short Font;
        public readonly string SpeakerName;
        public readonly string Text;

        public AsciiMessagePacket(PacketReader reader)
            : base(0x1C, "Ascii Message")
        {
            Serial = reader.ReadInt32();
            Model = reader.ReadInt16();
            MsgType = (MessageTypes)reader.ReadByte();
            Hue = reader.ReadUInt16();
            Font = reader.ReadInt16();
            SpeakerName = reader.ReadString(30).Trim();
            Text = reader.ReadString();
        }
    }
}
