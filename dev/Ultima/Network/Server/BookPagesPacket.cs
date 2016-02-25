/***************************************************************************
 *   BookPagesPacket.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
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
using UltimaXNA.Ultima.World.Entities.Items;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class BookPagesPacket : RecvPacket
    {
        private Serial m_Serial;
        private int m_PageCount;
        private BaseBook.BookPageInfo[] m_Pages;

        public Serial Serial
        {
            get { return m_Serial; }
            set { m_Serial = value; }
        }

        public int PageCount
        {
            get
            {
                return m_PageCount;
            }

            set
            {
                m_PageCount = value;
            }
        }

        public BaseBook.BookPageInfo[] Pages
        {
            get
            {
                return m_Pages;
            }

            set
            {
                m_Pages = value;
            }
        }

        public BookPagesPacket(PacketReader reader)
            : base(0x66, "Book Pages")
        {
            m_Serial = reader.ReadInt32();
            m_PageCount = reader.ReadInt16();
            m_Pages = new BaseBook.BookPageInfo[m_PageCount];

            for (int i = 0; i < m_PageCount; ++i)
            {
                int page = reader.ReadInt16();
                int length = reader.ReadInt16();
                string[] lines = new string[length];

                for (int j = 0; j < length; j++)
                {
                    lines[j] = reader.ReadString();
                }

                m_Pages[i] = new BaseBook.BookPageInfo(lines);
            }
        }
    }
}
