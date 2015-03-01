/***************************************************************************
 *   MobileAttributesPacket.cs
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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Server
{
    public class MobileAttributesPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _maxHits;
        readonly short _currentHits;
        readonly short _maxMana;
        readonly short _currentMana;
        readonly short _maxStamina;
        readonly short _currentStamina;

        public Serial Serial
        {
            get { return _serial; }
        }

        public short MaxHits
        {
            get { return _maxHits; }
        }

        public short CurrentHits
        {
            get { return _currentHits; }
        }

        public short MaxMana
        {
            get { return _maxMana; }
        }

        public short CurrentMana
        {
            get { return _currentMana; }
        }

        public short MaxStamina
        {
            get { return _maxStamina; }
        }

        public short CurrentStamina
        {
            get { return _currentStamina; }
        }


        public MobileAttributesPacket(PacketReader reader)
            : base(0x2D, "Mobile Attributes")
        {
            _serial = reader.ReadInt32();
            _maxHits = reader.ReadInt16();
            _currentHits = reader.ReadInt16();
            _maxMana = reader.ReadInt16();
            _currentMana = reader.ReadInt16();
            _maxStamina = reader.ReadInt16();
            _currentStamina = reader.ReadInt16();
        }
    }
}
