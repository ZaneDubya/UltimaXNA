/***************************************************************************
 *   ArtData.cs
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
    class ArtData
    {
        private static Texture2D[][] m_cache;
        private static FileIndex m_Index;
        private static ushort[][] m_dimensions;
        private static GraphicsDevice m_graphics;

        static ArtData()
        {
            m_cache = new Texture2D[0x10000][];
            m_Index = new FileIndex("artidx.mul", "art.mul", 0x10000, -1); // !!! must find patch file reference for artdata.
            m_dimensions = new ushort[0x4000][];
        }

        public static void Initialize(GraphicsDevice graphics)
        {
            m_graphics = graphics;
        }

        public static Texture2D GetLandTexture(int index)
        {
            index &= 0x3FFF;

            if (m_cache[index] == null) { m_cache[index] = new Texture2D[0x1000]; }
            Texture2D data = m_cache[index][0];

            if (data == null)
            {
                m_cache[index][0] = data = readLandTexture(index);
            }

            return data;
        }

        public static Texture2D GetStaticTexture(int index)
        {
            index &= 0x3FFF;
            index += 0x4000;

            if (m_cache[index] == null) { m_cache[index] = new Texture2D[0x1000]; }
            Texture2D data = m_cache[index][0];

            if (data == null)
            {
                m_cache[index][0] = data = readStaticTexture(index);
            }

            return data;
        }

        private static unsafe Texture2D readLandTexture(int index)
        {
            int length, extra;
            bool is_patched;

            BinaryFileReader reader = m_Index.Seek(index, out length, out extra, out is_patched);
            if (reader == null)
                return null;

            uint[] data = new uint[44 * 44];

            ushort[] fileData = reader.ReadUShorts(((44 + 2) / 2) * 44);
            int i = 0;

            int count = 2;
            int offset = 21;

            fixed (uint* pData = data)
            {
                uint* dataRef = pData;

                for (int y = 0; y < 22; y++, count += 2, offset--, dataRef += 44)
                {
                    uint* start = dataRef + offset;
                    uint* end = start + count;

                    Metrics.ReportDataRead(count * 2);

                    while (start < end)
                    {
                        uint color = fileData[i++];
                        *start++ = 0xFF000000 + (
                                    ((((color >> 10) & 0x1F) * multiplier)) |
                                    ((((color >> 5) & 0x1F) * multiplier) << 8) |
                                    (((color & 0x1F) * multiplier) << 16)
                                    );
                    }
                }

                count = 44;
                offset = 0;

                for (int y = 0; y < 22; y++, count -= 2, offset++, dataRef += 44)
                {
                    uint* start = dataRef + offset;
                    uint* end = start + count;

                    Metrics.ReportDataRead(count * 2);

                    while (start < end)
                    {
                        uint color = fileData[i++];
                        *start++ = 0xFF000000 + (
                                    ((((color >> 10) & 0x1F) * multiplier)) |
                                    ((((color >> 5) & 0x1F) * multiplier) << 8) |
                                    (((color & 0x1F) * multiplier) << 16)
                                    );
                    }
                }
            }

            Texture2D texture = new Texture2D(m_graphics, 44, 44);

            texture.SetData<uint>(data);

            return texture;
        }

        public static void GetStaticDimensions(int index, out int width, out int height)
        {
            index &= 0x3FFF;

            ushort[] dimensions = m_dimensions[index];

            if (dimensions == null)
            {
                m_dimensions[index] = dimensions = readStaticDimensions(index + 0x4000);
            }

            width = dimensions[0];
            height = dimensions[1];
        }

        private static ushort[] readStaticDimensions(int index)
        {
            int length, extra;
            bool is_patched;

            BinaryFileReader reader = m_Index.Seek(index, out length, out extra, out is_patched);
            reader.ReadInt();

            return new ushort[] { reader.ReadUShort(), reader.ReadUShort() };
        }

        const int multiplier = 0xFF / 0x1F;

        static int getMaxLookup(ushort[] lookups)
        {
            int max = 0;
            for (int i = 0; i < lookups.Length; i++)
            {
                if (lookups[i] > max)
                    max = lookups[i];
            }
            return max;
        }

        private static unsafe Texture2D readStaticTexture(int index)
        {
            int length, extra;
            bool is_patched;

            BinaryFileReader reader = m_Index.Seek(index, out length, out extra, out is_patched);

            if(reader == null)
            {
                return null;
            }

            reader.ReadInt(); // this data is discarded. Why?

            int width = reader.ReadShort();
            int height = reader.ReadShort();

            if (width <= 0 || height <= 0)
            {
                return null;
            }

            if (m_dimensions[index - 0x4000] == null)
            {
                m_dimensions[index - 0x4000] = new ushort[] { (ushort)width, (ushort)height };
            }


            ushort[] lookups = reader.ReadUShorts(height);

            int dataStart = (int)reader.Position + (height * 2);
            int readLength = (getMaxLookup(lookups) + width * 2); // we don't know the length of the last line, so we read width * 2, anticipating worst case compression.
            if (dataStart + readLength * 2 > reader.Stream.Length)
                readLength = ((int)reader.Stream.Length - dataStart) >> 1;
            ushort[] fileData = reader.ReadUShorts(readLength);
            uint[] pixelData = new uint[width * height];

            fixed (uint* pData = pixelData)
            {
                uint* dataRef = pData;
                int i;

                for (int y = 0; y < height; y++, dataRef += width)
                {
                    i = lookups[y];

                    uint* start = dataRef;

                    int count, offset;

                    while (((offset = fileData[i++]) + (count = fileData[i++])) != 0)
                    {
                        start += offset;
                        uint* end = start + count;

                        while (start < end)
                        {
                            uint color = fileData[i++];
                            *start++ = 0xFF000000 + (
                                    ((((color >> 10) & 0x1F) * multiplier)) |
                                    ((((color >> 5) & 0x1F) * multiplier) << 8) |
                                    (((color & 0x1F) * multiplier) << 16)
                                    );
                        }
                    }
                }
            }

            Metrics.ReportDataRead(sizeof(ushort) * (fileData.Length + lookups.Length + 2));

            Texture2D texture = new Texture2D(m_graphics, width, height);

            texture.SetData<uint>(pixelData);

            return texture;
        }
    }
}