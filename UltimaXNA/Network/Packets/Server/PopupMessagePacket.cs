/***************************************************************************
 *   PopupMessagePacket.cs
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

namespace UltimaXNA.Network.Packets.Server
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

        readonly byte _id;

        public string Message
        {
            get
            {
                return Messages[_id];
            }
        }

        public PopupMessagePacket(PacketReader reader)
            : base(0x53, "Popup Message")
        {
            _id = reader.ReadByte();
        }
    }
}
