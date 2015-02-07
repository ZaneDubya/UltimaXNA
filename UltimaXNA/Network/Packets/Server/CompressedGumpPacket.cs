/***************************************************************************
 *   CompressedGumpPacket.cs
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
using System;
using System.Collections.Generic;
using System.Text;
using UltimaXNA.Network;
#endregion

namespace UltimaXNA.Network.Packets.Server
{
    public class CompressedGumpPacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly Serial GumpID;
        public readonly int X;
        public readonly int Y;
        public readonly string GumpData;
        public readonly string[] TextLines;

        public bool HasData
        {
            get { return GumpData != null; }
        }

        public CompressedGumpPacket(PacketReader reader)
            : base(0xDD, "Compressed Gump")
        {
            Serial = reader.ReadInt32();
            GumpID = reader.ReadInt32();
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            
            int compressedLength = reader.ReadInt32() - 4;
            int decompressedLength = reader.ReadInt32();
            byte[] compressedData = reader.ReadBytes(compressedLength);
            byte[] decompressedData = new byte[decompressedLength];

            if (Compression.Unpack(decompressedData, ref decompressedLength, compressedData, compressedLength) != ZLibError.Okay)
            {
                // Problem decompressing, go ahead and quit.
                return;
            }
            else
            {
                GumpData = Encoding.ASCII.GetString(decompressedData);

                int numTextLines = reader.ReadInt32();
                int compressedTextLength = reader.ReadInt32() - 4;
                int decompressedTextLength = reader.ReadInt32();
                byte[] decompressedText = new byte[decompressedTextLength];
                if (numTextLines > 0 && decompressedTextLength > 0)
                {
                    byte[] compressedTextData = reader.ReadBytes(compressedTextLength);
                    Compression.Unpack(decompressedText, ref decompressedTextLength, compressedTextData, compressedTextLength);
                    int index = 0;
                    List<string> lines = new List<string>();
                    for (int i = 0; i < numTextLines; i++)
                    {
                        int length = decompressedText[index] * 256 + decompressedText[index + 1];
                        index += 2;
                        byte[] b = new byte[length * 2];
                        Array.Copy(decompressedText, index, b, 0, length * 2);
                        index += length * 2;
                        lines.Add(Encoding.BigEndianUnicode.GetString(b));
                    }
                    TextLines = lines.ToArray();
                }
                else
                {
                    TextLines = new string[0];
                }
            }
        }
    }
}
