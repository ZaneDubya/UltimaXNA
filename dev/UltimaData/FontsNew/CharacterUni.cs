using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaData.FontsNew
{
    internal class CharacterUni : ACharacter
    {
        public CharacterUni(BinaryReader reader)
        {
            XOffset = reader.ReadByte();
            YOffset = reader.ReadByte();
            Width = reader.ReadByte();
            Height = reader.ReadByte();

            // only read data if there is UltimaData...
            if ((this.Width > 0) && (this.Height > 0))
            {
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
