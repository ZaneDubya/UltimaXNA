using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets
{
    public abstract class RecvPacket : IRecvPacket
    {
        readonly int id;
        readonly string name;

        public int Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public RecvPacket(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
