/***************************************************************************
 *   PopupMessagePacket.cs
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
    public class PopupMessagePacket : RecvPacket
    {
        public static string[] Messages = new string[] {
                "Incorrect password", 
                "This character does not exist any more!",
		        "This character already exists.",
                "Could not attach to game server.",
                "Could not attach to game server.",
		        "A character is already logged in.",
		        "Synchronization Error.",
		        "You have been idle for to long.",
                "Could not attach to game server.",
                "Character transfer in progress."
            };

        readonly byte m_id;

        public string Message
        {
            get
            {
                return Messages[m_id];
            }
        }

        public PopupMessagePacket(PacketReader reader)
            : base(0x53, "Popup Message")
        {
            m_id = reader.ReadByte();
        }
    }
}
