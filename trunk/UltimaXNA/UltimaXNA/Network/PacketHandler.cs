using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network
{
    public delegate void PacketReceiveHandler(PacketReader reader);
    public delegate void TypedPacketReceiveHandler(IRecvPacket packet);

    public class PacketHandler
    {
        int id;
        int length;
        string name;
        PacketReceiveHandler handler;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public PacketReceiveHandler Handler
        {
            get { return handler; }
            set { handler = value; }
        }

        public PacketHandler(int id, string name, int length, PacketReceiveHandler handler)
        {
            this.id = id;
            this.name = name;
            this.length = length;
            this.handler = handler;
        }
    }

    public class TypedPacketHandler : PacketHandler
    {
        Type type;
        TypedPacketReceiveHandler handler;

        public Type Type
        {
            get { return type; }
        }

        public TypedPacketReceiveHandler TypeHandler
        {
            get { return handler; }
            set { handler = value; }
        }

        private new PacketReceiveHandler Handler
        {
            get { return null; }
            set { base.Handler = value; }
        }

        public TypedPacketHandler(int id, string name, Type type, int length, TypedPacketReceiveHandler handler)
            : base(id, name, length, null)
        {
            this.type = type;
            this.handler = handler;
        }

        public IRecvPacket CreatePacket(PacketReader reader)
        {
            return (IRecvPacket)Activator.CreateInstance(type, new object[] { reader });
        }
    }
}
