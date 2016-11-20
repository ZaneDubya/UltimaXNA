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
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    public class BookGump : Gump
    {
        BaseBook m_Book;
        GumpPic m_BookBackground;
        GumpPic m_PageCornerLeft;
        GumpPic m_PageCornerRight;
        List<TextEntryPage> m_TextEntries = new List<TextEntryPage>();
        TextEntry m_TitleTextEntry;
        TextEntry m_AuthorTextEntry;
        int m_LastPage;
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
        public BookGump(BaseBook entity)
            : base(entity.Serial, 0)
        {
            m_Book = entity;
            m_Book.SetCallbacks(OnEntityUpdate, OnEntityDispose);
            m_LastPage = (m_Book.PageCount + 2) / 2;
            IsMoveable = true;
            m_World = ServiceRegistry.GetService<WorldModel>(false);
            BuildGump();
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Dispose()
        {
            m_Book.ClearCallBacks(OnEntityUpdate, OnEntityDispose);
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
        void OnEntityUpdate(AEntity entity)
        {
            m_Book = entity as BaseBook;
            BuildGump();
        }

        void OnEntityDispose(AEntity entity)
        {
            Dispose();
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
            m_TitleTextEntry = new TextEntry(this, 45, 50, 155, 300, 1, 0, 0, m_Book.Title);
            m_TitleTextEntry.MakeThisADragger();
            m_TitleTextEntry.IsEditable = m_Book.IsEditable;
            m_AuthorTextEntry = new TextEntry(this, 45, 110, 160, 300, 1, 0, 0, m_Book.Author);
            m_AuthorTextEntry.MakeThisADragger();
            m_AuthorTextEntry.IsEditable = m_Book.IsEditable;
            AddControl(m_TitleTextEntry, 1);
            AddControl(new HtmlGumpling(this, 45, 90, 155, 300, 0, 0, string.Format("Author:")), 1);
            AddControl(m_AuthorTextEntry, 1);
            // Add book pages to active pages
            bool isRight = true;
            int activePage = 1;
            for (int i = 0; i < m_Book.PageCount; i++)
            {
                int x = isRight ? 235 : 45;
                m_TextEntries.Add(new TextEntryPage(this, x, 32, 160, 300, 8));
                m_TextEntries[i].MakeThisADragger();
                m_TextEntries[i].IsEditable = m_Book.IsEditable;
                m_TextEntries[i].LeadingHtmlTag = "<font color=#800>";
                AddControl(m_TextEntries[i], activePage);
                if (isRight)
                {
                    activePage++;
                }
                isRight = !isRight;
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
                SetActivePage(m_LastPage);
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
            if (page > m_LastPage)
                page = m_LastPage;

            ActivePage = page;

            // Hide the page corners if we're at the first or final page.
            m_PageCornerLeft.Page = (page != 1) ? 0 : int.MaxValue;
            m_PageCornerRight.Page = (page != m_LastPage) ? 0 : int.MaxValue;
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
                if (m_TitleTextEntry.Text != m_Book.Title || m_AuthorTextEntry.Text != m_Book.Author)
                {
                    m_World?.Interaction.BookHeaderNewChange(m_Book.Serial, m_TitleTextEntry.Text, m_AuthorTextEntry.Text);
                }
                if (rightPageIndex < m_TextEntries.Count && m_TextEntries[rightPageIndex].Text != m_Book.Pages[rightPageIndex].GetAllLines())
                {
                    m_World?.Interaction.BookPageChange(m_Book.Serial, rightPageIndex, GetTextEntryAsArray(m_TextEntries[rightPageIndex]));
                }
            }
            else
            {
                if (m_TextEntries[leftPageIndex].Text != m_Book.Pages[leftPageIndex].GetAllLines())
                {
                    m_World?.Interaction.BookPageChange(m_Book.Serial, leftPageIndex, GetTextEntryAsArray(m_TextEntries[leftPageIndex]));
                }
                if (rightPageIndex < m_TextEntries.Count - 1 && m_TextEntries[rightPageIndex].Text != m_Book.Pages[rightPageIndex].GetAllLines())
                {
                    m_World?.Interaction.BookPageChange(m_Book.Serial, rightPageIndex, GetTextEntryAsArray(m_TextEntries[rightPageIndex]));
                }
            }
        }

        string[] GetTextEntryAsArray(TextEntryPage text)
        {
            List<string> lineList = new List<string>();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < text.Text.Length; i++)
            {
                if (!char.IsControl(text.Text[i]))
                {
                    sb.Append(text.Text[i]);
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
                if (i == text.Text.Length - 1)
                    lineList.Add(sb.ToString());
            }
            string[] lines = new string[lineList.Count];
            lineList.CopyTo(lines);
            return lines;
        }
    }
}
