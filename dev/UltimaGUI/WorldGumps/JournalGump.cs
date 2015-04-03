/***************************************************************************
 *   JournalGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Text;
using UltimaXNA.UltimaGUI.Controls;
using System.Collections.Generic;
using UltimaXNA.UltimaGUI.WorldGumps;

namespace UltimaXNA.UltimaGUI.WorldGumps
{
    class JournalGump : Gump
    {
        ExpandableScroll m_scroll;
        HtmlGump m_list;

        public JournalGump()
            : base(0, 0)
        {
            AddControl(m_scroll = new ExpandableScroll(this, 0, 0, 0, 300));
            m_scroll.TitleGumpID = 0x82A;
            m_scroll.MakeDragger(this);
            m_scroll.MakeCloseTarget(this);
            IsMovable = true;

            AddControl(m_list = new HtmlGump(this, 0, 10, 20, 280, 100, 0, 1, ""));
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            m_list.X = 26;
            m_list.Y = 33;
            m_list.Width = this.Width - 56;
            m_list.Height = this.Height - 95;
            m_list.Text = "";
            ChatWindow chat = UltimaEngine.UserInterface.GetControl<ChatWindow>(0);

            for (int i = 0; i < chat.m_journalHistory.Count; i++)
                m_list.Text += "<left color=50422D><span width='14'/>" + chat.m_journalHistory[i] + "</left><br/>";
            base.Update(gameTime);
        }

    }
}
