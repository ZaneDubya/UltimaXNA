/***************************************************************************
 *   Art.cs
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
using System.IO;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.Data
{
    class Art
    {
        private static Texture2D[][] _cache = new Texture2D[0x10000][];
        private static FileIndexClint _index = new FileIndexClint("artidx.mul", "art.mul");
        private static ushort[][] _dimensions = new ushort[0x4000][];
        private static GraphicsDevice _graphics;

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
                _cache[index][0] = data = readLandTexture(index, 0);
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
                _cache[index][0] = data = readStaticTexture(index, 0);
            }

            return data;
        }

        public static Texture2D GetStaticTexture(int index, int hue)
        {
            index &= 0x3FFF;
            index += 0x4000;

            if (_cache[index] == null) { _cache[index] = new Texture2D[0x1000]; }
            Texture2D data = _cache[index][hue];

            if (data == null)
            {
                _cache[index][hue] = data = readStaticTexture(index, hue);
            }

            return data;
        }

        private static unsafe Texture2D readLandTexture(int index, int hue)
        {
            _index.Seek(index);

            ushort[] data = new ushort[44 * 44];

            int count = 2;
            int offset = 21;

            fixed (ushort* pData = data)
            {
                ushort* dataRef = pData;

                for (int y = 0; y < 22; y++, count += 2, offset--, dataRef += 44)
                {
                    ushort* start = dataRef + offset;
                    ushort* end = start + count;

                    Metrics.ReportDataRead(count * 2);

                    while (start < end)
                    {
                        *start++ = (ushort)(_index.BinaryReader.ReadUInt16() ^ 0x8000);
                    }
                }

                count = 44;
                offset = 0;

                for (int y = 0; y < 22; y++, count -= 2, offset++, dataRef += 44)
                {
                    ushort* start = dataRef + offset;
                    ushort* end = start + count;

                    Metrics.ReportDataRead(count * 2);

                    while (start < end)
                    {
                        *start++ = (ushort)(_index.BinaryReader.ReadUInt16() ^ 0x8000);
                    }
                }
            }

            Texture2D texture = new Texture2D(_graphics, 44, 44, 1, TextureUsage.None, SurfaceFormat.Bgra5551);

            texture.SetData<ushort>(data);

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

        private static unsafe Texture2D readStaticTexture(int index, int hue)
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

            int[] lookups = new int[height];

            int dataStart = (int)_index.BinaryReader.BaseStream.Position + (height * 2);

            for (int i = 0; i < height; i++)
            {
                lookups[i] = (int)(dataStart + (_index.BinaryReader.ReadUInt16() * 2));
            }
            Metrics.ReportDataRead(height * 2);

            Hue hueObject = null;
            hue = (hue & 0x3FFF) - 1;
            if (hue >= 0 && hue < Hues.List.Length)
            {
                hueObject = Hues.List[hue];
            }

            ushort[] data = new ushort[width * height];

            fixed (ushort* pData = data)
            {
                ushort* dataRef = pData;

                for (int y = 0; y < height; y++, dataRef += width)
                {
                    _index.BinaryReader.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                    ushort* start = dataRef;

                    int count, offset;

                    while (((offset = _index.BinaryReader.ReadUInt16()) + (count = _index.BinaryReader.ReadUInt16())) != 0)
                    {
                        start += offset;
                        ushort* end = start + count;

                        Metrics.ReportDataRead(count * 2);

                        while (start < end)
                        {
                            if (hue < 0)
                            {
                                *start++ = (ushort)(_index.BinaryReader.ReadUInt16() ^ 0x8000);
                            }
                            else
                            {
                                *start++ = hueObject.GetColorUShort(_index.BinaryReader.ReadUInt16() >> 10);
                            }
                        }
                    }
                }
            }

            Texture2D texture = new Texture2D(_graphics, width, height, 1, TextureUsage.None, SurfaceFormat.Bgra5551);

            texture.SetData<ushort>(data);

            return texture;
        }
    }
}