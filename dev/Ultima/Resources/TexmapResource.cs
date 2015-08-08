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
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.IO;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    class TexmapResource
    {
        private Texture2D[] m_Cache = new Texture2D[0x4000];
        private FileIndex m_Index = new FileIndex("texidx.mul", "texmaps.mul", 0x4000, -1); // !!! must find patch file reference for texmap.
        private GraphicsDevice m_Graphics;

        public TexmapResource(GraphicsDevice graphics)
        {
            m_Graphics = graphics;
        }

        public Texture2D GetTexmapTexture(int index)
        {
            index &= 0x3FFF;

            if (m_Cache[index] == null)
            {
                m_Cache[index] = readTexmapTexture(index);
                if (m_Cache[index] == null)
                    m_Cache[index] = GetTexmapTexture(127);
            }

            return m_Cache[index];
        }

        private unsafe Texture2D readTexmapTexture(int index)
        {
            int length, extra;
            bool is_patched;

            BinaryFileReader reader = m_Index.Seek(index, out length, out extra, out is_patched);
            if (reader == null)
                return null;
            if (reader.Stream.Length == 0)
            {
                Tracer.Warn("Requested texmap texture #{0} does not exist. Replacing with 'unused' graphic.", index);
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
                    *pDataRef++ = color;
                    count++;
                }
            }

            Texture2D texture = new Texture2D(m_Graphics, textureSize, textureSize, false, SurfaceFormat.Bgra5551);

            texture.SetData<ushort>(pixelData);

            Metrics.ReportDataRead((int)reader.Position - metrics_dataread_start);

            return texture;
        }
    }
}