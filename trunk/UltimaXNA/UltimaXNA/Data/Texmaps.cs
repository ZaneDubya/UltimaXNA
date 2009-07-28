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
using Microsoft.Xna.Framework.Graphics;
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

            extra = extra == 0 ? 64 : 128;

            ushort[] data = new ushort[extra * extra];

            fixed (ushort* pData = data)
            {
                ushort* pDataRef = pData;

                for (int y = 0; y < extra; ++y, pDataRef += extra)
                {
                    ushort* start = pDataRef;
                    ushort* end = start + extra;

                    while (start < end)
                    {
                        *start++ = (ushort)(m_Index.BinaryReader.ReadUInt16() ^ 0x8000);
                    }
                }
            }

            Texture2D texture = new Texture2D(_graphics, extra, extra, 1, TextureUsage.None, SurfaceFormat.Bgra5551);

            texture.SetData<ushort>(data);

            return texture;
        }
    }
}