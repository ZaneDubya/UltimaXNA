/***************************************************************************
 *   PartyMessageInfo.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Core.Network;

namespace UltimaXNA.Ultima.Network.Server.GeneralInfo {
    class PartyMessageInfo : IGeneralInfo {
        public readonly bool IsPrivate;
        public readonly int Count;
        public readonly int[] Serials;

        public PartyMessageInfo(PacketReader reader, bool isPrivate) {
            IsPrivate = isPrivate;
            Count = reader.ReadByte();
            Serials = new int[Count];
            for (int i = 0; i < Count; i++) {
                Serials[i] = reader.ReadInt32();
            }
        }
    }
}
