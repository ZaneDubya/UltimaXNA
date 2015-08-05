/***************************************************************************
 *   JournalGump.cs
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
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class JournalGump : Gump
    {
        private ExpandableScroll m_Background;
        private RenderedTextList m_JournalEntries;
        private IScrollBar m_ScrollBar;

        public JournalGump()
            : base(0, 0)
        {
            IsMoveable = true;

            AddControl(m_Background = new ExpandableScroll(this, 0, 0, 300));
            m_Background.TitleGumpID = 0x82A;

            m_ScrollBar = (IScrollBar)AddControl(new ScrollFlag(this));
            AddControl(m_JournalEntries = new RenderedTextList(this, 30, 36, 242, 200, m_ScrollBar));
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("journal");

            InitializeJournalEntries();
            PlayerState.Journaling.OnJournalEntryAdded += AddJournalEntry;
        }

        public override void Update(double totalMS, double frameMS)
        {
            m_JournalEntries.Height = Height - 98;
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);
        }

        private void AddJournalEntry(string text)
        {
            m_JournalEntries.AddEntry(string.Format("<left color='#50422D'><span width='14'/>{0}</left><br/>", text));
        }

        private void InitializeJournalEntries()
        {
            for (int i = 0; i < PlayerState.Journaling.JournalEntries.Count; i++)
            {
                AddJournalEntry(PlayerState.Journaling.JournalEntries[i]);
            }

            m_ScrollBar.MinValue = 0;
        }
    }
}
