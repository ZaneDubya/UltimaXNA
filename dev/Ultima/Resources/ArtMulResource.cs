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
using UltimaXNA.Ultima.Data;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    class ArtMulResource
    {
        GraphicsDevice m_Graphics;
        AFileIndex m_FileIndex;
        PixelPicking m_StaticPicking;
        Texture2D[] m_LandTileTextureCache;
        Texture2D[] m_StaticTileTextureCache;

        public ArtMulResource(GraphicsDevice graphics)
        {
            m_Graphics = graphics;
            m_FileIndex = ClientVersion.InstallationIsUopFormat ? FileManager.CreateFileIndex("artLegacyMUL.uop", 0x10000, false, ".tga") : FileManager.CreateFileIndex("artidx.mul", "art.mul", 0x10000, -1); // !!! must find patch file reference for artdata.
            m_StaticPicking = new PixelPicking();
            m_LandTileTextureCache = new Texture2D[0x10000];
            m_StaticTileTextureCache = new Texture2D[0x10000];
        }

        public Texture2D GetLandTexture(int index)
        {
            index &= FileManager.ItemIDMask;

            if (m_LandTileTextureCache[index] == null)
            {
                m_LandTileTextureCache[index] = ReadLandTexture(index);
            }

            return m_LandTileTextureCache[index];
        }

        public Texture2D GetStaticTexture(int index)
        {
            index &= FileManager.ItemIDMask;

            if (m_StaticTileTextureCache[index] == null)
            {
                Texture2D texture;
                ReadStaticTexture(index + 0x4000, out texture);
                m_StaticTileTextureCache[index] = texture;
            }

            return m_StaticTileTextureCache[index];
        }

        public void GetStaticDimensions(int index, out int width, out int height)
        {
            index &= FileManager.ItemIDMask;
            if (m_StaticTileTextureCache[index] == null)
            {
                GetStaticTexture(index);
            }
            m_StaticPicking.GetDimensions(index + 0x4000, out width, out height);
        }

        public bool IsPointInItemTexture(int index, int x, int y, int extraRange = 0)
        {
            if (m_StaticTileTextureCache[index] == null)
            {
                GetStaticTexture(index);
            }
            return m_StaticPicking.Get(index + 0x4000, x, y, extraRange);
        }

        private unsafe Texture2D ReadLandTexture(int index)
        {
            int length, extra;
            bool is_patched;
            BinaryFileReader reader = m_FileIndex.Seek(index, out length, out extra, out is_patched);
            if (reader == null)
            {
                return null;
            }
            ushort[] pixels = new ushort[44 * 44];
            ushort[] data = reader.ReadUShorts(23 * 44); // land tile textures store only opaque pixels
            Metrics.ReportDataRead(data.Length);
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
                    while (start < end)
                    {
                        ushort color = data[i++];
                        *start++ = (ushort)(color | 0x8000);
                    }
                }
            }
            Texture2D texture = new Texture2D(m_Graphics, 44, 44, false, SurfaceFormat.Bgra5551);
            texture.SetData(pixels);
            return texture;
        }

        private unsafe void ReadStaticTexture(int index, out Texture2D texture)
        {
            texture = null;
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
            if (width <= 0 || height <= 0)
            {
                return;
            }
            // read the texture data!
            ushort[] lookups = reader.ReadUShorts(height);
            ushort[] data = reader.ReadUShorts(length - lookups.Length * 2 - 8);
            Metrics.ReportDataRead(sizeof(ushort) * (data.Length + lookups.Length + 2));
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
            texture = new Texture2D(m_Graphics, width, height, false, SurfaceFormat.Bgra5551);
            texture.SetData(pixels);
            m_StaticPicking.Set(index, width, height, pixels);
            return;
        }
    }
}
