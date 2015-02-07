/***************************************************************************
 *   DamagePacket.cs
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
    public class DamagePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _damage;

        public Serial Serial
        {
            get { return _serial; }
        } 

        public short Damage
        {
            get { return _damage; }
        } 
        
        public DamagePacket(PacketReader reader)
            : base(0x0B, "Damage")
        {
            _serial = reader.ReadInt32();
            _damage = reader.ReadInt16();
        }
    }
}
