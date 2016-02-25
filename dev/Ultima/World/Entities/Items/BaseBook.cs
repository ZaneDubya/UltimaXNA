/***************************************************************************
 *   BaseBook.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Linq;
using System.Collections.Generic;
using UltimaXNA.Ultima.World.Maps;
using System.Text;

namespace UltimaXNA.Ultima.World.Entities.Items
{
    public class BaseBook : Item
    {
        private string m_Title;
        private string m_Author;
        private BookPageInfo[] m_Pages;
        private bool m_Writable;
        private static ushort[] m_BookItemIDs = new ushort[] {
            0xFEF, // Brown Book
            0xFF0, // Tan Book
            0xFF1, // Red Book
            0xFF2  // Blue Book
        };

        public static bool IsBookItem(ushort itemID)
        {
            return m_BookItemIDs.Contains<ushort>(itemID);
        }

        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        public string Author
        {
            get { return m_Author; }
            set { m_Author = value; }
        }

        public bool Writable
        {
            get { return m_Writable; }
            set { m_Writable = value; }
        }

        public int PagesCount
        {
            get { return m_Pages.Length; }
        }

        public BookPageInfo[] Pages
        {
            get { return m_Pages; }
            set { m_Pages = value; }
        }

        public BaseBook(Serial serial, Map map) 
            : this(serial, map, 20, true)
        {
        }

        public BaseBook(Serial serial, Map map, int pageCount, bool writable)
            : this(serial, map, pageCount, writable, null, null)
        {
        }

        public BaseBook(Serial serial, Map map, int pageCount, bool writable, string title, string author)
            : base(serial, map)
        {
            m_Title = title;
            m_Author = author;
            m_Writable = writable;
            m_Pages = new BookPageInfo[0];
        }

        public string ContentAsString
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (BookPageInfo bpi in this.m_Pages)
                {
                    foreach (string line in bpi.Lines)
                    {
                        sb.AppendLine(line);
                    }
                }

                return sb.ToString();
            }
        }

        public string[] ContentAsStringArray
        {
            get
            {
                List<string> lines = new List<string>();

                foreach (BookPageInfo bpi in this.m_Pages)
                {
                    lines.AddRange(bpi.Lines);
                }

                return lines.ToArray();
            }
        }

        public class BookPageInfo
        {
            private string[] m_lines;

            public string[] Lines
            {
                get
                {
                    return m_lines;
                }

                set
                {
                    m_lines = value;
                }
            }

            public BookPageInfo()
            {
                m_lines = new string[0];
            }

            public BookPageInfo(string[] lines)
            {
                m_lines = lines;
            }

            public string getAllLines()
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < m_lines.Length; i++)
                {
                    sb.AppendLine(m_lines[i]);
                }

                return sb.ToString();
            }
        }
    }
}