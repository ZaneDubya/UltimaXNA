/***************************************************************************
 *   ArtResource.cs
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
using UltimaXNA.Core.Data;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    class ArtMulResource
    {
        private Texture2D[] m_LandTileTextureCache;
        private Texture2D[] m_StaticTileTextureCache;
        private Pair<int>[] m_StaticDimensions;

        private GraphicsDevice m_Graphics;
        private FileIndex m_FileIndex;

        public ArtMulResource(GraphicsDevice graphics)
        {
            m_Graphics = graphics;
            m_FileIndex = new FileIndex("artidx.mul", "art.mul", 0x10000, -1); // !!! must find patch file reference for artdata.

            m_LandTileTextureCache = new Texture2D[0x4000];
            m_StaticTileTextureCache = new Texture2D[0x4000];
            m_StaticDimensions = new Pair<int>[0x4000];
        }

        public Texture2D GetLandTexture(int index)
        {
            index &= 0x3FFF;

            if (m_LandTileTextureCache[index] == null)
            {
                m_LandTileTextureCache[index] = ReadLandTexture(index);
            }

            return m_LandTileTextureCache[index];
        }

        public Texture2D GetStaticTexture(int index)
        {
            index &= 0x3FFF;
            if (m_StaticTileTextureCache[index] == null)
            {
                Texture2D texture;
                Pair<int> dimensions;
                ReadStaticTexture(index + 0x4000, out texture, out dimensions);
                m_StaticTileTextureCache[index] = texture;
                m_StaticDimensions[index] = dimensions;
            }

            return m_StaticTileTextureCache[index];
        }

        public void GetStaticDimensions(int index, out int width, out int height)
        {
            index &= 0x3FFF;
            if (m_StaticTileTextureCache[index] == null)
            {
                GetStaticTexture(index);
            }
            Pair<int> dimensions = m_StaticDimensions[index];
            width = dimensions.A;
            height = dimensions.B;
        }

        private unsafe Texture2D ReadLandTexture(int index)
        {
            int length, extra;
            bool is_patched;

            BinaryFileReader reader = m_FileIndex.Seek(index, out length, out extra, out is_patched);
            if (reader == null)
                return null;

            ushort[] pixels = new ushort[44 * 44];

            ushort[] data = reader.ReadUShorts(23 * 44); // land tile textures only store their opaque pixels - see Art.mul file format.
            int i = 0;

            fixed (ushort* pData = pixels)
            {
                ushort* dataRef = pData;

                // fill the top half of the tile
                int count = 2;
                int offset = 21;
                for (int y = 0; y < 22; y++, count += 2, offset--, dataRef += 44)
                {
                    ushort* start = dataRef + offset;
                    ushort* end = start + count;

                    Metrics.ReportDataRead(count * 2);

                    while (start < end)
                    {
                        ushort color = data[i++];
                        *start++ = (ushort)(color | 0x8000);
                    }
                }

                // file the bottom half of the tile
                count = 44;
                offset = 0;
                for (int y = 0; y < 22; y++, count -= 2, offset++, dataRef += 44)
                {
                    ushort* start = dataRef + offset;
                    ushort* end = start + count;

                    Metrics.ReportDataRead(count * 2);

                    while (start < end)
                    {
                        ushort color = data[i++];
                        *start++ = (ushort)(color | 0x8000);
                    }
                }
            }

            Texture2D texture = new Texture2D(m_Graphics, 44, 44, false, SurfaceFormat.Bgra5551);
            texture.SetData<ushort>(pixels);

            return texture;
        }

        private unsafe void ReadStaticTexture(int index, out Texture2D texture, out Pair<int> dimensions)
        {
            texture = null;
            dimensions = new Pair<int>(0, 0);

            int length, extra;
            bool is_patched;

            // get a reader inside Art.Mul
            BinaryFileReader reader = m_FileIndex.Seek(index, out length, out extra, out is_patched);
            if (reader == null)
            {
                return;
            }

            reader.ReadInt(); // don't need this, see Art.mul file format.

            // get the dimensions of the texture
            int width = reader.ReadShort();
            int height = reader.ReadShort();
            dimensions = new Pair<int>(width, height);
            if (width <= 0 || height <= 0)
            {
                return;
            }

            // read the texture data!
            ushort[] lookups = reader.ReadUShorts(height);

            ushort[] data = reader.ReadUShorts(length - lookups.Length * 2 - 8);
            ushort[] pixels = new ushort[width * height];

            fixed (ushort* pData = pixels)
            {
                ushort* dataRef = pData;
                int i;

                for (int y = 0; y < height; y++, dataRef += width)
                {
                    i = lookups[y];

                    ushort* start = dataRef;

                    int count, offset;

                    while (((offset = data[i++]) + (count = data[i++])) != 0)
                    {
                        start += offset;
                        ushort* end = start + count;

                        while (start < end)
                        {
                            ushort color = data[i++];
                            *start++ = (ushort)(color | 0x8000);
                        }
                    }
                }
            }

            Metrics.ReportDataRead(sizeof(ushort) * (data.Length + lookups.Length + 2));

            texture = new Texture2D(m_Graphics, width, height, false, SurfaceFormat.Bgra5551);
            texture.SetData<ushort>(pixels);

            return;
        }
    }
}
