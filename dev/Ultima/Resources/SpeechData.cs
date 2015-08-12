/***************************************************************************
 *   Speech.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.Resources
{
    class SpeechEntry
    {
        private int m_Index;
        List<string> m_Strings;
        List<Regex> m_Regex;

        public int Index { get { return m_Index; } }
        public List<string> Strings { get { return m_Strings; } }
        public List<Regex> Regex { get { return m_Regex; } }

        public SpeechEntry(int index)
        {
            m_Index = index;
            m_Strings = new List<string>();
            m_Regex = new List<Regex>();
        }
    }

    class SpeechEntrySorter : IComparer<SpeechEntry>
    {
        public int Compare(SpeechEntry x, SpeechEntry y)
        {
            return x.Index.CompareTo(y.Index);
        }
    }

    class SpeechData
    {
        static List<Dictionary<int, SpeechEntry>> table;

        public static void GetSpeechTriggers(string text, string lang, out int count, out int[] triggers)
        {
            if (table == null)
                table = LoadSpeechFile();
            
            List<int> t = new List<int>();
            int speechTable = 0; // "ENU/0"

            foreach (KeyValuePair<int, SpeechEntry> e in table[speechTable])
            {
                for (int i = 0; i < e.Value.Regex.Count; i++)
                {
                    if (e.Value.Regex[i].IsMatch(text))
                    {
                        if (!t.Contains(e.Key))
                            t.Add(e.Key);
                    }
                }
            }

            count = t.Count;
            triggers = t.ToArray();
        }

        static List<Dictionary<int, SpeechEntry>> LoadSpeechFile()
        {
            List<Dictionary<int, SpeechEntry>> tables = new List<Dictionary<int, SpeechEntry>>();
            int lastIndex = -1;

            Dictionary<int, SpeechEntry> table = null;

            string path = FileManager.GetFilePath("speech.mul");

            using (BinaryReader bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                while (bin.PeekChar() >= 0)
                {
                    int index = (bin.ReadByte() << 8) | bin.ReadByte();
                    int length = (bin.ReadByte() << 8) | bin.ReadByte();
                    string text = Encoding.UTF8.GetString(bin.ReadBytes(length)).Trim();

                    if (text.Length == 0)
                        continue;

                    if (table == null || lastIndex > index)
                    {
                        if (index == 0 && text == "*withdraw*")
                            tables.Insert(0, table = new Dictionary<int, SpeechEntry>());
                        else
                            tables.Add(table = new Dictionary<int, SpeechEntry>());
                    }

                    lastIndex = index;

                    SpeechEntry entry = null;
                    table.TryGetValue(index, out entry);

                    if (entry == null)
                        table[index] = entry = new SpeechEntry(index);

                    entry.Strings.Add(text);
                    entry.Regex.Add(new Regex(text.Replace("*", @".*"), RegexOptions.IgnoreCase));
                }

                return tables;
            }
        }
    }
}
