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
        private static int[] m_Table1 = new int[0];
        private static int[] m_Table2 = new int[0];
        private static int[] m_Table3 = new int[0];
        private static int[] m_Table4 = new int[0];

		// Issue 6 - Missing mounted animations - http://code.google.com/p/ultimaxna/issues/detail?id=6 - Smjert
		// MountItemID , BodyID
		private static int[][] m_MountIDConv = new int[][]
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
            switch (Data.Mobtypes.AnimationType(bodyID))
            {
                case 0:
                    return 2;
                case 1:
                    return 8;
                case 2:
                    return 21;
                default:
                    return 2;
            }
        }

        public static int DeathAnimationFrameCount(int bodyID)
        {
            switch (Data.Mobtypes.AnimationType(bodyID))
            {
                case 0:
                    return 4;
                case 1:
                    return 4;
                case 2:
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

                while ((line = ip.ReadLine()) != null)
                {
                    if ((line = line.Trim()).Length == 0 || line.StartsWith("#") || line.StartsWith("\"#"))
                        continue;

                    string[] split = line.Split('\t');

                    int original = System.Convert.ToInt32(split[0]);
                    int anim2 = System.Convert.ToInt32(split[1]);
					int anim3;
					int anim4;
					int anim5;

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
            }

            m_Table1 = new int[max1 + 1];
            m_Table2 = new int[max2 + 1];
            m_Table3 = new int[max3 + 1];
            m_Table4 = new int[max4 + 1];

            for (int i = 0; i < m_Table1.Length; ++i)
                m_Table1[i] = -1;

            for (int i = 0; i < list1.Count; i += 2)
                m_Table1[(int)list1[i]] = (int)list1[i + 1];

            for (int i = 0; i < m_Table2.Length; ++i)
                m_Table2[i] = -1;

            for (int i = 0; i < list2.Count; i += 2)
                m_Table2[(int)list2[i]] = (int)list2[i + 1];

            for (int i = 0; i < m_Table3.Length; ++i)
                m_Table3[i] = -1;

            for (int i = 0; i < list3.Count; i += 2)
                m_Table3[(int)list3[i]] = (int)list3[i + 1];

            for (int i = 0; i < m_Table4.Length; ++i)
                m_Table4[i] = -1;

            for (int i = 0; i < list4.Count; i += 2)
                m_Table4[(int)list4[i]] = (int)list4[i + 1];
        }

        /// <summary>
        /// Checks to see if <paramref name="body" /> is contained within the mapping table.
        /// </summary>
        /// <returns>True if it is, false if not.</returns>
        public static bool Contains(int body)
        {
            if (m_Table1 != null && body >= 0 && body < m_Table1.Length && m_Table1[body] != -1)
                return true;

            if (m_Table2 != null && body >= 0 && body < m_Table2.Length && m_Table2[body] != -1)
                return true;

            if (m_Table3 != null && body >= 0 && body < m_Table3.Length && m_Table3[body] != -1)
                return true;

            if (m_Table4 != null && body >= 0 && body < m_Table4.Length && m_Table4[body] != -1)
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
				for(int i = 0; i < m_MountIDConv.Length; ++i)
				{
					int[] conv = m_MountIDConv[i];
					if (conv[0] == body)
					{
						body = conv[1];
						break;
					}
				}
			}
			// Issue 6 - End
            if (m_Table1 != null && body >= 0 && body < m_Table1.Length)
            {
                int val = m_Table1[body];

                if (val != -1)
                {
                    body = val;
                    return 2;
                }
            }

            if (m_Table2 != null && body >= 0 && body < m_Table2.Length)
            {
                int val = m_Table2[body];

                if (val != -1)
                {
                    body = val;
                    return 3;
                }
            }

            if (m_Table3 != null && body >= 0 && body < m_Table3.Length)
            {
                int val = m_Table3[body];

                if (val != -1)
                {
                    body = val;
                    return 4;
                }
            }

            if (m_Table4 != null && body >= 0 && body < m_Table4.Length)
            {
                int val = m_Table4[body];

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
        public int m_OldID;
        public int m_NewID;
        public int m_NewHue;

        public BodyTableEntry(int oldID, int newID, int newHue)
        {
            m_OldID = oldID;
            m_NewID = newID;
            m_NewHue = newHue;
        }
    }

    public sealed class BodyTable
    {
        public static Hashtable m_Entries;

        static BodyTable()
        {
            m_Entries = new Hashtable();

            string filePath = FileManager.GetFilePath("body.def");

            if (filePath == null)
                return;

            StreamReader def = new StreamReader(filePath);

            string line;

            while ((line = def.ReadLine()) != null)
            {
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

                    m_Entries[iParam1] = new BodyTableEntry(iParam2, iParam1, iParam3);
                }
                catch
                {
                }
            }
        }
    }

    public sealed class AnimationsXNA
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

        private static FrameXNA[][][][] m_Cache;

        private AnimationsXNA()
        {
        }

        public static FrameXNA[] GetAnimation(GraphicsDevice nDevice, int body, int action, int direction, int hue, bool preserveHue)
        {
            // I moved this line here since at line 497, previously, it uses m_Cache with real body as index and that index has no instance, AnimID has instance instead.
            // Example with Hiryu (AnimID 243, real body has 201 after convert):
            // Prev: m_Cache[AnimID] = new instance, convert AnimID to real body, if(m_Cache[realbody]..etc) <-Crash
            // Now: convert AnimID to real body, m_Cache[realbody] = new instance, if(m_Cache[realbody]..etc) <- OK
            // - Smjert
            int fileType = BodyConverter.Convert(ref body);

            if (preserveHue)
                Translate(ref body);
            else
                Translate(ref body, ref hue);

            // Make sure the cache is complete.
            // max number of bodies is about 1000
            try
            {
                if (m_Cache == null) m_Cache = new FrameXNA[0x1000][][][];
                if (m_Cache[body] == null)
                    m_Cache[body] = new FrameXNA[35][][];
                if (m_Cache[body][action] == null)
                    m_Cache[body][action] = new FrameXNA[8][];
                if (m_Cache[body][action][direction] == null)
                    m_Cache[body][action][direction] = new FrameXNA[1];
                if (m_Cache[body][action][direction][0] != null)
                    return m_Cache[body][action][direction];
            }
            catch
            {
                return null;
            }

            FileIndex fileIndex;

            int index;

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
                case 5:
                    {
                        fileIndex = m_FileIndex5;

                        if (body < 200 && body != 34) // looks strange, though it works.
                            index = body * 110;
                        else
                            index = 35000 + ((body - 400) * 65);

                        break;
                    }
            }

            if ((index + (action * 5)) > int.MaxValue)
            {
                throw new ArithmeticException();
            }

            index += action * 5;

            if (direction <= 4)
                index += direction;
            else
                index += direction - (direction - 4) * 2;

            int length, extra;
            bool patched;
            Stream stream = fileIndex.Seek(index, out length, out extra, out patched);

            if (stream == null)
            {
                return null;
            }

            bool flip = (direction > 4);

            BinaryReader bin = new BinaryReader(stream);

            ushort[] palette = new ushort[0x100];

            for (int i = 0; i < 0x100; ++i)
                palette[i] = (ushort)(bin.ReadUInt16() ^ 0x8000);

            int start = (int)bin.BaseStream.Position;
            int frameCount = bin.ReadInt32();

            int[] lookups = new int[frameCount];

            for (int i = 0; i < frameCount; ++i)
                lookups[i] = start + bin.ReadInt32();

            bool onlyHueGrayPixels = ((hue & 0x8000) == 0);

            hue = (hue & 0x3FFF) - 1;

            Hue hueObject = null;

            if (hue >= 0 && hue < Hues.List.Length)
                hueObject = Hues.List[hue];

            // Load the base animation, unhued.
            if (m_Cache[body][action][direction][0] == null)
            {
                FrameXNA[] frames = new FrameXNA[frameCount];
                for (int i = 0; i < frameCount; ++i)
                {
                    // Fix for broken cleaver animation, perhaps others, per issue13 (http://code.google.com/p/ultimaxna/issues/detail?id=13) --ZDW 6/15/2009
                    if (lookups[i] < lookups[0])
                    {
                        frames[i] = FrameXNA.Empty;
                    }
                    else
                    {
                        bin.BaseStream.Seek(lookups[i], SeekOrigin.Begin);
                        frames[i] = new FrameXNA(nDevice, palette, bin, flip);
                    }
                }
                m_Cache[body][action][direction] = frames;
            }
            return m_Cache[body][action][direction];
        }

        private static int[] m_Table;

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

                if (vhue < 0 || vhue >= Hues.List.Length)
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

    public sealed class FrameXNA
    {
        private Microsoft.Xna.Framework.Point m_Center;
        private Microsoft.Xna.Framework.Graphics.Texture2D m_Texture;

        public Microsoft.Xna.Framework.Point Center { get { return m_Center; } }
        public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get { return m_Texture; } }

        private const int DoubleXor = (0x200 << 22) | (0x200 << 12);

        public static readonly FrameXNA Empty = new FrameXNA();
        public static readonly FrameXNA[] EmptyFrames = new FrameXNA[1] { Empty };

        private FrameXNA()
        {
            // m_Texture = new Texture2D(nDevice, 1, 1);
        }

        public unsafe FrameXNA(GraphicsDevice nDevice, ushort[] palette, BinaryReader bin, bool flip)
        {
            int xCenter = bin.ReadInt16();
            int yCenter = bin.ReadInt16();

            int width = bin.ReadUInt16();
            int height = bin.ReadUInt16();

            // Fix for animations with no data.
            if ((width == 0) || (height == 0))
            {
                m_Texture = null;
                return;
            }

            ushort[] data = new ushort[width * height];

            // ushort* line = (ushort*)bd.Scan0;
            // int delta = bd.Stride >> 1;

            int header;

            int xBase = xCenter - 0x200;
            int yBase = (yCenter + height) - 0x200;

            fixed (ushort* pData = data)
            {
                ushort* dataRef = pData;
                int delta = width;

                if (!flip)
                {
                    dataRef += xBase;
                    dataRef += (yBase * delta);

                    while ((header = bin.ReadInt32()) != 0x7FFF7FFF)
                    {
                        header ^= DoubleXor;

                        ushort* cur = dataRef + ((((header >> 12) & 0x3FF) * delta) + ((header >> 22) & 0x3FF));
                        ushort* end = cur + (header & 0xFFF);

                        while (cur < end)
                            *cur++ = palette[bin.ReadByte()];
                    }
                }
                else
                {
                    dataRef -= xBase - width + 1;
                    dataRef += (yBase * delta);

                    while ((header = bin.ReadInt32()) != 0x7FFF7FFF)
                    {
                        header ^= DoubleXor;

                        ushort* cur = dataRef + ((((header >> 12) & 0x3FF) * delta) - ((header >> 22) & 0x3FF));
                        ushort* end = cur - (header & 0xFFF);

                        while (cur > end)
                            *cur-- = palette[bin.ReadByte()];
                    }

                    xCenter = width - xCenter;
                }
            }

            m_Center = new Microsoft.Xna.Framework.Point(xCenter, yCenter);

            m_Texture = new Texture2D(nDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Bgra5551);
            m_Texture.SetData<ushort>(data);
        }
    }
} 