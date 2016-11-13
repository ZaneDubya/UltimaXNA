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
using UltimaXNA.Ultima.World.Maps;
using System.Text;

namespace UltimaXNA.Ultima.World.Entities.Items
{
    public class BaseBook : Item
    {
        string m_Title;
        string m_Author;
        BookPageInfo[] m_Pages;
        bool m_Writable;
        static ushort[] m_BookItemIDs = {
            0xFEF, // Brown Book
            0xFF0, // Tan Book
            0xFF1, // Red Book
            0xFF2  // Blue Book
        };

        public static bool IsBookItem(ushort itemID)
        {
            return m_BookItemIDs.Contains(itemID);
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
            : this(serial, map, true)
        {
        }

        public BaseBook(Serial serial, Map map, bool writable)
            : this(serial, map, writable, null, null)
        {
        }

        public BaseBook(Serial serial, Map map, bool writable, string title, string author)
            : base(serial, map)
        {
            m_Title = title;
            m_Author = author;
            m_Writable = writable;
            m_Pages = new BookPageInfo[0];
        }

        public class BookPageInfo
        {
            string[] m_Lines;
            public string[] Lines
            {
                get
                {
                    return m_Lines;
                }
                set
                {
                    m_Lines = value;
                }
            }

            public BookPageInfo()
            {
                m_Lines = new string[0];
            }

            public BookPageInfo(string[] lines)
            {
                m_Lines = lines;
            }

            public string getAllLines()
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < m_Lines.Length; i++)
                {
                    sb.AppendLine(m_Lines[i]);
                }

                return sb.ToString();
            }
        }
    }
}