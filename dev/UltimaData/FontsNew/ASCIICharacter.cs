using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaData.FontsNew
{
    class ASCIICharacter : ACharacter
    {
        private uint[] m_CharacterPixels = null;

        public ASCIICharacter(byte[] buffer, ref int pos)
        {
            Width = buffer[pos++];
            Height = buffer[pos++];
            pos++; // unknown byte value... is this a delimeter or an offset?

            if (Width > 0 && Height > 0)
            {
                m_CharacterPixels = new uint[Width * Height];

                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        ushort data = (ushort)(buffer[pos++] | (buffer[pos++] << 8));

                        uint pixel = 0x000000;

                        if (data != 0)
                        {
                            pixel = (0xFF000000 | (uint)(
                                ((((data >> 10) & 0x1F) * 0xFF / 0x1F)) |
                                ((((data >> 5) & 0x1F) * 0xFF / 0x1F) << 8) |
                                (((data & 0x1F) * 0xFF / 0x1F) << 16)
                                ));
                        }

                        m_CharacterPixels[x + y * Width] = pixel;
                    }
                }
            }
        }

        internal void WriteTextureData(uint[] textureData)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    textureData[(y + TextureBounds.Y) * Width + (x + TextureBounds.X)] = m_CharacterPixels[y * Width + x];
                }
            }
            m_CharacterPixels = null;
        }
    }
}
