/***************************************************************************
 *   MobileAnimationPacketPacket.cs
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

namespace UltimaXNA.Ultima.Network.Server
{
    public class MobileAnimationPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_action;
        readonly short m_framecount;
        readonly short m_repeatcount;
        readonly byte m_reverse;
        readonly byte m_repeat;
        readonly byte m_delay;

        public Serial Serial
        {
            get { return m_serial; } 
        }

        public short Action
        {
            get { return m_action; }
        }

        public short FrameCount
        {
            get { return m_framecount; }
        }

        public short RepeatCount
        {
            get { return m_repeatcount; }
        }

        public bool Reverse 
        {
            get { return (m_reverse == 1); }
        }

        public bool Repeat
        {
            get { return (m_repeat == 1); } 
        }

        public byte Delay
        {
            get { return m_delay; }
        }

        public MobileAnimationPacket(PacketReader reader)
            : base(0x6E, "Mobile Animation")
        {
            m_serial = reader.ReadInt32();
            m_action = reader.ReadInt16();
            m_framecount = reader.ReadInt16();
            m_repeatcount = reader.ReadInt16();
            m_reverse = reader.ReadByte(); // 0x00=forward, 0x01=backwards
            m_repeat = reader.ReadByte(); // 0 - Don't repeat / 1 repeat
            m_delay = reader.ReadByte();
        }
    }
}
