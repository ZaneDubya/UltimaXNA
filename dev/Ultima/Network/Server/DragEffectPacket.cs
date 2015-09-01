/***************************************************************************
 *   DragEffectPacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
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
    public class DragEffectPacket : RecvPacket
    {
        readonly int m_itemId;
        readonly int m_amount;
        readonly Serial m_sourceContainer;
        readonly int m_sourceX;
        readonly int m_sourceY;
        readonly int m_sourceZ;
        readonly Serial m_destContainer;
        readonly int m_destX;
        readonly int m_destY;
        readonly int m_destZ;

        public int ItemId 
        {
            get { return m_itemId; }
        }

        public int Amount 
        {
            get { return m_itemId; } 
        }

        public Serial SourceContainer 
        {
            get { return m_sourceContainer; }
        }

        public int SourceX 
        {
            get { return m_sourceX; } 
        }

        public int SourceY 
        {
            get { return m_sourceY; } 
        }

        public int SourceZ
        {
            get { return m_sourceZ; } 
        }

        public Serial DestContainer 
        {
            get { return m_destContainer; } 
        }

        public int DestX 
        {
            get { return m_destX; } 
        }

        public int DestY         
        {
            get { return m_destY; }
        }

        public int DestZ 
        {
            get { return m_destZ; }
        }

        public DragEffectPacket(PacketReader reader)
            : base(0x23, "Dragging Item")
        {
            m_itemId = reader.ReadUInt16();
            reader.ReadByte(); // 0x03 bytes unknown.
            reader.ReadByte(); //
            reader.ReadByte(); //
            m_amount = reader.ReadUInt16();
            m_sourceContainer = reader.ReadInt32(); // 0xFFFFFFFF for ground
            m_sourceX = reader.ReadUInt16();
            m_sourceY = reader.ReadUInt16();
            m_sourceZ = reader.ReadByte();
            m_destContainer = reader.ReadInt32(); // 0xFFFFFFFF for ground
            m_destX = reader.ReadUInt16();
            m_destY = reader.ReadUInt16();
            m_destZ = reader.ReadByte();
        }
    }
}
