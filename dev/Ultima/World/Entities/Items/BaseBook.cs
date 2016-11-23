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
using UltimaXNA.Ultima.World.Maps;
using System.Text;

namespace UltimaXNA.Ultima.World.Entities.Items
{
    public class BaseBook : Item
    {
        string m_Title;
        string m_Author;
        BookPageInfo[] m_Pages;
        bool m_IsEditable;

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

        public bool IsEditable
        {
            get { return m_IsEditable; }
            set { m_IsEditable = value; }
        }

        public int PageCount => m_Pages.Length;

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
            m_IsEditable = writable;
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

            public string GetAllLines()
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < m_Lines.Length; i++)
                {
                    sb.Append($"{m_Lines[i]}\n");
                }
                return sb.ToString();
            }
        }
    }
}