using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class GumpMenuSelectPacket : SendPacket
    {
        public GumpMenuSelectPacket(int id, int gumpId, int buttonId, int[] switchIds, Pair<short, string>[] textEntries)
            : base(0xB1, "Gump Menu Select")
        {
            Stream.Write(id);
            Stream.Write(gumpId);
            Stream.Write(buttonId);
            Stream.Write(switchIds.Length);

            for (int i = 0; i < switchIds.Length; i++)
                Stream.Write(switchIds[i]);

            Stream.Write(textEntries.Length);

            for (int i = 0; i < textEntries.Length; i++)
            {
                int length = textEntries[i].ItemB.Length * 2;

                Stream.Write(textEntries[i].ItemA);
                Stream.Write(length);
                Stream.WriteBigUniFixed(textEntries[i].ItemB, length);
            }
        }
    }
}
