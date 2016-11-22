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
        List<TextEntryPage> m_Pages = new List<TextEntryPage>();
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
            int page = 1;
            for (int i = 0; i < m_Book.PageCount; i++)
            {
                int x = isRight ? 235 : 45;
                m_Pages.Add(new TextEntryPage(this, x, 32, 155, 300, (page - 1) * 2 + (isRight ? 1 : 0)));
                m_Pages[i].SetMaxLines(8, OnPageOverflow, OnPageUnderflow);
                m_Pages[i].MakeThisADragger();
                m_Pages[i].IsEditable = m_Book.IsEditable;
                m_Pages[i].LeadingHtmlTag = "<font color=#800>";
                AddControl(m_Pages[i], page);
                if (isRight)
                {
                    page++;
                }
                isRight = !isRight;
            }
            SetActivePage(1);
            AudioService service = ServiceRegistry.GetService<AudioService>();
            service.PlaySound(0x058);
            m_Pages[0].Text = "since|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| feeling is first who pays any attention to the syntax of things will never wholly kiss you; wholly to be a fool while Spring is in the world my blood approves and kisses are a better fate than wisdom ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||";
            m_Pages[0].CaratAt = m_Pages[0].Text.Length;
        }

        void PageCorner_MouseClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
            {
                return;
            }
            CheckForContentChanges();
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
            {
                return;
            }
            CheckForContentChanges();
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
            CheckForContentChanges();
            AudioService service = ServiceRegistry.GetService<AudioService>();
            service.PlaySound(0x058);
            base.CloseWithRightMouseButton();
        }

        void SetActivePage(int page)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (page > m_LastPage)
            {
                page = m_LastPage;
            }
            ActivePage = page;
            // Hide the page corners if we're at the first or final page.
            m_PageCornerLeft.Page = (page != 1) ? 0 : int.MaxValue;
            m_PageCornerRight.Page = (page != m_LastPage) ? 0 : int.MaxValue;
            int textEntryPageIndex = (page - 1) * 2 - 1;
            if (textEntryPageIndex == -1)
            {
                textEntryPageIndex = 0;
            }
            if (m_Pages[textEntryPageIndex] != null)
            {
                UserInterface.KeyboardFocusControl = m_Pages[textEntryPageIndex];
                m_Pages[textEntryPageIndex].CaratAt = m_Pages[textEntryPageIndex].Text.Length;
            }
            
        }

        void CheckForContentChanges()
        {
            int leftIndex = ActivePage * 2 - 3;
            int rightIndex = leftIndex + 1;
            // Check title, author, and the first page if leftPageIndex < 0
            // Else if leftPageIndex >= 0, they are all pages
            if (leftIndex < 0)
            {
                if (m_TitleTextEntry.Text != m_Book.Title || m_AuthorTextEntry.Text != m_Book.Author)
                {
                    m_World?.Interaction.BookHeaderNewChange(m_Book.Serial, m_TitleTextEntry.Text, m_AuthorTextEntry.Text);
                }
                if (rightIndex < m_Pages.Count && m_Pages[rightIndex].Text != m_Book.Pages[rightIndex].GetAllLines())
                {
                    m_World?.Interaction.BookPageChange(m_Book.Serial, rightIndex, GetTextEntryAsArray(m_Pages[rightIndex]));
                }
            }
            else
            {
                if (m_Pages[leftIndex].Text != m_Book.Pages[leftIndex].GetAllLines())
                {
                    m_World?.Interaction.BookPageChange(m_Book.Serial, leftIndex, GetTextEntryAsArray(m_Pages[leftIndex]));
                }
                if (rightIndex < m_Pages.Count - 1 && m_Pages[rightIndex].Text != m_Book.Pages[rightIndex].GetAllLines())
                {
                    m_World?.Interaction.BookPageChange(m_Book.Serial, rightIndex, GetTextEntryAsArray(m_Pages[rightIndex]));
                }
            }
        }

        /// <summary>
        /// Called when the user hits backspace at index 0 on a page. 
        /// </summary>
        void OnPageUnderflow(int index)
        {
            if (index <= 1)
            {
                return;
            }
            int underflowFrom = index - 1;
            int underflowTo = index - 2;
            string underflowFromText = m_Pages[underflowFrom].Text;
            string underflowToText = m_Pages[underflowTo].Text.Substring(0, (m_Pages[underflowTo].Text.Length > 0 ? m_Pages[underflowTo].Text.Length - 1 : 0));
            m_Pages[underflowFrom].Text = string.Empty;
            SetActivePage((underflowTo + 1) / 2 + 1);
            m_Pages[underflowTo].Text = underflowToText + underflowFromText;
        }

        /// <summary>
        /// Called when text on a page is too large to be held in the page. The text overflows to the next page.
        /// </summary>
        void OnPageOverflow(int index, string overflow)
        {
            int overflowFrom = index - 1;
            int overflowTo = index;
            if (overflowTo >= m_Pages.Count)
            {
                // this text has been pushed out of the book.
            }
            else
            {
                m_Pages[overflowTo].Text = m_Pages[overflowTo].Text.Insert(0, overflow);
                CheckForContentChanges();
                SetActivePage((overflowTo + 1) / 2 + 1);
                UserInterface.KeyboardFocusControl = m_Pages[overflowTo];
                m_Pages[overflowTo].CaratAt = overflow.Length;
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
                {
                    lineList.Add(sb.ToString());
                }
            }
            string[] lines = new string[lineList.Count];
            lineList.CopyTo(lines);
            return lines;
        }
    }
}
