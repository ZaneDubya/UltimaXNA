/***************************************************************************
 *   DeathAnimationPacket.cs
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
    public class DeathAnimationPacket : RecvPacket
    {
        public readonly Serial PlayerSerial;
        public readonly Serial CorpseSerial;
        public DeathAnimationPacket(PacketReader reader)
            : base(0xAF, "Death Animation")
        {
            PlayerSerial = reader.ReadInt32();
            CorpseSerial = reader.ReadInt32();
            reader.ReadInt32(); // unknown - all zero's.
        }
    }
}
