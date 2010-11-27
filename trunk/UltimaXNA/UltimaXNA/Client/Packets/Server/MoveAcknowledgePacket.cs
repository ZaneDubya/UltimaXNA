/***************************************************************************
 *   MoveAcknowledgePacket.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace UltimaXNA.Client.Packets.Server
{
    public class MoveAcknowledgePacket : RecvPacket
    {
        readonly byte _sequence;
        readonly byte _notoriety;

        public byte Sequence 
        {
            get { return _sequence; } 
        }

        public byte Notoriety
        {
            get { return _notoriety; }
        }

        public MoveAcknowledgePacket(PacketReader reader)
            : base(0x22, "Move Request Acknowledged")
        {
            _sequence = reader.ReadByte(); // (matches sent sequence)
            _notoriety = reader.ReadByte(); // Not sure why it sends this.
        }
    }
}
