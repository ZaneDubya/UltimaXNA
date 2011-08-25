/***************************************************************************
 *   GraphicEffectPacket.cs
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
    public class GraphicEffectHuedPacket : GraphicEffectPacket
    {
        public readonly int Hue;
        public readonly GraphicEffectBlendMode BlendMode;

        public GraphicEffectHuedPacket(PacketReader reader)
            : base(0xC0, reader)
        {
            Hue = reader.ReadInt32();
            BlendMode = (GraphicEffectBlendMode)reader.ReadInt32();
        }
    }

    public class GraphicEffectPacket : RecvPacket
    {
        public readonly GraphicEffectType EffectType;
        public readonly Serial SourceSerial;
        public readonly Serial TargetSerial;
        public readonly int BaseItemID;
        public readonly int SourceX;
        public readonly int SourceY;
        public readonly int SourceZ;
        public readonly int TargetX;
        public readonly int TargetY;
        public readonly int TargetZ;
        public readonly int Speed;
        public readonly int Duration;
        public readonly bool FixedDirection;
        public readonly bool DoesExplode;

        public GraphicEffectPacket(int id, PacketReader reader)
            : base(id, "Show Graphic Effect")
        {
            EffectType = (GraphicEffectType)reader.ReadByte();
            SourceSerial = reader.ReadInt32();
            TargetSerial = reader.ReadInt32();
            BaseItemID = reader.ReadUInt16();
            SourceX = reader.ReadUInt16();
            SourceY = reader.ReadUInt16();
            SourceZ = reader.ReadByte();
            TargetX = reader.ReadUInt16();
            TargetY = reader.ReadUInt16();
            TargetZ = reader.ReadByte();
            Speed = reader.ReadByte();
            Duration = reader.ReadByte();
            int unknown0 = reader.ReadUInt16();
            FixedDirection = (reader.ReadByte() != 0);
            DoesExplode = (reader.ReadByte() != 0);
        }

        public GraphicEffectPacket(PacketReader reader)
            : this(0x70, reader)
        {

        }
    }

    public enum GraphicEffectType
    {
        Moving = 0x00,
        Lightning = 0x01,
        FixedXYZ = 0x02,
        FixedFrom = 0x03,
        ScreenFade = 0x04,
        Nothing = 0xFF,
    }

    public enum GraphicEffectBlendMode
    {
        Normal = 0x00,                  // normal, black is transparent
        Multiply = 0x01,                // darken
        Screen = 0x02,                  // lighten
        ScreenMore = 0x03,              // lighten more (slightly)
        ScreenLess = 0x04,              // lighten less
        NormalHalfTransparent = 0x05,   // transparent but with black edges - 50% transparency?
        ShadowBlue = 0x06,              // complete shadow with blue edges
        ScreenRed = 0x07                // transparent more red
    }
}
