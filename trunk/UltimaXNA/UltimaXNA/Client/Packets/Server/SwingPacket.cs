/***************************************************************************
 *   SwingPacket.cs
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
    public class SwingPacket : RecvPacket
    {
        readonly Serial _attacker;
        readonly Serial _defender;
        readonly byte _flag;

        public Serial Attacker
        {
            get { return _attacker; }
        }

        public Serial Defender
        {
            get { return _defender; }
        }

        public byte Flag
        {
            get { return _flag; }
        }

        public SwingPacket(PacketReader reader)
            : base(0x2F, "Swing")
        {
            _flag = reader.ReadByte();
            _attacker = reader.ReadInt32();
            _defender = reader.ReadInt32();
        }
    }
}
