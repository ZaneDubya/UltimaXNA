/***************************************************************************
 *   Animations.cs
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
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.Data
{
    /// <summary>
    /// Contains translation tables used for mapping body values to file subsets.
    /// <seealso cref="Animations" />
    /// </summary>
    public sealed class BodyConverter
    {
        private static int[] _Table1 = new int[0];
        private static int[] _Table2 = new int[0];
        private static int[] _Table3 = new int[0];
        private static int[] _Table4 = new int[0];

		// Issue 6 - Missing mounted animations - http://code.google.com/p/ultimaxna/issues/detail?id=6 - Smjert
		// MountItemID , BodyID
		private static int[][] _MountIDConv = new int[][]
		{
			new int[]{0x3E94, 0xF3}, // Hiryu
			new int[]{0x3E97, 0xC3}, // Beetle
			new int[]{0x3E98, 0xC2}, // Swamp Dragon
			new int[]{0x3E9A, 0xC1}, // Ridgeback
			new int[]{0x3E9B, 0xC0}, // Unicorn
			new int[]{0x3E9C, 0xBF}, // Ki-Rin
			new int[]{0x3E9E, 0xBE}, // Fire Steed
			new int[]{0x3E9F, 0xC8}, // Horse
			new int[]{0x3EA0, 0xE2}, // Grey Horse
			new int[]{0x3EA1, 0xE4}, // Horse
			new int[]{0x3EA2, 0xCC}, // Brown Horse
			new int[]{0x3EA3, 0xD2}, // Zostrich
			new int[]{0x3EA4, 0xDA}, // Zostrich
			new int[]{0x3EA5, 0xDB}, // Zostrich
			new int[]{0x3EA6, 0xDC}, // Llama
			new int[]{0x3EA7, 0x74}, // Nightmare
			new int[]{0x3EA8, 0x75}, // Silver Steed
			new int[]{0x3EA9, 0x72}, // Nightmare
			new int[]{0x3EAA, 0x73}, // Ethereal Horse
			new int[]{0x3EAB, 0xAA}, // Ethereal Llama
			new int[]{0x3EAC, 0xAB}, // Ethereal Zostrich
			new int[]{0x3EAD, 0x84}, // Ki-Rin
			new int[]{0x3EAF, 0x78}, // Minax Warhorse
			new int[]{0x3EB0, 0x79}, // ShadowLords Warhorse
			new int[]{0x3EB1, 0x77}, // COM Warhorse
			new int[]{0x3EB2, 0x76}, // TrueBritannian Warhorse
			new int[]{0x3EB3, 0x90}, // Seahorse
			new int[]{0x3EB4, 0x7A}, // Unicorn
			new int[]{0x3EB5, 0xB1}, // Nightmare
			new int[]{0x3EB6, 0xB2}, // Nightmare
			new int[]{0x3EB7, 0xB3}, // Dark Nightmare
			new int[]{0x3EB8, 0xBC}, // Ridgeback
			new int[]{0x3EBA, 0xBB}, // Ridgeback
			new int[]{0x3EBB, 0x319}, // Undead Horse
			new int[]{0x3EBC, 0x317}, // Beetle
			new int[]{0x3EBD, 0x31A}, // Swamp Dragon
			new int[]{0x3EBE, 0x31F}, // Armored Swamp Dragon
			new int[]{0x3F6F, 0x9} // Daemon
		};
		// Issue 6 - End

        private BodyConverter() { }

        public static int DeathAnimationIndex(int bodyID)
        {
            switch (AnimationsXNA.BodyType(bodyID))
            {
                case BodyTypes.HighDetail:
                    return 2;
                case BodyTypes.LowDetail:
                    return 8;
                case BodyTypes.Humanoid:
                    return 21;
                default:
                    return 2;
            }
        }

        public static int DeathAnimationFrameCount(int bodyID)
        {
            switch (AnimationsXNA.BodyType(bodyID))
            {
                case BodyTypes.HighDetail:
                    return 4;
                case BodyTypes.LowDetail:
                    return 4;
                case BodyTypes.Humanoid:
                    return 6;
                default:
                    return 4;
            }
        }

        static BodyConverter()
        {
            string path = FileManager.GetFilePath("bodyconv.def");

            if (path == null)
                return;

            ArrayList list1 = new ArrayList(), list2 = new ArrayList(), list3 = new ArrayList(), list4 = new ArrayList();
            int max1 = 0, max2 = 0, max3 = 0, max4 = 0;

            using (StreamReader ip = new StreamReader(path))
            {
                string line;
                int totalDataRead = 0;

                while ((line = ip.ReadLine()) != null)
                {
                    totalDataRead += line.Length;

                    if ((line = line.Trim()).Length == 0 || line.StartsWith("#") || line.StartsWith("\"#"))
                        continue;

                    // string[] split = line.Split('\t');
                    string[] split = Regex.Split(line, @"\t|\s+", RegexOptions.IgnoreCase);
                    int original = System.Convert.ToInt32(split[0]);
                    int anim2 = System.Convert.ToInt32(split[1]);
					int anim3;
					int anim4;
					int anim5;
                    // Int32.TryParse(split[0], out original);
                    // Int32.TryParse(split[1], out anim2);

                    // The control here was wrong, previously it was always putting -1 without parsing the file - Smjert
                    if (split.Length < 3 || !int.TryParse(split[2], out anim3))
                    {
                        anim3 = -1;
                    }

                    if (split.Length < 4 || !int.TryParse(split[3], out anim4))
                    {
                        anim4 = -1;
                    }

                    if (split.Length < 5 || !int.TryParse(split[4], out anim5))
                    {
                        anim5 = -1;
                    }
					// End Mod - Smjert

                    if (anim2 != -1)
                    {
                        if (anim2 == 68)
                            anim2 = 122;

                        if (original > max1)
                            max1 = original;

                        list1.Add(original);
                        list1.Add(anim2);
                    }

                    if (anim3 != -1)
                    {
                        if (original > max2)
                            max2 = original;

                        list2.Add(original);
                        list2.Add(anim3);
                    }

                    if (anim4 != -1)
                    {
                        if (original > max3)
                            max3 = original;

                        list3.Add(original);
                        list3.Add(anim4);
                    }

                    if (anim5 != -1)
                    {
                        if (original > max4)
                            max4 = original;

                        list4.Add(original);
                        list4.Add(anim5);
                    }
                }
                ClientVars.Metrics.ReportDataRead(totalDataRead);
            }

            _Table1 = new int[max1 + 1];
            _Table2 = new int[max2 + 1];
            _Table3 = new int[max3 + 1];
            _Table4 = new int[max4 + 1];

            for (int i = 0; i < _Table1.Length; ++i)
                _Table1[i] = -1;

            for (int i = 0; i < list1.Count; i += 2)
                _Table1[(int)list1[i]] = (int)list1[i + 1];

            for (int i = 0; i < _Table2.Length; ++i)
                _Table2[i] = -1;

            for (int i = 0; i < list2.Count; i += 2)
                _Table2[(int)list2[i]] = (int)list2[i + 1];

            for (int i = 0; i < _Table3.Length; ++i)
                _Table3[i] = -1;

            for (int i = 0; i < list3.Count; i += 2)
                _Table3[(int)list3[i]] = (int)list3[i + 1];

            for (int i = 0; i < _Table4.Length; ++i)
                _Table4[i] = -1;

            for (int i = 0; i < list4.Count; i += 2)
                _Table4[(int)list4[i]] = (int)list4[i + 1];
        }

        /// <summary>
        /// Checks to see if <paramref name="body" /> is contained within the mapping table.
        /// </summary>
        /// <returns>True if it is, false if not.</returns>
        public static bool Contains(int body)
        {
            if (_Table1 != null && body >= 0 && body < _Table1.Length && _Table1[body] != -1)
                return true;

            if (_Table2 != null && body >= 0 && body < _Table2.Length && _Table2[body] != -1)
                return true;

            if (_Table3 != null && body >= 0 && body < _Table3.Length && _Table3[body] != -1)
                return true;

            if (_Table4 != null && body >= 0 && body < _Table4.Length && _Table4[body] != -1)
                return true;

            return false;
        }

        /// <summary>
        /// Attempts to convert <paramref name="body" /> to a body index relative to a file subset, specified by the return value.
        /// </summary>
        /// <returns>A value indicating a file subset:
        /// <list type="table">
        /// <listheader>
        /// <term>Return Value</term>
        /// <description>File Subset</description>
        /// </listheader>
        /// <item>
        /// <term>1</term>
        /// <description>Anim.mul, Anim.idx (Standard)</description>
        /// </item>
        /// <item>
        /// <term>2</term>
        /// <description>Anim2.mul, Anim2.idx (LBR)</description>
        /// </item>
        /// <item>
        /// <term>3</term>
        /// <description>Anim3.mul, Anim3.idx (AOS)</description>
        /// </item>
        /// </list>
        /// </returns>
        public static int Convert(ref int body)
        {
			// Issue 6 - Missing mounted animations - http://code.google.com/p/ultimaxna/issues/detail?id=6 - Smjert
			// Converts MountItemID to BodyID
			if ( body > 0x3E93 )
			{
				for(int i = 0; i < _MountIDConv.Length; ++i)
				{
					int[] conv = _MountIDConv[i];
					if (conv[0] == body)
					{
						body = conv[1];
						break;
					}
				}
			}
			// Issue 6 - End
            if (_Table1 != null && body >= 0 && body < _Table1.Length)
            {
                int val = _Table1[body];

                if (val != -1)
                {
                    body = val;
                    return 2;
                }
            }

            if (_Table2 != null && body >= 0 && body < _Table2.Length)
            {
                int val = _Table2[body];

                if (val != -1)
                {
                    body = val;
                    return 3;
                }
            }

            if (_Table3 != null && body >= 0 && body < _Table3.Length)
            {
                int val = _Table3[body];

                if (val != -1)
                {
                    body = val;
                    return 4;
                }
            }

            if (_Table4 != null && body >= 0 && body < _Table4.Length)
            {
                int val = _Table4[body];

                if (val != -1)
                {
                    body = val;
                    return 5;
                }
            }

            return 1;
        }
    }

    public sealed class BodyTableEntry
    {
        public int _OldID;
        public int _NewID;
        public int _NewHue;

        public BodyTableEntry(int oldID, int newID, int newHue)
        {
            _OldID = oldID;
            _NewID = newID;
            _NewHue = newHue;
        }
    }

    public sealed class BodyTable
    {
        public static Hashtable _Entries;

        static BodyTable()
        {
            _Entries = new Hashtable();

            string filePath = FileManager.GetFilePath("body.def");

            if (filePath == null)
                return;

            StreamReader def = new StreamReader(filePath);

            string line;
            int totalDataRead = 0;

            while ((line = def.ReadLine()) != null)
            {
                totalDataRead += line.Length;

                if ((line = line.Trim()).Length == 0 || line.StartsWith("#"))
                    continue;

                try
                {
                    int index1 = line.IndexOf(" {");
                    int index2 = line.IndexOf("} ");

                    string param1 = line.Substring(0, index1);
                    string param2 = line.Substring(index1 + 2, index2 - index1 - 2);
                    string param3 = line.Substring(index2 + 2);

                    int indexOf = param2.IndexOf(',');

                    if (indexOf > -1)
                        param2 = param2.Substring(0, indexOf).Trim();

                    int iParam1 = Convert.ToInt32(param1);
                    int iParam2 = Convert.ToInt32(param2);
                    int iParam3 = Convert.ToInt32(param3);

                    _Entries[iParam1] = new BodyTableEntry(iParam2, iParam1, iParam3);
                }
                catch
                {
                }
            }
            ClientVars.Metrics.ReportDataRead(totalDataRead);
        }
    }

    public sealed class AnimationsXNA
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

        private static FrameXNA[][][][] _Cache;
        private static GraphicsDevice _graphics;
        private static int[] _Table;

        public static void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;
        }

        public static GraphicsDevice DEBUG_GFX { get { return _graphics; } }

        public static int GetAnimationFrameCount(int body, int action, int direction, int hue)
        {
            FrameXNA[] frames = GetAnimation(body, action, direction, hue);
            if (frames == null)
                return 0;
            return frames.Length;
        }

        public static FrameXNA[] GetAnimation(int body, int action, int direction, int hue)
        {
            int animIndex;
            FileIndex fileIndex;
            int length, extra;
            bool patched;
            Stream stream;
            BinaryReader bin;

            if (body >= 0x1000)
                return null;

            // The UO Server can request actions with an index greater
            // than the total number of actions. Check for this.
            action = LimitActionIndex(body, hue, action);

            getIndexes(ref body, ref hue, action, direction, out animIndex, out fileIndex);

            FrameXNA[] f = checkCache(body, action, direction);
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
                FrameXNA[] frames = GetAnimation(bin);
                return _Cache[body][action][direction] = frames;
            }
        }

        public static FrameXNA[] GetAnimation(BinaryReader bin)
        {
            uint[] palette = getPalette(bin); // 0x100 * 2 = 0x0200 bytes
            int lookupStart = (int)bin.BaseStream.Position;
            int frameCount = bin.ReadInt32(); // 0x04 bytes

            int[] lookups = new int[frameCount]; // frameCount * 0x04 bytes
            for (int i = 0; i < frameCount; ++i) { lookups[i] = bin.ReadInt32(); }

            FrameXNA[] frames = new FrameXNA[frameCount];
            for (int i = 0; i < frameCount; ++i)
            {
                if (lookups[i] < lookups[0])
                {
                    frames[i] = FrameXNA.Empty; // Fix for broken animations, per issue13
                }
                else
                {
                    bin.BaseStream.Seek(lookupStart + lookups[i], SeekOrigin.Begin);
                    frames[i] = new FrameXNA(_graphics, palette, bin);
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

        private static FrameXNA[] checkCache(int body, int action, int direction)
        {
            // Make sure the cache is complete.
            // max number of bodies is about 0x1000
            if (_Cache == null) _Cache = new FrameXNA[0x1000][][][];
            if (_Cache[body] == null)
                _Cache[body] = new FrameXNA[35][][];
            if (_Cache[body][action] == null)
                _Cache[body][action] = new FrameXNA[8][];
            if (_Cache[body][action][direction] == null)
                _Cache[body][action][direction] = new FrameXNA[1];
            if (_Cache[body][action][direction][0] != null)
                return _Cache[body][action][direction];
            else
                return null;
        }

        private static void getIndexes(ref int body, ref int hue, int action, int direction, out int index, out FileIndex fileIndex)
        {
            Translate(ref body, ref hue);

            int fileType = BodyConverter.Convert(ref body);
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

                if (vhue < 0 || vhue >= Hues.List.Length)
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

    public sealed class FrameXNA
    {
        private Microsoft.Xna.Framework.Point _Center;
        private Microsoft.Xna.Framework.Graphics.Texture2D _Texture;

        public Microsoft.Xna.Framework.Point Center { get { return _Center; } }
        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get { return _Texture; } }

        private const int DoubleXor = (0x200 << 22) | (0x200 << 12);

        public static readonly FrameXNA Empty = new FrameXNA();
        public static readonly FrameXNA[] EmptyFrames = new FrameXNA[1] { Empty };

        private FrameXNA()
        {
        }

        public unsafe FrameXNA(GraphicsDevice graphics, uint[] palette, byte[] frame, int width, int height, int xCenter, int yCenter)
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

        public unsafe FrameXNA(GraphicsDevice graphics, uint[] palette, BinaryReader bin)
        {
            int xCenter = bin.ReadInt16();
            int yCenter = bin.ReadInt16();

            int width = bin.ReadUInt16();
            int height = bin.ReadUInt16();

            // Fix for animations with no data.
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

                ClientVars.Metrics.ReportDataRead(dataRead);
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

            // Fix for animations with no data.
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