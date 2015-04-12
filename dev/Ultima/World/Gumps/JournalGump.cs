/***************************************************************************
 *   JournalGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.World.Gumps
{
    class JournalGump : Gump
    {
        ExpandableScroll m_Background;
        List<RenderedText> m_JournalEntries;
        ScrollBar m_ScrollBar;

        public JournalGump()
            : base(0, 0)
        {
            AddControl(m_Background = new ExpandableScroll(this, 0, 0, 0, 300));
            m_Background.TitleGumpID = 0x82A;
            m_Background.MakeDragger(this);
            m_Background.MakeCloseTarget(this);

            AddControl(m_ScrollBar = new ScrollBar(this, 0));
            IsMovable = true;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_JournalEntries = new List<RenderedText>();
            InitializeJournalEntries();
            PlayerState.Journaling.OnJournalEntryAdded += AddJournalEntry;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);

            m_ScrollBar.Position = new Point(Width - 45, 35);
            m_ScrollBar.Height = Height - 100;
            if (m_ScrollBar.MaxValue <= 0)
                m_ScrollBar.Visible = false;
            else
            {
                if (m_ScrollBar.Value < 0)
                    m_ScrollBar.Value = 0;
                m_ScrollBar.Visible = false;
            }
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            base.Draw(spriteBatch);

            Point p = new Point(X + 36, Y + Height - 65);
            int maxheight = Height - 100;
            int height = 0;

            for (int i = m_JournalEntries.Count - 1; i >= 0; i--)
            {
                if (height + m_JournalEntries[i].Height <= maxheight)
                {
                    p.Y -= m_JournalEntries[i].Height;
                    height += m_JournalEntries[i].Height;
                    m_JournalEntries[i].Draw(spriteBatch, p);
                }
                else
                {
                    int y = (maxheight - height);
                    m_JournalEntries[i].Draw(spriteBatch, new Rectangle(p.X, Y + 35, m_JournalEntries[i].Width, y), 0, m_JournalEntries[i].Height - y);
                    break;
                }
            }
        }


        private void AddJournalEntry(string text)
        {
            while (m_JournalEntries.Count > 99)
            {
                m_ScrollBar.MaxValue -= m_JournalEntries[0].Height;
                m_JournalEntries.RemoveAt(0);
            }
            m_JournalEntries.Add(new RenderedText(string.Format("<left color=50422D><span width='14'/>{0}</left><br/>", text), true, 200));
            m_ScrollBar.MaxValue += m_JournalEntries[m_JournalEntries.Count - 1].Height;
        }

        private void InitializeJournalEntries()
        {
            int height = 0;
            for (int i = 0; i < PlayerState.Journaling.JournalEntries.Count; i++)
            {
                AddJournalEntry(PlayerState.Journaling.JournalEntries[i]);
                height += m_JournalEntries[i].Height;
            }

            m_ScrollBar.MinValue = 0;
            m_ScrollBar.MaxValue = height - 100;
            if (m_ScrollBar.Value < 0)
                m_ScrollBar.Value = 0;
        }
    }
}
