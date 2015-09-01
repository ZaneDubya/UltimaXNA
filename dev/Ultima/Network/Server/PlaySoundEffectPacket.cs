/***************************************************************************
 *   PlaySoundEffectPacket.cs
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
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class PlaySoundEffectPacket : RecvPacket
    {
        public readonly int Mode;
        public readonly int SoundModel;
        public readonly int Unknown;
        public readonly int X, Y, Z;
        public PlaySoundEffectPacket(PacketReader reader)
            : base(0x54, "Play Sound Effect")
        {
            Mode = reader.ReadByte();
            SoundModel = reader.ReadInt16();
            Unknown = reader.ReadInt16();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            Z = reader.ReadInt16();
        }
    }
}
