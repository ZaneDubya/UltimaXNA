using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class SeasonChangePacket : RecvPacket
    {
        readonly byte _season;
        readonly byte _playSound;

        public byte Season
        {
            get { return _season; }
        }
        public byte PlaySound
        {
            get { return _playSound; }
        }

        public SeasonChangePacket(PacketReader reader)
            : base(0xBC, "Seasonal Information")
        {
            this._season = reader.ReadByte();
            this._playSound = reader.ReadByte();
        }
    }
}
