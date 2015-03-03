/***************************************************************************
 *   Texmaps.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
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

namespace UltimaXNA.UltimaData
{
    class TexmapData
    {
        private static Texture2D[] m_Cache = new Texture2D[0x4000];
        private static FileIndexClint m_Index = new FileIndexClint("texidx.mul", "texmaps.mul");
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
            int textureSize;

            m_Index.Seek(index, out textureSize);

            int streamStart = (int)m_Index.BinaryReader.BaseStream.Position;

            textureSize = (textureSize == 0) ? 64 : 128;

            uint[] pixelData = new uint[textureSize * textureSize];
            ushort[] fileData = m_Index.ReadUInt16Array(textureSize * textureSize);

            fixed (uint* pData = pixelData)
            {
                uint* pDataRef = pData;

                int count = 0;
                int max = textureSize * textureSize;

                while (count < max)
                {
                    uint color = fileData[count];
                    *pDataRef++ = 0xff000000 + (
                                    ((((color >> 10) & 0x1F) * multiplier)) |
                                    ((((color >> 5) & 0x1F) * multiplier) << 8) |
                                    (((color & 0x1F) * multiplier) << 16)
                                    );
                    count++;
                }
            }

            Texture2D texture = new Texture2D(m_graphics, textureSize, textureSize);

            texture.SetData<uint>(pixelData);

            UltimaVars.Metrics.ReportDataRead((int)m_Index.BinaryReader.BaseStream.Position - streamStart);

            return texture;
        }
    }
}