/***************************************************************************
 *   StringList.cs
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
using System.IO;
using System.Text;
using System.Collections;
#endregion

namespace UltimaXNA.UltimaData
{
    public class StringData
    {
        private static Hashtable m_Table;
        private static StringEntry[] m_Entries;
        private static string m_Language;

        public static StringEntry[] Entries { get { return m_Entries; } }
        public static Hashtable Table { get { return m_Table; } }
        public static string Language { get { return m_Language; } }

        private static byte[] m_Buffer = new byte[1024];

        public static string Entry(int index)
        {
            if (m_Table[index] == null)
                return string.Empty;
            else
                return m_Table[index].ToString();
        }

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

            byte[] buffer;

            using (BinaryReader bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                buffer = bin.ReadBytes((int)bin.BaseStream.Length);
                Diagnostics.Metrics.ReportDataRead((int)bin.BaseStream.Position);
            }

            int pos = 6;
            int count = buffer.Length;
            while (pos < count)
            {
                int number = BitConverter.ToInt32(buffer, pos);
                int length = BitConverter.ToInt16(buffer, pos + 5);
                string text = Encoding.UTF8.GetString(buffer, pos + 7, length);
                pos += length + 7;
                list.Add(new StringEntry(number, text));
                m_Table[number] = text;
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