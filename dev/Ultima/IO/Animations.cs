/***************************************************************************
 *   AnimationData.cs
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
using System.IO;
using UltimaXNA.Core;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.IO;
using UltimaXNA.Core.Graphics;
#endregion

namespace UltimaXNA.Ultima.IO
{
    public sealed class Animations
    {
        private static FileIndex m_FileIndex = new FileIndex("Anim.idx", "Anim.mul", 0x40000, 6);
        public static FileIndex FileIndex { get { return m_FileIndex; } }

        private static FileIndex m_FileIndex2 = new FileIndex("Anim2.idx", "Anim2.mul", 0x10000, -1);
        public static FileIndex FileIndex2 { get { return m_FileIndex2; } }

        private static FileIndex m_FileIndex3 = new FileIndex("Anim3.idx", "Anim3.mul", 0x20000, -1);
        public static FileIndex FileIndex3 { get { return m_FileIndex3; } }

        private static FileIndex m_FileIndex4 = new FileIndex("Anim4.idx", "Anim4.mul", 0x20000, -1);
        public static FileIndex FileIndex4 { get { return m_FileIndex4; } }

        private static FileIndex m_FileIndex5 = new FileIndex("Anim5.idx", "Anim5.mul", 0x20000, -1);
        public static FileIndex FileIndex5 { get { return m_FileIndex5; } }

        private static AnimationFrame[][][][] m_Cache;
        private static GraphicsDevice m_graphics;
        private static int[] m_Table;

        static Animations()
        {
            SpriteBatch3D sb = ServiceRegistry.GetService<SpriteBatch3D>();
            m_graphics = sb.GraphicsDevice;
        }

        public static int GetAnimationFrameCount(int body, int action, int direction, int hue)
        {
            AnimationFrame[] frames = GetAnimation(body, action, direction, hue);
            if (frames == null)
                return 0;
            return frames.Length;
        }

        public static AnimationFrame[] GetAnimation(int body, int action, int direction, int hue)
        {
            int animIndex;
            FileIndex fileIndex;
            int length, extra;
            bool patched;

            getIndexes(ref body, ref hue, action, direction, out animIndex, out fileIndex);

            AnimationFrame[] f = checkCache(body, action, direction);
            if (f != null)
                return f;

            BinaryFileReader reader = fileIndex.Seek(animIndex, out length, out extra, out patched);
            if (reader == null)
                return null;

            AnimationFrame[] frames = GetAnimation(reader);
            return m_Cache[body][action][direction] = frames;
        }

        public static AnimationFrame[] GetAnimation(BinaryFileReader reader)
        {
            
            uint[] palette = getPalette(reader); // 0x100 * 2 = 0x0200 bytes
            int read_start = (int)reader.Position; // save file position after palette.

            int frameCount = reader.ReadInt(); // 0x04 bytes

            int[] lookups = new int[frameCount]; // frameCount * 0x04 bytes
            for (int i = 0; i < frameCount; ++i) { lookups[i] = reader.ReadInt(); }

            AnimationFrame[] frames = new AnimationFrame[frameCount];
            for (int i = 0; i < frameCount; ++i)
            {
                if (lookups[i] < lookups[0])
                {
                    frames[i] = AnimationFrame.Empty; // Fix for broken animations, per issue13
                }
                else
                {
                    reader.Seek(read_start + lookups[i], SeekOrigin.Begin);
                    frames[i] = new AnimationFrame(m_graphics, palette, reader);
                }
            }
            return frames;
        }

        public static byte[] GetData(int body, int action, int direction, int hue)
        {
            int animIndex;
            FileIndex fileIndex;
            int length, extra;
            bool patched;

            if (body >= 0x1000)
                return null;

            getIndexes(ref body, ref hue, action, direction, out animIndex, out fileIndex);
            BinaryFileReader reader = fileIndex.Seek(animIndex, out length, out extra, out patched);
            if (reader == null)
                return null;

            return reader.ReadBytes(length);
        }

        private static uint[] getPalette(BinaryFileReader reader)
        {
            uint[] pal = new uint[0x100];
            for (int i = 0; i < 0x100; ++i)
            {
                uint color = reader.ReadUShort();
                pal[i] = 0xFF000000 + (
                    ((((color >> 10) & 0x1F) * 0xFF / 0x1F)) |
                    ((((color >> 5) & 0x1F) * 0xFF / 0x1F) << 8) |
                    (((color & 0x1F) * 0xFF / 0x1F) << 16)
                    );
            }
            return pal;
        }

        private static AnimationFrame[] checkCache(int body, int action, int direction)
        {
            // Make sure the cache is complete.
            // max number of bodies is about 0x1000
            if (m_Cache == null) m_Cache = new AnimationFrame[0x1000][][][];
            if (m_Cache[body] == null)
                m_Cache[body] = new AnimationFrame[35][][];
            if (m_Cache[body][action] == null)
                m_Cache[body][action] = new AnimationFrame[8][];
            if (m_Cache[body][action][direction] == null)
                m_Cache[body][action][direction] = new AnimationFrame[1];
            if (m_Cache[body][action][direction][0] != null)
                return m_Cache[body][action][direction];
            else
                return null;
        }

        private static void getIndexes(ref int body, ref int hue, int action, int direction, out int index, out FileIndex fileIndex)
        {
            Translate(ref body, ref hue);

            int fileType = IO.BodyConverter.Convert(ref body);
            switch (fileType)
            {
                default:
                case 1:
                    {
                        fileIndex = m_FileIndex;

                        if (body < 200)
                            index = body * 110;
                        else if (body < 400)
                            index = 22000 + ((body - 200) * 65);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
                case 2:
                    {
                        fileIndex = m_FileIndex2;

                        if (body < 200)
                            index = body * 110;
                        else
                            index = 22000 + ((body - 200) * 65);

                        break;
                    }
                case 3:
                    {
                        fileIndex = m_FileIndex3;

                        if (body < 300)
                            index = body * 65;
                        else if (body < 400)
                            index = 33000 + ((body - 300) * 110);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
                case 4:
                    {
                        fileIndex = m_FileIndex4;

                        if (body < 200)
                            index = body * 110;
                        else if (body < 400)
                            index = 22000 + ((body - 200) * 65);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
                // Issue 60 - Missing (or wrong) object animations - http://code.google.com/p/ultimaxna/issues/detail?id=60 - Smjert
                case 5:
                    {
                        fileIndex = m_FileIndex5;
                        if ((body < 200) && (body != 34)) // looks strange, though it works.
                            index = body * 110;
                        else if (body < 400)
                            index = 22000 + ((body - 200) * 65);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
                // Issue 60 - End
            }

            index += action * 5;

            if (direction <= 4)
            {
                index += direction;
            }
            else
            {
                index += direction - (direction - 4) * 2;
            }
        }

        public static void Translate(ref int body)
        {
            if (m_Table == null)
                LoadTable();

            if (body <= 0 || body >= m_Table.Length)
            {
                body = 0;
                return;
            }

            body = (m_Table[body] & 0x7FFF);
        }

        public static void Translate(ref int body, ref int hue)
        {
            if (m_Table == null)
                LoadTable();

            if (body <= 0 || body >= m_Table.Length)
            {
                body = 0;
                return;
            }

            int table = m_Table[body];

            if ((table & (1 << 31)) != 0)
            {
                body = table & 0x7FFF;

                int vhue = (hue & 0x3FFF) - 1;

                if (vhue < 0 || vhue >= HueData.List.Length)
                    hue = (table >> 15) & 0xFFFF;
            }
        }

        private static void LoadTable()
        {
            int count = 400 + ((m_FileIndex.Index.Length - 35000) / 175);

            m_Table = new int[count];

            for (int i = 0; i < count; ++i)
            {
                object o = BodyTable.m_Entries[i];

                if (o == null || BodyConverter.Contains(i))
                {
                    m_Table[i] = i;
                }
                else
                {
                    BodyTableEntry bte = (BodyTableEntry)o;

                    m_Table[i] = bte.m_OldID | (1 << 31) | (((bte.m_NewHue ^ 0x8000) & 0xFFFF) << 15);
                }
            }
        }
    }

    public sealed class AnimationFrame
    {
        private Microsoft.Xna.Framework.Point m_Center;
        private Microsoft.Xna.Framework.Graphics.Texture2D m_Texture;

        public Microsoft.Xna.Framework.Point Center { get { return m_Center; } }
        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get { return m_Texture; } }

        private const int DoubleXor = (0x200 << 22) | (0x200 << 12);

        public static readonly AnimationFrame Empty = new AnimationFrame();
        public static readonly AnimationFrame[] EmptyFrames = new AnimationFrame[1] { Empty };

        private AnimationFrame()
        {
        }

        public unsafe AnimationFrame(GraphicsDevice graphics, uint[] palette, byte[] frame, int width, int height, int xCenter, int yCenter)
        {
            palette[0] = 0xFFFFFFFF;
            uint[] data = new uint[width * height];
            fixed (uint* pData = data)
            {
                uint* dataRef = pData;
                int frameIndex = 0;

                while (frameIndex < frame.Length)
                {
                    *dataRef++ = palette[frame[frameIndex++]];
                }
            }

            m_Center = new Microsoft.Xna.Framework.Point(xCenter, yCenter);
            m_Texture = new Texture2D(graphics, width, height);
            m_Texture.SetData<uint>(data);
            palette[0] = 0;
        }

        public unsafe AnimationFrame(GraphicsDevice graphics, uint[] palette, BinaryFileReader reader)
        {
            int xCenter = reader.ReadShort();
            int yCenter = reader.ReadShort();

            int width = reader.ReadUShort();
            int height = reader.ReadUShort();

            // Fix for animations with no IO.
            if ((width == 0) || (height == 0))
            {
                m_Texture = null;
                return;
            }

            uint[] data = new uint[width * height];

            int header;

            int xBase = xCenter - 0x200;
            int yBase = (yCenter + height) - 0x200;

            fixed (uint* pData = data)
            {
                uint* dataRef = pData;
                int delta = width;

                int dataRead = 0;

                dataRef += xBase;
                dataRef += (yBase * delta);

                while ((header = reader.ReadInt()) != 0x7FFF7FFF)
                {
                    header ^= DoubleXor;

                    uint* cur = dataRef + ((((header >> 12) & 0x3FF) * delta) + ((header >> 22) & 0x3FF));
                    uint* end = cur + (header & 0xFFF);

                    int filecounter = 0;
                    byte[] filedata = reader.ReadBytes(header & 0xFFF);

                    while (cur < end)
                        *cur++ = palette[filedata[filecounter++]];

                    dataRead += header & 0xFFF;
                }

                Metrics.ReportDataRead(dataRead);
            }

            m_Center = new Microsoft.Xna.Framework.Point(xCenter, yCenter);

            m_Texture = new Texture2D(graphics, width, height);
            m_Texture.SetData<uint>(data);
        }

        public static unsafe byte[] Frame_ReadData(uint[] palette, BinaryReader bin, bool flip)
        {
            int dataStart = (int)bin.BaseStream.Position;

            int xCenter = bin.ReadInt16();
            int yCenter = bin.ReadInt16();

            int width = bin.ReadUInt16();
            int height = bin.ReadUInt16();

            // Fix for animations with no IO.
            if ((width == 0) || (height == 0))
            {
                return getData(bin, dataStart);
            }

            uint[] data = new uint[width * height];

            // ushort* line = (ushort*)bd.Scan0;
            // int delta = bd.Stride >> 1;

            int header;

            int xBase = xCenter - 0x200;
            int yBase = (yCenter + height) - 0x200;

            fixed (uint* pData = data)
            {
                uint* dataRef = pData;
                int delta = width;

                if (!flip)
                {
                    dataRef += xBase;
                    dataRef += (yBase * delta);

                    while ((header = bin.ReadInt32()) != 0x7FFF7FFF)
                    {
                        header ^= DoubleXor;

                        uint* cur = dataRef + ((((header >> 12) & 0x3FF) * delta) + ((header >> 22) & 0x3FF));
                        uint* end = cur + (header & 0xFFF);

                        int filecounter = 0;
                        byte[] filedata = bin.ReadBytes(header & 0xFFF);

                        while (cur < end)
                            *cur++ = palette[filedata[filecounter++]];
                    }
                }
                else
                {
                    dataRef -= xBase - width + 1;
                    dataRef += (yBase * delta);

                    while ((header = bin.ReadInt32()) != 0x7FFF7FFF)
                    {
                        header ^= DoubleXor;

                        uint* cur = dataRef + ((((header >> 12) & 0x3FF) * delta) - ((header >> 22) & 0x3FF));
                        uint* end = cur - (header & 0xFFF);

                        int filecounter = 0;
                        byte[] filedata = bin.ReadBytes(header & 0xFFF);

                        while (cur > end)
                            *cur-- = palette[filedata[filecounter++]];
                    }

                    xCenter = width - xCenter;
                }
            }

            return getData(bin, dataStart);
        }

        private static byte[] getData(BinaryReader bin, int start)
        {
            int length = (int)bin.BaseStream.Position - start;
            bin.BaseStream.Position = start;
            byte[] b = bin.ReadBytes(length);
            return b;
        }
    }
} 