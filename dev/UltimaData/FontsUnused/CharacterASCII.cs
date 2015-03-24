using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using UltimaXNA.UltimaData.FontsNew;

namespace UltimaXNA.UltimaData.FontsUnused
{
    class CharacterASCII : ACharacter
    {
        public CharacterASCII(BinaryReader reader)
        {
            Width = reader.ReadByte();
            Height = reader.ReadByte();
            reader.ReadByte(); // unknown byte value... is this a delimeter or an offset?

            if (Width > 0 && Height > 0)
            {
                m_PixelData = new uint[Width * Height];

                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        ushort data = (ushort)(reader.ReadByte() | (reader.ReadByte() << 8));

                        uint pixel = 0x000000;

                        if (data != 0)
                        {
                            pixel = (0xFF000000 | (uint)(
                                ((((data >> 10) & 0x1F) * 0xFF / 0x1F)) |
                                ((((data >> 5) & 0x1F) * 0xFF / 0x1F) << 8) |
                                (((data & 0x1F) * 0xFF / 0x1F) << 16)
                                ));
                        }

                        m_PixelData[x + y * Width] = pixel;
                    }
                }
            }
        }
    }
}
