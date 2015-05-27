/***************************************************************************
 *   Gumps.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.IO;

#endregion

namespace UltimaXNA.Ultima.IO
{
    public class GumpData
    {
        private static GraphicsDevice m_graphicsDevice;

        private static FileIndex m_FileIndex = new FileIndex("Gumpidx.mul", "Gumpart.mul", 0x10000, 12);
        public static FileIndex FileIndex { get { return m_FileIndex; } }

        private static Texture2D[] m_cache = new Texture2D[0x10000];

        private const int multiplier = 0xFF / 0x1F;

        public static void Initialize(GraphicsDevice graphics)
        {
            m_graphicsDevice = graphics;
        }

        public unsafe static Texture2D GetGumpXNA(int index, bool replaceMask080808 = false)
        {
            if (index < 0)
                return null;

            if (m_cache[index] == null)
            {
                int length, extra;
                bool patched;

                BinaryFileReader reader = m_FileIndex.Seek(index, out length, out extra, out patched);
                if (reader == null)
                    return null;

                int width = (extra >> 16) & 0xFFFF;
                int height = extra & 0xFFFF;

                int metrics_dataread_start = (int)reader.Position;

                int[] lookups = reader.ReadInts(height);
                ushort[] fileData = reader.ReadUShorts(length - (height * 2));

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
                                    uint color32 = 0xFF000000 + (
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

                if (replaceMask080808)
                {
                    for (int i = 0; i < pixels.Length; i++)
                        if (pixels[i] == 0xff080808)
                            pixels[i] = 0xffff00ff;
                }

                Texture2D texture = new Texture2D(m_graphicsDevice, width, height, false, SurfaceFormat.Color);
                texture.SetData(pixels);
                m_cache[index] = texture;
            }
            return m_cache[index];
        }
    }
}