/***************************************************************************
 *   MobileAnimationPacketPacket.cs
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

namespace UltimaXNA.Client.Packets.Server
{
    public class MobileAnimationPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _action;
        readonly short _framecount;
        readonly short _repeatcount;
        readonly byte _reverse;
        readonly byte _repeat;
        readonly byte _delay;

        public Serial Serial
        {
            get { return _serial; } 
        }

        public short Action
        {
            get { return _action; }
        }

        public short FrameCount
        {
            get { return _framecount; }
        }

        public short RepeatCount
        {
            get { return _repeatcount; }
        }

        public bool Reverse 
        {
            get { return (_reverse == 1); }
        }

        public bool Repeat
        {
            get { return (_repeat == 1); } 
        }

        public byte Delay
        {
            get { return _delay; }
        }

        public MobileAnimationPacket(PacketReader reader)
            : base(0x6E, "Mobile Animation")
        {
            _serial = reader.ReadInt32();
            _action = reader.ReadInt16();
            _framecount = reader.ReadInt16();
            _repeatcount = reader.ReadInt16();
            _reverse = reader.ReadByte(); // 0x00=forward, 0x01=backwards
            _repeat = reader.ReadByte(); // 0 - Don't repeat / 1 repeat
            _delay = reader.ReadByte();
        }
    }
}
