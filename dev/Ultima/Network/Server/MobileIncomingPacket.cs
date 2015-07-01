/***************************************************************************
 *   MobileIncomingPacket.cs
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
using System.Collections.Generic;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class MobileIncomingPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_body;        
        readonly short m_x;
        readonly short m_y;
        readonly short m_z;
        readonly byte m_direction;
        readonly ushort m_hue;
        public readonly MobileFlags Flags;
        readonly byte m_notoriety;
        readonly EquipmentEntry[] m_equipment;

        public Serial Serial
        {
            get { return m_serial; } 
        }

        public short BodyID 
        {
            get { return m_body; }
        }

        public short X
        {
            get { return m_x; } 
        }

        public short Y 
        {
            get { return m_y; } 
        }

        public short Z 
        {
            get { return m_z; } 
        }

        public byte Direction
        {
            get { return m_direction; } 
        }

        public ushort Hue 
        {
            get { return m_hue; } 
        }

        public EquipmentEntry[] Equipment
        {
            get { return m_equipment; }
        }

        /// <summary>
        /// 0x1: Innocent (Blue)
        /// 0x2: Friend (Green)
        /// 0x3: Grey (Grey - Non Criminal)
        /// 0x4: Criminal (Grey)
        /// 0x5: Enemy (Orange)
        /// 0x6: Murderer (Red)
        /// 0x7: Invulnerable (Yellow)
        /// </summary>
        public byte Notoriety
        {
            get { return m_notoriety; }
        }  

        public MobileIncomingPacket(PacketReader reader)
            : base(0x78, "Mobile Incoming")
        {
            // Mobile
            m_serial = reader.ReadInt32();
            m_body = reader.ReadInt16();
            m_x = reader.ReadInt16();
            m_y = reader.ReadInt16();
            m_z = reader.ReadSByte();
            m_direction = reader.ReadByte();
            m_hue = reader.ReadUInt16();
            Flags = new MobileFlags((MobileFlag)reader.ReadByte());
            m_notoriety = reader.ReadByte();

            // Read equipment - nine bytes ea.
            List<EquipmentEntry> items = new List<EquipmentEntry>();

            Serial serial = reader.ReadInt32();
            if (!serial.IsValid)
            {
                reader.ReadByte(); //zero terminated
                m_equipment = new EquipmentEntry[0];
            }
            else
            {
                while (serial.IsValid)
                {
                    ushort gumpId = reader.ReadUInt16();
                    byte layer = reader.ReadByte();
                    ushort hue = 0;

                    if ((gumpId & 0x8000) == 0x8000)
                    {
                        gumpId = (ushort)((int)gumpId - 0x8000);
                        hue = reader.ReadUInt16();
                    }

                    items.Add(new EquipmentEntry(serial, gumpId, layer, hue));
                    // read the next serial and begin the loop again. break at 0x00000000
                    serial = reader.ReadInt32();
                }
                m_equipment = items.ToArray();
            }
        }
    }
}
