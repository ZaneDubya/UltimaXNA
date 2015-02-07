/***************************************************************************
 *   Art.cs
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
using System.IO;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.UltimaData
{
    class Art
    {
        private static Texture2D[][] _cache;
        private static FileIndexClint _index;
        private static ushort[][] _dimensions;
        private static GraphicsDevice _graphics;

        static Art()
        {
            _cache = new Texture2D[0x10000][];
            _index = new FileIndexClint("artidx.mul", "art.mul");
            _dimensions = new ushort[0x4000][];
        }

        public static void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;
        }

        public static Texture2D GetLandTexture(int index)
        {
            index &= 0x3FFF;

            if (_cache[index] == null) { _cache[index] = new Texture2D[0x1000]; }
            Texture2D data = _cache[index][0];

            if (data == null)
            {
                _cache[index][0] = data = readLandTexture(index);
            }

            return data;
        }

        public static Texture2D GetStaticTexture(int index)
        {
            index &= 0x3FFF;
            index += 0x4000;

            if (_cache[index] == null) { _cache[index] = new Texture2D[0x1000]; }
            Texture2D data = _cache[index][0];

            if (data == null)
            {
                _cache[index][0] = data = readStaticTexture(index);
            }

            return data;
        }

        private static unsafe Texture2D readLandTexture(int index)
        {
            _index.Seek(index);

            uint[] data = new uint[44 * 44];

            ushort[] fileData = _index.ReadUInt16Array(((44 + 2) / 2) * 44);
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

                    UltimaVars.Metrics.ReportDataRead(count * 2);

                    while (start < end)
                    {
                        uint color = fileData[i++];
                        *start++ = 0xff000000 + (
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

                    UltimaVars.Metrics.ReportDataRead(count * 2);

                    while (start < end)
                    {
                        uint color = fileData[i++];
                        *start++ = 0xff000000 + (
                                    ((((color >> 10) & 0x1F) * multiplier)) |
                                    ((((color >> 5) & 0x1F) * multiplier) << 8) |
                                    (((color & 0x1F) * multiplier) << 16)
                                    );
                    }
                }
            }

            Texture2D texture = new Texture2D(_graphics, 44, 44);

            texture.SetData<uint>(data);

            return texture;
        }

        public static void GetStaticDimensions(int index, out int width, out int height)
        {
            index &= 0x3FFF;

            ushort[] dimensions = _dimensions[index];

            if (dimensions == null)
            {
                _dimensions[index] = dimensions = readStaticDimensions(index + 0x4000);
            }

            width = dimensions[0];
            height = dimensions[1];
        }

        private static ushort[] readStaticDimensions(int index)
        {
            _index.Seek(index);
            _index.BinaryReader.ReadInt32();

            return new ushort[] { (ushort)_index.BinaryReader.ReadInt16(), (ushort)_index.BinaryReader.ReadInt16() };
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
            _index.Seek(index);
            _index.BinaryReader.ReadInt32();

            int width = _index.BinaryReader.ReadInt16();
            int height = _index.BinaryReader.ReadInt16();

            if (width <= 0 || height <= 0)
            {
                return null;
            }

            if (_dimensions[index - 0x4000] == null)
            {
                _dimensions[index - 0x4000] = new ushort[] { (ushort)width, (ushort)height };
            }

            
            ushort[] lookups = _index.ReadUInt16Array(height);

            int dataStart = (int)_index.BinaryReader.BaseStream.Position + (height * 2);
            int readLength = (getMaxLookup(lookups) + width * 2); // we don't know the length of the last line, so we read width * 2, anticipating worst case compression.
            if (dataStart + readLength * 2 > _index.BinaryReader.BaseStream.Length)
                readLength = ((int)_index.BinaryReader.BaseStream.Length - dataStart) >> 1;
            ushort[] fileData = _index.ReadUInt16Array(readLength);
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
                            *start++ = 0xff000000 + (
                                    ((((color >> 10) & 0x1F) * multiplier)) |
                                    ((((color >> 5) & 0x1F) * multiplier) << 8) |
                                    (((color & 0x1F) * multiplier) << 16)
                                    );
                        }
                    }
                }
            }

            UltimaVars.Metrics.ReportDataRead(sizeof(ushort) * (fileData.Length + lookups.Length + 2));

            Texture2D texture = new Texture2D(_graphics, width, height);

            texture.SetData<uint>(pixelData);

            return texture;
        }
    }
}