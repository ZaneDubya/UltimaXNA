/***************************************************************************
 *   TargetCursorPacket.cs
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
    public class TargetCursorPacket : RecvPacket
    {
        readonly byte m_commandtype;
        readonly int m_cursorid;
        readonly byte m_cursortype;

        public byte CommandType 
        {
            get { return m_commandtype; }
        }

        public int CursorID 
        {
            get { return m_cursorid; }         
        }

        public byte CursorType 
        {
            get { return m_cursortype; } 
        }
        
        public TargetCursorPacket(PacketReader reader)
            : base(0x6C, "Target Cursor")
        {
            m_commandtype = reader.ReadByte(); // 0x00 = Select Object; 0x01 = Select X, Y, Z
            m_cursorid = reader.ReadInt32();
            m_cursortype = reader.ReadByte(); // 0 - 2 = unknown; 3 = Cancel current targetting RunUO seems to always send 0.
        }
    }
}
