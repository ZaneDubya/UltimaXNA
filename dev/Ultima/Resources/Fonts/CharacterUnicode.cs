/***************************************************************************
 *   CharacterUni.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.IO;
using UltimaXNA.Core.UI.Fonts;
#endregion

namespace UltimaXNA.Ultima.Resources.Fonts
{
    internal class CharacterUnicode : ACharacter
    {
        public CharacterUnicode()
        {

        }

        public CharacterUnicode(BinaryReader reader)
        {
            XOffset = (sbyte)reader.ReadByte();
            YOffset = (sbyte)reader.ReadByte();
            Width = reader.ReadByte();
            Height = reader.ReadByte();
            ExtraWidth = 1;

            // only read data if there is IO...
            if ((Width > 0) && (Height > 0))
            {
                m_PixelData = new uint[Width * Height];

                // At this point, we know we have data, so go ahead and start reading!
                for (int y = 0; y < Height; ++y)
                {
                    byte[] scanline = reader.ReadBytes(((Width - 1) / 8) + 1);
                    int bitX = 7;
                    int byteX = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        uint color = 0x00000000;
                        if ((scanline[byteX] & (byte)Math.Pow(2, bitX)) != 0)
                            color = 0xFFFFFFFF;

                        m_PixelData[y * Width + x] = color;

                        bitX--;
                        if (bitX < 0)
                        {
                            bitX = 7;
                            byteX++;
                        }
                    }
                }
            }
        }
    }
}
