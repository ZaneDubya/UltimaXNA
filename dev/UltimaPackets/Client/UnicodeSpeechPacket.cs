/***************************************************************************
 *   UnicodeSpeechPacket.cs
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
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Client
{
    public class UnicodeSpeechPacket : SendPacket
    {
        public UnicodeSpeechPacket(short color, short font, string lang, string text)
            : base(0xAD, "Unicode Speech")
        {
            Stream.Write((byte)0);
            Stream.Write((short)color);
            Stream.Write((short)font);
            Stream.WriteAsciiNull(lang);
            // Stream.Write((byte)0);
            // Stream.Write((byte)0x10);
            // Stream.Write((byte)0x02);
            Stream.WriteBigUniNull(text);
        }
    }
}
