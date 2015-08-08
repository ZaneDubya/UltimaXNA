#region usings
using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    /// <summary>
    /// Contains translation tables used for mapping body values to file subsets.
    /// <seealso cref="AnimationResource" />
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

        public static int DeathAnimationIndex(Body body)
        {
            switch (body.Type)
            {
                case BodyType.Human:
                    return 21;
                case BodyType.Monster:
                    return 2;
                case BodyType.Animal:
                    return 8;
                default:
                    return 2;
            }
        }

        public static int DeathAnimationFrameCount(Body body)
        {
            switch (body.Type)
            {
                case BodyType.Human:
                    return 6;
                case BodyType.Monster:
                    return 4;
                case BodyType.Animal:
                    return 4;
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
                Metrics.ReportDataRead(totalDataRead);
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
            if (body > 0x3E93)
            {
                for (int i = 0; i < m_MountIDConv.Length; ++i)
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

                    m_Entries[iParam1] = new BodyTableEntry(iParam2, iParam1, iParam3);
                }
                catch
                {
                }
            }
            Metrics.ReportDataRead(totalDataRead);
        }
    }
}
