using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class ServerRelayPacket : RecvPacket
    {
        readonly int _ipAddress;
        readonly int _port;
        readonly int _accountId;

        public int IpAddress
        {
            get { return _ipAddress; }
        }

        public int Port
        {
            get { return _port; }
        }

        public int AccountId
        {
            get { return _accountId; }
        }

        public ServerRelayPacket(PacketReader reader)
            : base(0x8C, "Server Relay")
        {
            _ipAddress = reader.ReadInt32();
            _port = reader.ReadInt16();
            _accountId = reader.ReadInt32();
        }
    }
}
