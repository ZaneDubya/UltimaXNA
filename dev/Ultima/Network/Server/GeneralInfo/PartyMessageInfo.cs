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
    /// <summary>
    /// Subcommand 0x06 / 0x03 and 0x06 / 0x04: Party message.
    /// </summary>
    public class PartyMessageInfo : IGeneralInfo {
        public readonly bool IsPrivate;
        public readonly Serial Source;
        public readonly string Message;

        public PartyMessageInfo(PacketReader reader, bool isPrivate) {
            IsPrivate = isPrivate;
            Source = (Serial)reader.ReadInt32();
            Message = reader.ReadUnicodeString();
        }
    }
}
