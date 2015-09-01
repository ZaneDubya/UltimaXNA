/***************************************************************************
 *   GumpMenuSelectPacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    public class GumpMenuSelectPacket : SendPacket
    {
        public GumpMenuSelectPacket(int id, int gumpId, int buttonId, int[] switchIds, Tuple<short, string>[] textEntries)
            : base(0xB1, "Gump Menu Select")
        {
            Stream.Write((uint)id);
            Stream.Write((uint)gumpId);
            Stream.Write((uint)buttonId);

            if (switchIds == null)
            {
                Stream.Write((uint)0);
            }
            else
            {
                Stream.Write((uint)switchIds.Length);
                for (int i = 0; i < switchIds.Length; i++)
                    Stream.Write((uint)switchIds[i]);
            }

            if (textEntries == null)
            {
                Stream.Write((uint)0);
            }
            else
            {
                Stream.Write((uint)textEntries.Length);
                for (int i = 0; i < textEntries.Length; i++)
                {
                    int length = textEntries[i].Item2.Length * 2;

                    Stream.Write((ushort)textEntries[i].Item1);
                    Stream.Write((ushort)length);
                    Stream.WriteBigUniFixed(textEntries[i].Item2, textEntries[i].Item2.Length);
                }
            }
        }
    }
}
