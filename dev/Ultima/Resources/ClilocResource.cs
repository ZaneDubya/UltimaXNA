/***************************************************************************
 *   StringData.cs
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
using System.Text;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public class ClilocResource
    {
        public Hashtable Table
        {
            get;
            private set;
        }

        public string Language
        {
            get;
            private set;
        }

        public ClilocResource(string language)
        {
            LoadStringList(language);
        }

        public string GetString(int index)
        {
            if (Table[index] == null)
            {
                Tracer.Warn("Missing cliloc with index {0}. Client version is lower than expected by Server.", index);
                return string.Format("Err: Cliloc Entry {0} not found.", index);
            }
            else
                return Table[index].ToString();
        }

        public void LoadStringList(string language)
        {
            Language = language;
            Table = new Hashtable();

            string path = FileManager.GetFilePath(String.Format("cliloc.{0}", language));

            if (path == null)
            {
                return;
            }

            ArrayList list = new ArrayList();

            byte[] buffer;

            using (BinaryReader bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                buffer = bin.ReadBytes((int)bin.BaseStream.Length);
                Metrics.ReportDataRead((int)bin.BaseStream.Position);
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
                Table[number] = text;
            }
        }

        private class StringEntry
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
}