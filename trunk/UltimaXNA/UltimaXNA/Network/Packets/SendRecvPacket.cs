using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UltimaXNA.Network.Packets
{
    public abstract class SendRecvPacket: ISendPacket, IRecvPacket
    {
        readonly int id;
        readonly int length;
        readonly string name;

        public int Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public int Length
        {
            get { return length; }
        }

        private const int BufferSize = 4096;

        protected PacketWriter Stream;

        public SendRecvPacket(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.Stream = PacketWriter.CreateInstance(length);
            this.Stream.Write(id);
            this.Stream.Write((short)0);
        }

        public SendRecvPacket(int id, string name, int length)
        {
            this.id = id;
            this.name = name;
            this.length = length;

            this.Stream = PacketWriter.CreateInstance(length);
            this.Stream.Write((byte)id);
        }

        public void EnsureCapacity(int length)
        {
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)id);
            Stream.Write((short)length);
        }

        public byte[] Compile()
        {
            this.Stream.Flush();

            if (this.Length == 0)
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
            return string.Format("Id: {0:X2} Name: {1} Length: {2}", id, name, length);
        }
    }
}
