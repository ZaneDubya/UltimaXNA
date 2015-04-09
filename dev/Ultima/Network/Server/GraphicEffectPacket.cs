/***************************************************************************
 *   GraphicEffectPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Core.Diagnostics.Tracing;

#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class GraphicEffectExtendedPacket : GraphicEffectHuedPacket
    {
        public GraphicEffectExtendedPacket(int id, PacketReader reader)
            : base(id, reader)
        {
            // BYTE[2] effect # (tile ID)
            // BYTE[2] explode effect # (0 if no explosion)
            // BYTE[2] additional effect # that's only used for moving effects, 0 otherwise
            // BYTE[4] if target is item (type 2) that's itemId, 0 otherwise
            // BYTE[1] layer (of the character, e.g left hand, right hand, ... 0-4, 0xff: moving effect or target is no char)
            // BYTE[2] yet another (unknown) additional effect that's only set for moving effect, 0 otherwise
        	Tracer.Warn("Packet 0xC7 received; support for this packet is not yet implemented.");
		}

        public GraphicEffectExtendedPacket(PacketReader reader)
            : this(0xC7, reader)
        {

        }
    }

    public class GraphicEffectHuedPacket : GraphicEffectPacket
    {
        public readonly int Hue;
        public readonly GraphicEffectBlendMode BlendMode;

        public GraphicEffectHuedPacket(int id, PacketReader reader)
            : base(id, reader)
        {
            Hue = reader.ReadInt32();
            BlendMode = (GraphicEffectBlendMode)reader.ReadInt32();
        }

        public GraphicEffectHuedPacket(PacketReader reader)
            : this(0xC0, reader)
        {

        }
    }

    public class GraphicEffectPacket : RecvPacket
    {
        public readonly GraphicEffectType EffectType;
        public readonly Serial SourceSerial;
        public readonly Serial TargetSerial;
        public readonly int ItemID;
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
            ItemID = reader.ReadUInt16();
            SourceX = reader.ReadUInt16();
            SourceY = reader.ReadUInt16();
            SourceZ = reader.ReadByte();
            TargetX = reader.ReadUInt16();
            TargetY = reader.ReadUInt16();
            TargetZ = reader.ReadByte();
            Speed = reader.ReadByte();
            Duration = reader.ReadByte();
            int unknown0 = reader.ReadUInt16(); // on OSI, flamestrike is 0x0100.
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
