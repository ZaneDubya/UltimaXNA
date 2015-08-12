/***************************************************************************
 *   CharacterASCII.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.IO;
using UltimaXNA.Core.UI.Fonts;
#endregion

namespace UltimaXNA.Ultima.Resources.Fonts
{
    internal class CharacterAscii : ACharacter
    {
        public CharacterAscii()
        {

        }

        public CharacterAscii(BinaryReader reader)
        {
            Width = reader.ReadByte();
            Height = reader.ReadByte();
            UsePassedColor = false;

            reader.ReadByte(); // byte delimeter?
            int startY = Height;
            int endY = -1;
            uint[] pixels = null;

            if (Width > 0 && Height > 0)
            {
                pixels = new uint[Width * Height];

                int i = 0;
                for (int y = 0; y < Height; y++)
                {
                    bool rowHasData = false;
                    for (int x = 0; x < Width; x++)
                    {
                        short pixel = (short)(reader.ReadByte() | (reader.ReadByte() << 8));

                        if (pixel != 0)
                        {
                            pixels[i] = (uint)(0xFF000000 + (
                                ((((pixel >> 10) & 0x1F) * 0xFF / 0x1F)) |
                                ((((pixel >> 5) & 0x1F) * 0xFF / 0x1F) << 8) |
                                (((pixel & 0x1F) * 0xFF / 0x1F) << 16)
                                ));
                            rowHasData = true;
                        }
                        i++;
                    }
                    if (rowHasData)
                    {
                        if (startY > y)
                            startY = y;
                        endY = y;
                    }
                }
            }

            endY += 1;
            if (endY == 0)
            {
                m_PixelData = null;
            }
            else if (endY == Height)
            {
                m_PixelData = pixels;
            }
            else
            {
                m_PixelData = new uint[Width * endY];
                int i = 0;
                for (int y = 0; y < endY; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        m_PixelData[i++] = pixels[y * Width + x];
                    }
                }
                YOffset = Height - endY;
                Height = endY;
            }
        }
    }
}
