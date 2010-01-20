/***************************************************************************
 *   Gumps.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.Data
{
    public class Gumps
    {
        private static GraphicsDevice _graphicsDevice;

        private static FileIndex m_FileIndex = new FileIndex("Gumpidx.mul", "Gumpart.mul", 0x10000, 12);
        public static FileIndex FileIndex { get { return m_FileIndex; } }

        private static Texture2D[] _cache = new Texture2D[0x10000];

        public static void Initialize(GraphicsDevice graphics)
        {
            _graphicsDevice = graphics;
        }

        public unsafe static Texture2D GetGumpXNA(int index)
        {
            if (_cache[index] == null)
            {
                int length, extra;
                bool patched;
                Stream stream = m_FileIndex.Seek(index, out length, out extra, out patched);

                if (stream == null)
                    return null;

                int width = (extra >> 16) & 0xFFFF;
                int height = extra & 0xFFFF;

                uint[] pixels = new uint[width * height];
                BinaryReader bin = new BinaryReader(stream);

                int[] lookups = new int[height];
                int start = (int)bin.BaseStream.Position;

                for (int i = 0; i < height; ++i)
                    lookups[i] = start + (bin.ReadInt32() * 4);

                fixed (uint* line = &pixels[0])
                {
                    for (int y = 0; y < height; ++y)
                    {
                        bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                        uint* cur = line + (y * width);
                        uint* end = cur + (width);

                        while (cur < end)
                        {
                            uint color = bin.ReadUInt16();
                            uint* next = cur + bin.ReadUInt16();

                            if (color == 0)
                            {
                                cur = next;
                            }
                            else
                            {
                                uint color32 = 0xff000000 + (
                                    ((((color >> 10) & 0x1F) * 0xFF / 0x1F) << 16) |
                                    ((((color >> 5) & 0x1F) * 0xFF / 0x1F) << 8) |
                                    (((color & 0x1F) * 0xFF / 0x1F))
                                    );
                                while (cur < next)
                                    *cur++ = color32;
                            }
                        }
                    }
                }

                Metrics.ReportDataRead(sizeof(UInt16) * height * width);

                Texture2D texture = new Texture2D(_graphicsDevice, width, height);
                texture.SetData(pixels);
                _cache[index] = texture;
            }
            return _cache[index];
        }
    }
}