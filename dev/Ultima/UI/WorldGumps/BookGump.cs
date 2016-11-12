/***************************************************************************
 *   BookGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using System.Text;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Audio;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Items;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class BookGump : Gump
    {
        BaseBook m_Book;
        GumpPic m_BookBackground;
        GumpPic m_PageCornerLeft;
        GumpPic m_PageCornerRight;
        List<TextEntry> textEntries = new List<TextEntry>();
        TextEntry titleTextEntry;
        TextEntry authorTextEntry;
        int m_lastPage;
        WorldModel m_World;

        ushort[] m_GumpBaseIDs =
        {
            0x1F4, // Yellow Cornered Book
            0x1FE, // Regular Cornered Book
            0x898, // Funky Book?
            0x899, // Tan Book?
            0x89A, // Red Book?
            0x89B, // Blue Book?
            0x8AC, // SpellBook
            0x2B00, // Necromancy Book?
            0x2B01, // Ice Book?
            0x2B02, // Arms Book?
            0x2B06, // Bushido Book?
            0x2B07, // Another Crazy Kanji Thing
            0x2B2F // A Greenish Book
        };

        // ================================================================================
        // Ctor, Update, Dispose
        // ================================================================================
        public BookGump(BaseBook entity, int itemID)
            : base(entity.Serial, 0)
        {
            m_Book = entity;
            m_Book.OnEntityUpdated += OnEntityUpdate;
            m_lastPage = (m_Book.PagesCount + 2) / 2;
            IsMoveable = true;
            m_World = ServiceRegistry.GetService<WorldModel>();
            BuildGump();
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Dispose()
        {
            m_Book.OnEntityUpdated -= OnEntityUpdate;
            if (m_PageCornerLeft != null)
            {
                m_PageCornerLeft.MouseClickEvent -= PageCorner_MouseClickEvent;
                m_PageCornerLeft.MouseDoubleClickEvent -= PageCorner_MouseDoubleClickEvent;
            }
            if (m_PageCornerRight != null)
            {
                m_PageCornerRight.MouseClickEvent -= PageCorner_MouseClickEvent;
                m_PageCornerRight.MouseDoubleClickEvent -= PageCorner_MouseDoubleClickEvent;
            }
            base.Dispose();
        }

        // ================================================================================
        // OnEntityUpdate - called when book entity is updated by server.
        // ================================================================================
        void OnEntityUpdate()
        {
            BuildGump();
        }

        void BuildGump()
        {
            ClearControls();
            if (m_Book.ItemID >= 0xFEF && m_Book.ItemID <= 0xFF2)
            {
                m_BookBackground = new GumpPic(this, 0, 0, 0x1FE, 0);
                m_PageCornerLeft = new GumpPic(this, 0, 0, 0x1FF, 0);
                m_PageCornerRight = new GumpPic(this, 356, 0, 0x200, 0);
            }
            AddControl(m_BookBackground);   // book background gump
            AddControl(m_PageCornerLeft);   // page turn left
            m_PageCornerLeft.GumpLocalID = 0;
            m_PageCornerLeft.MouseClickEvent += PageCorner_MouseClickEvent;
            m_PageCornerLeft.MouseDoubleClickEvent += PageCorner_MouseDoubleClickEvent;
            AddControl(m_PageCornerRight);  // page turn right
            m_PageCornerRight.GumpLocalID = 1;
            m_PageCornerRight.MouseClickEvent += PageCorner_MouseClickEvent;
            m_PageCornerRight.MouseDoubleClickEvent += PageCorner_MouseDoubleClickEvent;
            // Draw the title and author page
            titleTextEntry = new TextEntry(this, 45, 50, 160, 300, 1, 0, 0, m_Book.Title);
            titleTextEntry.MakeThisADragger();
            titleTextEntry.IsEditable = m_Book.Writable;
            authorTextEntry = new TextEntry(this, 45, 110, 160, 300, 1, 0, 0, m_Book.Author);
            authorTextEntry.MakeThisADragger();
            authorTextEntry.IsEditable = m_Book.Writable;
            AddControl(new HtmlGumpling(this, 45, 30, 160, 300, 0, 0, string.Format("Title:")), 1);
            AddControl(titleTextEntry, 1);
            AddControl(new HtmlGumpling(this, 45, 90, 160, 300, 0, 0, string.Format("Author:")), 1);
            AddControl(authorTextEntry, 1);
            // Add book pages to active pages
            bool isRight = true;
            int activePage = 1;
            for (int i = 0; i < m_Book.PagesCount; i++)
            {
                if (isRight)
                {
                    textEntries.Add(new TextEntry(this, 235, 32, 160, 300, 1, 0, 200, m_Book.Pages[i].getAllLines()));
                    textEntries[i].MakeThisADragger();
                    textEntries[i].IsEditable = m_Book.Writable;
                    AddControl(textEntries[i], activePage);
                    isRight = false;
                    activePage++;
                }
                else
                {
                    textEntries.Add(new TextEntry(this, 45, 32, 160, 300, 1, 0, 200, m_Book.Pages[i].getAllLines()));
                    textEntries[i].MakeThisADragger();
                    textEntries[i].IsEditable = m_Book.Writable;
                    AddControl(textEntries[i], activePage);
                    isRight = true;
                }
            }
            SetActivePage(1);
            AudioService service = ServiceRegistry.GetService<AudioService>();
            service.PlaySound(0x058);
        }

        void PageCorner_MouseClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            IsContentChanged();
            if (sender.GumpLocalID == 0)
            {
                SetActivePage(ActivePage - 1);
            }
            else
            {
                SetActivePage(ActivePage + 1);
            }
            AudioService service = ServiceRegistry.GetService<AudioService>();
            service.PlaySound(0x055);
        }

        void PageCorner_MouseDoubleClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;

            IsContentChanged();

            if (sender.GumpLocalID == 0)
            {
                SetActivePage(1);
            }
            else
            {
                SetActivePage(m_lastPage);
            }
        }

        protected override void CloseWithRightMouseButton()
        {
            IsContentChanged();
            AudioService service = ServiceRegistry.GetService<AudioService>();
            service.PlaySound(0x058);
            base.CloseWithRightMouseButton();
        }

        void SetActivePage(int page)
        {
            if (page < 1)
                page = 1;
            if (page > m_lastPage)
                page = m_lastPage;

            ActivePage = page;

            // Hide the page corners if we're at the first or final page.
            m_PageCornerLeft.Page = (page != 1) ? 0 : int.MaxValue;
            m_PageCornerRight.Page = (page != m_lastPage) ? 0 : int.MaxValue;
        }

        void IsContentChanged()
        {
            // When ActivePage is 1, we should return leftPageIndex = 1 and rightPageIndex = 2
            // When ActivePage is 2, we should return leftPageIndex = 3 and rightPageIndex = 4
            // When ActivePage is 3, we should return leftPageIndex = 5 and rightPageIndex = 6
            // etc.
            int leftPageIndex = ActivePage * 2 - 3;
            int rightPageIndex = (ActivePage * 2 - 3) + 1;

            // Check title, author, and the first page if leftPageIndex < 0
            // Else if leftPageIndex >= 0, they are all pages
            if (leftPageIndex < 0)
            {
                if (titleTextEntry.Text != m_Book.Title || authorTextEntry.Text != m_Book.Author)
                {
                    m_World.Interaction.BookHeaderNewChange(m_Book.Serial, titleTextEntry.Text, authorTextEntry.Text);
                }

                string test = m_Book.Pages[rightPageIndex].getAllLines();
                if (rightPageIndex < textEntries.Count && textEntries[rightPageIndex].Text != test)
                {
                    m_World.Interaction.BookPageChange(m_Book.Serial, rightPageIndex, GetTextEntryAsArray(textEntries[rightPageIndex]));
                }
            }
            else
            {
                if (textEntries[leftPageIndex].Text != m_Book.Pages[leftPageIndex].getAllLines())
                {
                    m_World.Interaction.BookPageChange(m_Book.Serial, leftPageIndex, GetTextEntryAsArray(textEntries[leftPageIndex]));
                }
                if (rightPageIndex < textEntries.Count - 1 && textEntries[rightPageIndex].Text != m_Book.Pages[rightPageIndex].getAllLines())
                {
                    m_World.Interaction.BookPageChange(m_Book.Serial, rightPageIndex, GetTextEntryAsArray(textEntries[rightPageIndex]));
                }
            }
        }

        string[] GetTextEntryAsArray(TextEntry te)
        {
            List<string> lineList = new List<string>();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < te.Text.Length; i++)
            {
                if (!char.IsControl(te.Text[i]))
                {
                    sb.Append(te.Text[i]);
                }
                else
                {
                    if (sb.ToString() != string.Empty)
                    {
                        lineList.Add(sb.ToString());
                        sb = new StringBuilder();
                    }
                }
                // Last character of text
                if (i == te.Text.Length - 1)
                    lineList.Add(sb.ToString());
            }
            string[] lines = new string[lineList.Count];
            lineList.CopyTo(lines);
            return lines;
        }
    }
}
