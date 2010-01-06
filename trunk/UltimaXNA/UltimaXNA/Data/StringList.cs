﻿/***************************************************************************
 *   StringList.cs
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
using System.IO;
using System.Text;
using System.Collections;
#endregion

namespace UltimaXNA.Data
{
    public static class StringList
    {
        private static Hashtable m_Table;
        private static StringEntry[] m_Entries;
        private static string m_Language;

        public static StringEntry[] Entries { get { return m_Entries; } }
        public static Hashtable Table { get { return m_Table; } }
        public static string Language { get { return m_Language; } }

        private static byte[] m_Buffer = new byte[1024];

        public static void LoadStringList(string language)
        {
            m_Language = language;
            m_Table = new Hashtable();

            string path = FileManager.GetFilePath(String.Format("cliloc.{0}", language));

            if (path == null)
            {
                m_Entries = new StringEntry[0];
                return;
            }

            ArrayList list = new ArrayList();

            using (BinaryReader bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                bin.ReadInt32();
                bin.ReadInt16();

                while (bin.BaseStream.Length != bin.BaseStream.Position)
                {
                    int number = bin.ReadInt32();
                    bin.ReadByte();
                    int length = bin.ReadInt16();

                    if (length > m_Buffer.Length)
                        m_Buffer = new byte[(length + 1023) & ~1023];

                    bin.Read(m_Buffer, 0, length);
                    string text = Encoding.UTF8.GetString(m_Buffer, 0, length);

                    list.Add(new StringEntry(number, text));
                    m_Table[number] = text;
                }
                Metrics.ReportDataRead((int)bin.BaseStream.Position);
            }

            m_Entries = (StringEntry[])list.ToArray(typeof(StringEntry));
        }

        public static void Debug_WriteStringList()
        {
            // create a writer and open the file
            TextWriter tw = new StreamWriter("cliloc.txt");
            for (int i = 0; i < m_Entries.Length; i++)
            {
                // write a line of text to the file
                tw.WriteLine(m_Entries[i].Number + ": " + m_Entries[i].Text);
            }

            // close the stream
            tw.Close();
        }
    }

    public class StringEntry
    {
        private int m_Number;
        private string m_Text;

        public int Number { get { return m_Number; } }
        public string Text { get { return m_Text; } }

        public StringEntry(int number, string text)
        {
            m_Number = number;
            m_Text = text;
        }
    }
}