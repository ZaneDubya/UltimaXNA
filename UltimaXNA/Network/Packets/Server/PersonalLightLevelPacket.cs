/***************************************************************************
 *   PersonalLightLevelPacket.cs
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
    public class PersonalLightLevelPacket : RecvPacket
    {
        readonly Serial _creatureSerial;
        readonly byte _lightLevel;

        public Serial CreatureSerial
        {
            get { return _creatureSerial; }
        }

        public byte LightLevel
        {
            get { return _lightLevel; }
        }

        public PersonalLightLevelPacket(PacketReader reader)
            : base(0x4E, "Personal Light Level")
        {
            this._creatureSerial = reader.ReadInt32();
            this._lightLevel = reader.ReadByte();
        }
    }
}
