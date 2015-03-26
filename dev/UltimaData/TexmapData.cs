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
using InterXLib;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Diagnostics;
#endregion

namespace UltimaXNA.UltimaData
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

            int metrics_dataread_start = (int)reader.Position;

            int textureSize = (extra == 0) ? 64 : 128;

            uint[] pixelData = new uint[textureSize * textureSize];
            ushort[] fileData = reader.ReadUShorts(textureSize * textureSize);

            fixed (uint* pData = pixelData)
            {
                uint* pDataRef = pData;

                int count = 0;
                int max = textureSize * textureSize;

                while (count < max)
                {
                    uint color = fileData[count];
                    *pDataRef++ = 0xFF000000 + (
                                    ((((color >> 10) & 0x1F) * multiplier)) |
                                    ((((color >> 5) & 0x1F) * multiplier) << 8) |
                                    (((color & 0x1F) * multiplier) << 16)
                                    );
                    count++;
                }
            }

            Texture2D texture = new Texture2D(m_graphics, textureSize, textureSize);

            texture.SetData<uint>(pixelData);

            Metrics.ReportDataRead((int)reader.Position - metrics_dataread_start);

            return texture;
        }
    }
}