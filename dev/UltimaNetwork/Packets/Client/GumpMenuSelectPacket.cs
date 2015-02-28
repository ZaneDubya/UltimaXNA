/***************************************************************************
 *   GumpMenuSelectPacket.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaNetwork.Packets.Client
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
