/***************************************************************************
 *   Texmaps.cs
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
    class Texmaps
    {
        private static Texture2D[] m_Cache = new Texture2D[0x4000];
        private static FileIndexClint m_Index = new FileIndexClint("texidx.mul", "texmaps.mul");
        private static GraphicsDevice _graphics;

        public static void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;
        }

        public static Texture2D GetTexmapTexture(int index)
        {
            Texture2D texture = m_Cache[index];

            if (texture == null)
            {
                m_Cache[index] = texture = readTexmapTexture(index);
            }

            return texture;
        }

        private static unsafe Texture2D readTexmapTexture(int index)
        {
            int extra;

            m_Index.Seek(index, out extra);

            int streamStart = (int)m_Index.BinaryReader.BaseStream.Position;

            extra = extra == 0 ? 64 : 128;

            uint[] data = new uint[extra * extra];

            fixed (uint* pData = data)
            {
                uint* pDataRef = pData;

                for (int y = 0; y < extra; ++y, pDataRef += extra)
                {
                    uint* start = pDataRef;
                    uint* end = start + extra;

                    while (start < end)
                    {
                        uint color = m_Index.BinaryReader.ReadUInt16();
                        *start++ = 0xff000000 + (
                                    ((((color >> 10) & 0x1F) * 0xFF / 0x1F) << 16) |
                                    ((((color >> 5) & 0x1F) * 0xFF / 0x1F) << 8) |
                                    (((color & 0x1F) * 0xFF / 0x1F))
                                    );
                    }
                }
            }

            Texture2D texture = new Texture2D(_graphics, extra, extra);

            texture.SetData<uint>(data);

            Metrics.ReportDataRead((int)m_Index.BinaryReader.BaseStream.Position - streamStart);

            return texture;
        }
    }
}