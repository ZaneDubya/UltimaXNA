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
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.IO;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public class GumpMulResource
    {
        private GraphicsDevice m_graphicsDevice;

        private FileIndex m_FileIndex = new FileIndex("Gumpidx.mul", "Gumpart.mul", 0x10000, 12);
        public FileIndex FileIndex { get { return m_FileIndex; } }

        private Texture2D[] m_TextureCache = new Texture2D[0x10000];

        public GumpMulResource(GraphicsDevice graphics)
        {
            m_graphicsDevice = graphics;
        }

        public unsafe Texture2D GetGumpXNA(int index, bool replaceMask080808 = false)
        {
            if (index < 0)
                return null;

            if (m_TextureCache[index] == null)
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

                ushort[] pixels = new ushort[width * height];

                fixed (ushort* line = &pixels[0])
                {
                    fixed (ushort* data = &fileData[0])
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            ushort* dataRef = data + (lookups[y] - height) * 2;

                            ushort* cur = line + (y * width);
                            ushort* end = cur + width;

                            while (cur < end)
                            {
                                ushort color = *dataRef++;
                                ushort* next = cur + *dataRef++;

                                if (color == 0)
                                {
                                    cur = next;
                                }
                                else
                                {
                                    color |= 0x8000;
                                    while (cur < next)
                                        *cur++ = color;
                                }
                            }
                        }
                    }
                }

                Metrics.ReportDataRead(length);

                if (replaceMask080808)
                {
                    for (int i = 0; i < pixels.Length; i++)
                        if (pixels[i] == 0x8421)
                            pixels[i] = 0xFC1F;
                }

                Texture2D texture = new Texture2D(m_graphicsDevice, width, height, false, SurfaceFormat.Bgra5551);
                texture.SetData(pixels);
                m_TextureCache[index] = texture;
            }
            return m_TextureCache[index];
        }
    }
}