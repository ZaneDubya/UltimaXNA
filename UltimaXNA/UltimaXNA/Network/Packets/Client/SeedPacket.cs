using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UltimaXNA.Network.Packets.Client
{
    public class SeedPacket : SendPacket
    {
        public SeedPacket(int seed, int major, int minor, int revision, int prototype)
            : base(0xEF, "Seed", 21)
        {
            Stream.Write(seed);
            Stream.Write(major);
            Stream.Write(minor);
            Stream.Write(revision);
            Stream.Write(prototype);
        }
    }
}