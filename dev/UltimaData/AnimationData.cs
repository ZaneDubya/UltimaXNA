/***************************************************************************
 *   Animations.cs
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
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.UltimaData
{
    public sealed class AnimationData
    {
        private static FileIndex _FileIndex = new FileIndex("Anim.idx", "Anim.mul", 0x40000, 6);
        public static FileIndex FileIndex { get { return _FileIndex; } }

        private static FileIndex _FileIndex2 = new FileIndex("Anim2.idx", "Anim2.mul", 0x10000, -1);
        public static FileIndex FileIndex2 { get { return _FileIndex2; } }

        private static FileIndex _FileIndex3 = new FileIndex("Anim3.idx", "Anim3.mul", 0x20000, -1);
        public static FileIndex FileIndex3 { get { return _FileIndex3; } }

        private static FileIndex _FileIndex4 = new FileIndex("Anim4.idx", "Anim4.mul", 0x20000, -1);
        public static FileIndex FileIndex4 { get { return _FileIndex4; } }

        private static FileIndex _FileIndex5 = new FileIndex("Anim5.idx", "Anim5.mul", 0x20000, -1);
        public static FileIndex FileIndex5 { get { return _FileIndex5; } }

        private static AnimationFrame[][][][] _Cache;
        private static GraphicsDevice _graphics;
        private static int[] _Table;

        public static void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;
        }

        public static GraphicsDevice DEBUG_GFX { get { return _graphics; } }

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
            Stream stream;
            BinaryReader bin;

            //if (body >= 0x2000)
            //    return null;

            // The UO Server can request actions with an index greater
            // than the total number of actions. Check for this.
            action = LimitActionIndex(body, hue, action);

            getIndexes(ref body, ref hue, action, direction, out animIndex, out fileIndex);

            AnimationFrame[] f = checkCache(body, action, direction);
            if (f != null)
                return f;

            stream = fileIndex.Seek(animIndex, out length, out extra, out patched);

            if (stream == null)
            {
                return null;
            }
            else
            {
                bin = new BinaryReader(stream);
                AnimationFrame[] frames = GetAnimation(bin);
                return _Cache[body][action][direction] = frames;
            }
        }

        public static AnimationFrame[] GetAnimation(BinaryReader bin)
        {
            uint[] palette = getPalette(bin); // 0x100 * 2 = 0x0200 bytes
            int lookupStart = (int)bin.BaseStream.Position;
            int frameCount = bin.ReadInt32(); // 0x04 bytes

            int[] lookups = new int[frameCount]; // frameCount * 0x04 bytes
            for (int i = 0; i < frameCount; ++i) { lookups[i] = bin.ReadInt32(); }

            AnimationFrame[] frames = new AnimationFrame[frameCount];
            for (int i = 0; i < frameCount; ++i)
            {
                if (lookups[i] < lookups[0])
                {
                    frames[i] = AnimationFrame.Empty; // Fix for broken animations, per issue13
                }
                else
                {
                    bin.BaseStream.Seek(lookupStart + lookups[i], SeekOrigin.Begin);
                    frames[i] = new AnimationFrame(_graphics, palette, bin);
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
            Stream stream;
            BinaryReader bin;

            if (body >= 0x1000)
                return null;

            getIndexes(ref body, ref hue, action, direction, out animIndex, out fileIndex);
            stream = fileIndex.Seek(animIndex, out length, out extra, out patched);

            if (stream == null)
                return null;
            else
                bin = new BinaryReader(stream);

            return bin.ReadBytes(length);
        }

        private static uint[] getPalette(BinaryReader bin)
        {
            uint[] pal = new uint[0x100];
            for (int i = 0; i < 0x100; ++i)
            {
                uint color = bin.ReadUInt16();
                pal[i] = 0xff000000 + (
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
            if (_Cache == null) _Cache = new AnimationFrame[0x1000][][][];
            if (_Cache[body] == null)
                _Cache[body] = new AnimationFrame[35][][];
            if (_Cache[body][action] == null)
                _Cache[body][action] = new AnimationFrame[8][];
            if (_Cache[body][action][direction] == null)
                _Cache[body][action][direction] = new AnimationFrame[1];
            if (_Cache[body][action][direction][0] != null)
                return _Cache[body][action][direction];
            else
                return null;
        }

        private static void getIndexes(ref int body, ref int hue, int action, int direction, out int index, out FileIndex fileIndex)
        {
            Translate(ref body, ref hue);

            int fileType = UltimaData.BodyConverter.Convert(ref body);
            switch (fileType)
            {
                default:
                case 1:
                    {
                        fileIndex = _FileIndex;

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
                        fileIndex = _FileIndex2;

                        if (body < 200)
                            index = body * 110;
                        else
                            index = 22000 + ((body - 200) * 65);

                        break;
                    }
                case 3:
                    {
                        fileIndex = _FileIndex3;

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
                        fileIndex = _FileIndex4;

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
                        fileIndex = _FileIndex5;
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

        public static BodyTypes BodyType(int body)
        {
            return BodyType(body, -1);
        }

        public static BodyTypes BodyType(int body, int hue)
        {
            if (hue == -1)
                Translate(ref body);
            else
                Translate(ref body, ref hue);

            int fileType = BodyConverter.Convert(ref body);
            switch (fileType)
            {
                default:
                case 1:
                    {
                        if (body < 200)
                            return BodyTypes.HighDetail;
                        else if (body < 400)
                            return BodyTypes.LowDetail;
                        else
                            return BodyTypes.Humanoid;
                    }
                case 2:
                    {
                        if (body < 200)
                            return BodyTypes.HighDetail;
                        else
                            return BodyTypes.LowDetail;
                    }
                case 3:
                    {
                        if (body < 300)
                            return BodyTypes.LowDetail;
                        else if (body < 400)
                            return BodyTypes.HighDetail;
                        else
                            return BodyTypes.Humanoid;
                    }
                case 4:
                    {
                        if (body < 200)
                            return BodyTypes.HighDetail;
                        else if (body < 400)
                            return BodyTypes.LowDetail;
                        else
                            return BodyTypes.Humanoid;
                    }
                case 5:
                    {
                        if ((body < 200) && (body != 34))
                            return BodyTypes.HighDetail;
                        else if (body < 400)
                            return BodyTypes.LowDetail;
                        else
                            return BodyTypes.Humanoid;
                    }
            }
        }

        public static int LimitActionIndex(int bodyID, int hue, int action)
        {
            switch (BodyType(bodyID, hue))
            {
                case BodyTypes.HighDetail:
                    return action % 0x16;
                case BodyTypes.LowDetail:
                    return action % 0x13;
                case BodyTypes.Humanoid:
                    return action % 0x23;
                default:
                    return -1;
            }
        }

        public static void Translate(ref int body)
        {
            if (_Table == null)
                LoadTable();

            if (body <= 0 || body >= _Table.Length)
            {
                body = 0;
                return;
            }

            body = (_Table[body] & 0x7FFF);
        }

        public static void Translate(ref int body, ref int hue)
        {
            if (_Table == null)
                LoadTable();

            if (body <= 0 || body >= _Table.Length)
            {
                body = 0;
                return;
            }

            int table = _Table[body];

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
            int count = 400 + ((_FileIndex.Index.Length - 35000) / 175);

            _Table = new int[count];

            for (int i = 0; i < count; ++i)
            {
                object o = BodyTable._Entries[i];

                if (o == null || BodyConverter.Contains(i))
                {
                    _Table[i] = i;
                }
                else
                {
                    BodyTableEntry bte = (BodyTableEntry)o;

                    _Table[i] = bte._OldID | (1 << 31) | (((bte._NewHue ^ 0x8000) & 0xFFFF) << 15);
                }
            }
        }
    }

    public sealed class AnimationFrame
    {
        private Microsoft.Xna.Framework.Point _Center;
        private Microsoft.Xna.Framework.Graphics.Texture2D _Texture;

        public Microsoft.Xna.Framework.Point Center { get { return _Center; } }
        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get { return _Texture; } }

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

            _Center = new Microsoft.Xna.Framework.Point(xCenter, yCenter);
            _Texture = new Texture2D(graphics, width, height);
            _Texture.SetData<uint>(data);
            palette[0] = 0;
        }

        public unsafe AnimationFrame(GraphicsDevice graphics, uint[] palette, BinaryReader bin)
        {
            int xCenter = bin.ReadInt16();
            int yCenter = bin.ReadInt16();

            int width = bin.ReadUInt16();
            int height = bin.ReadUInt16();

            // Fix for animations with no UltimaData.
            if ((width == 0) || (height == 0))
            {
                _Texture = null;
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

                while ((header = bin.ReadInt32()) != 0x7FFF7FFF)
                {
                    header ^= DoubleXor;

                    uint* cur = dataRef + ((((header >> 12) & 0x3FF) * delta) + ((header >> 22) & 0x3FF));
                    uint* end = cur + (header & 0xFFF);

                    int filecounter = 0;
                    byte[] filedata = bin.ReadBytes(header & 0xFFF);

                    while (cur < end)
                        *cur++ = palette[filedata[filecounter++]];

                    dataRead += header & 0xFFF;
                }

                UltimaVars.Metrics.ReportDataRead(dataRead);
            }

            _Center = new Microsoft.Xna.Framework.Point(xCenter, yCenter);

            _Texture = new Texture2D(graphics, width, height);
            _Texture.SetData<uint>(data);
        }

        public static unsafe byte[] Frame_ReadData(uint[] palette, BinaryReader bin, bool flip)
        {
            int dataStart = (int)bin.BaseStream.Position;

            int xCenter = bin.ReadInt16();
            int yCenter = bin.ReadInt16();

            int width = bin.ReadUInt16();
            int height = bin.ReadUInt16();

            // Fix for animations with no UltimaData.
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

    public enum BodyTypes
    {
        Null = -1,
        HighDetail = 0,
        LowDetail = 1,
        Humanoid = 2
    }
} 