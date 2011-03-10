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

        private const int multiplier = 0xFF / 0x1F;

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

                BinaryReader bin = new BinaryReader(stream);
                int start = (int)bin.BaseStream.Position;

                int[] lookups = FileIndexClint.ReadInt32Array(bin, height);
                ushort[] fileData = FileIndexClint.ReadUInt16Array(bin, length - (height * 2));

                uint[] pixels = new uint[width * height];

                fixed (uint* line = &pixels[0])
                {
                    fixed (ushort* data = &fileData[0])
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            ushort* dataRef = data + (lookups[y] - height) * 2;

                            uint* cur = line + (y * width);
                            uint* end = cur + width;

                            while (cur < end)
                            {
                                uint color = *dataRef++;
                                uint* next = cur + *dataRef++;

                                if (color == 0)
                                {
                                    cur = next;
                                }
                                else
                                {
                                    uint color32 = 0xff000000 + (
                                        ((((color >> 10) & 0x1F) * multiplier)) |
                                        ((((color >> 5) & 0x1F) * multiplier) << 8) |
                                        (((color & 0x1F) * multiplier) << 16)
                                        );
                                    while (cur < next)
                                        *cur++ = color32;
                                }
                            }
                        }
                    }
                }

                Metrics.ReportDataRead(length);

                Texture2D texture = new Texture2D(_graphicsDevice, width, height, false, SurfaceFormat.Color);
                texture.SetData(pixels);
                _cache[index] = texture;
            }
            return _cache[index];
        }
    }
}