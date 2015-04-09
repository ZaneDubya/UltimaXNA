/***************************************************************************
 *   SendRecvPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.IO;
using UltimaXNA.Core.Network;
#endregion

namespace UltimaXNA.Core.Network.Packets
{
    public abstract class SendRecvPacket: ISendPacket, IRecvPacket
    {
        readonly int m_Id;
        readonly int m_Length;
        readonly string m_Name;

        public int Id
        {
            get { return m_Id; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public int Length
        {
            get { return m_Length; }
        }

        private const int BufferSize = 4096;

        protected PacketWriter Stream;

        public SendRecvPacket(int id, string name)
        {
            m_Id = id;
            m_Name = name;
            Stream = PacketWriter.CreateInstance(m_Length);
            Stream.Write(id);
            Stream.Write((short)0);
        }

        public SendRecvPacket(int id, string name, int length)
        {
            m_Id = id;
            m_Name = name;
            m_Length = length;

            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)id);
        }

        public void EnsureCapacity(int length)
        {
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)m_Id);
            Stream.Write((short)length);
        }

        public byte[] Compile()
        {
            Stream.Flush();

            if (Length == 0)
            {
                long length = Stream.Length;
                Stream.Seek((long)1, SeekOrigin.Begin);
                Stream.Write((ushort)length);
                Stream.Flush();
            }

            return Stream.Compile();
        }

        public override string ToString()
        {
            return string.Format("Id: {0:X2} Name: {1} Length: {2}", m_Id, m_Name, m_Length);
        }
    }
}
