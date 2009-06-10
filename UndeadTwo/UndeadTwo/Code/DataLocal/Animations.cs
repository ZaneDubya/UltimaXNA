#region File Description & Usings
//-----------------------------------------------------------------------------
// Animations.cs
//
// Based on UltimaSDK, modifications by Poplicola & ClintXNA
//-----------------------------------------------------------------------------
using System;
using System.Collections;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UndeadClient.DataLocal
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

        private BodyConverter() { }

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

                    if (split.Length >= 3 || !int.TryParse(split[2], out anim3))
                    {
                        anim3 = -1;
                    }

                    if (split.Length >= 4 || !int.TryParse(split[3], out anim4))
                    {
                        anim4 = -1;
                    }

                    if (split.Length >= 5 || !int.TryParse(split[4], out anim5))
                    {
                        anim5 = -1;
                    }

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

        private static FrameXNA[][][][][] m_Cache;

        private AnimationsXNA()
        {
        }

        public static FrameXNA[] GetAnimation(GraphicsDevice nDevice, int body, int action, int direction, int hue, bool preserveHue)
        {
            int iCacheHue = 0; //hue + 1; // since hue is -1 if we don't want to hue it.
            if (m_Cache == null)
            {
                // max number of bodies is about 1000
                m_Cache = new FrameXNA[1000][][][][];
            }

            if (m_Cache[body] == null)
                m_Cache[body] = new FrameXNA[35][][][];

            if (m_Cache[body][action] == null)
                m_Cache[body][action] = new FrameXNA[8][][];

            if (m_Cache[body][action][direction] == null)
                m_Cache[body][action][direction] = new FrameXNA[3000][];

            if (m_Cache[body][action][direction][iCacheHue] != null)
            {
                return m_Cache[body][action][direction][iCacheHue];
            }

            if (preserveHue)
                Translate(ref body);
            else
                Translate(ref body, ref hue);

            int fileType = BodyConverter.Convert(ref body);
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
                    bin.BaseStream.Seek(lookups[i], SeekOrigin.Begin);
                    frames[i] = new FrameXNA(nDevice, palette, bin, flip);
                }
                m_Cache[body][action][direction][0] = frames;
            }

            /*
            // Now load the hue animation.
            if (hueObject != null)
            {
                FrameXNA[] frames = new FrameXNA[frameCount];
                m_Cache[body][action][direction][0].CopyTo(frames, 0);
                for (int i = 0; i < frameCount; ++i)
                {
                    hueObject.ApplyTo(frames[i].Bitmap, onlyHueGrayPixels);
                }
                m_Cache[body][action][direction][iCacheHue] = frames;
            }
             */

            /*
            for (int i = 0; i < frameCount; ++i)
            {
                bin.BaseStream.Seek(lookups[i], SeekOrigin.Begin);
                frames[i] = new FrameXNA(nDevice, palette, bin, flip);

                if (hueObject != null)
                    hueObject.ApplyTo(frames[i].Bitmap, onlyHueGrayPixels);
            }
            */

            //return m_Cache[body][action][direction][iCacheHue];
            return m_Cache[body][action][direction][iCacheHue];
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

            // Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            // System.Drawing.Imaging.BitmapData bd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

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