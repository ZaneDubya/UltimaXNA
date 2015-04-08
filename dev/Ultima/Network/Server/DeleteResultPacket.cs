/***************************************************************************
 *   CharacterListUpdatePacket.cs
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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Server
{
    public class DeleteCharacterResponsePacket : RecvPacket
    {
        public string Result
        {
            get
            {
                switch ((DeleteResultType)m_result)
                {
                    case DeleteResultType.PasswordInvalid:
                        return UltimaData.StringData.Entry(3000018); // 3000018: That character password is invalid.
                    case DeleteResultType.CharNotExist:
                        return UltimaData.StringData.Entry(3000019); // 3000019: That character does not exist.
                    case DeleteResultType.CharBeingPlayed:
                        return UltimaData.StringData.Entry(3000020); // 3000020: That character is being played right now.
                    case DeleteResultType.CharTooYoung:
                        return UltimaData.StringData.Entry(3000021); // 3000021: That character is not old enough to delete. The character must be 7 days old before it can be deleted.
                    case DeleteResultType.CharQueued:
                        return UltimaData.StringData.Entry(3000022); // 3000022: That character is currently queued for backup and cannot be deleted.
                    case DeleteResultType.BadRequest:
                        return UltimaData.StringData.Entry(3000023); // 3000023: Couldn't carry out your request.
                    default:
                        return "Could not delete character.";
                }
            }
        }
        // enum from RunUO. Other values may be possible
        enum DeleteResultType
        {
            PasswordInvalid, // never sent by RunUO
            CharNotExist,
            CharBeingPlayed,
            CharTooYoung,
            CharQueued, // never sent by RunUO
            BadRequest
        }

        byte m_result;

        public DeleteCharacterResponsePacket(PacketReader reader)
            : base(0x85, "Character Delete Result")
        {
            m_result = reader.ReadByte();
        }
    }
}
