/***************************************************************************
 *   TexmapData.cs
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
    class TexmapData
    {
        private static Texture2D[] m_Cache = new Texture2D[0x4000];
        private static FileIndex m_Index = new FileIndex("texidx.mul", "texmaps.mul", 0x4000, -1); // !!! must find patch file reference for artdata.
        private static GraphicsDevice m_graphics;

        public static void Initialize(GraphicsDevice graphics)
        {
            m_graphics = graphics;
        }

        public static Texture2D GetTexmapTexture(int index)
        {
            int i = index & 0x3FFF;

            if (m_Cache[i] == null)
            {
                m_Cache[i] = readTexmapTexture(i);
            }

            return m_Cache[i];
        }

        const int multiplier = 0xFF / 0x1F;

        private static unsafe Texture2D readTexmapTexture(int index)
        {
            int length, extra;
            bool is_patched;

            BinaryFileReader reader = m_Index.Seek(index, out length, out extra, out is_patched);
            if (reader == null)
                return null;
            if (reader.Stream.Length == 0)
            {
                UltimaXNA.Core.Diagnostics.Tracing.Tracer.Critical("Empty texmap texture with index {0}!", index);
                return null;
            }

            int metrics_dataread_start = (int)reader.Position;

            int textureSize = (extra == 0) ? 64 : 128;

            ushort[] pixelData = new ushort[textureSize * textureSize];
            ushort[] fileData = reader.ReadUShorts(textureSize * textureSize);

            fixed (ushort* pData = pixelData)
            {
                ushort* pDataRef = pData;

                int count = 0;
                int max = textureSize * textureSize;

                while (count < max)
                {
                    ushort color = (ushort)(fileData[count] | 0x8000);
                    *pDataRef++ = color; /* 0xFF000000 + (
                                     ((((color >> 10) & 0x1F) * multiplier)) |
                                     ((((color >> 5) & 0x1F) * multiplier) << 8) |
                                     (((color & 0x1F) * multiplier) << 16)
                                     );*/
                    count++;
                }
            }

            Texture2D texture = new Texture2D(m_graphics, textureSize, textureSize, false, SurfaceFormat.Bgra5551);

            texture.SetData<ushort>(pixelData);

            Metrics.ReportDataRead((int)reader.Position - metrics_dataread_start);

            return texture;
        }
    }
}