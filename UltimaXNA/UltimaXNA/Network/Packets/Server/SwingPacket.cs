using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
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
