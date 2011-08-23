/***************************************************************************
 *   HuedEffectPacket.cs
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
using UltimaXNA.Network;
#endregion

namespace UltimaXNA.Client.Packets.Server
{
    public class HuedEffectPacket : RecvPacket
    {
        public HuedEffectPacket(PacketReader reader)
            : base(0xC0, "Hued Effect")
        {
            EffectType type = (EffectType)reader.ReadByte();
            Serial source = reader.ReadInt32();
            Serial target = reader.ReadInt32();
            int itemID = reader.ReadUInt16();
            int xSource = reader.ReadUInt16();
            int ySource = reader.ReadUInt16();
            int zSource = reader.ReadByte();
            int xTarget = reader.ReadUInt16();
            int yTarget = reader.ReadUInt16();
            int zTarget = reader.ReadByte();
            int speed = reader.ReadByte();
            int duration = reader.ReadByte();
            int unknown0 = reader.ReadUInt16();
            int fixedDirection = reader.ReadByte();
            int explodes = reader.ReadByte();
            int hue = reader.ReadInt32();
            int renderMode = reader.ReadInt32();
        }
    }

    public enum EffectType
    {
        Moving = 0x00,
        Lightning = 0x01,
        FixedXYZ = 0x02,
        FixedFrom = 0x03
    }
}
