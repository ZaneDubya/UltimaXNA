/***************************************************************************
 *   PartyInvitationInfo.cs
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
    /// Subcommand 0x06 / 0x07: Invitation to joint a party.
    /// </summary>
    public class PartyInvitationInfo : IGeneralInfo {
        public readonly int PartyLeaderSerial;

        public PartyInvitationInfo(PacketReader reader) {
            PartyLeaderSerial = reader.ReadInt32();
        }
    }
}
