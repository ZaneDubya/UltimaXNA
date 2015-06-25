/***************************************************************************
 *   SpellbookGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Core.Input;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Data;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class SpellbookGump : Gump
    {
        // ================================================================================
        // Private variables
        // ================================================================================
        SpellBook m_Spellbook;
        HtmlGumpling[] m_Indexes;

        // ================================================================================
        // Private services 
        // ================================================================================
        private WorldModel m_World;

        // ================================================================================
        // Ctor, Update, Dispose
        // ================================================================================
        public SpellbookGump(SpellBook entity, int itemID)
            : base(0, 0)
        {
            m_Spellbook = entity;
            m_Spellbook.OnEntityUpdated += OnEntityUpdate;

            IsMovable = true;

            if (m_Spellbook.BookType != Data.SpellBookTypes.Unknown)
            {
                CreateMageryGumplings();
            }
            else
            {
                // display a default spellbook graphic, based on the default spellbook type for this item ID.
                // right now, I'm just using a magery background, but really the background should change based
                // on the item id.
                AddControl(new GumpPic(this, 0, 0, 0x08AC, 0));
                // other options? necro? spellweaving?
            }
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Dispose()
        {
            m_Spellbook.OnEntityUpdated -= OnEntityUpdate;
            base.Dispose();
        }

        // ================================================================================
        // OnEntityUpdate - called when spellbook entity is updated by server.
        // ================================================================================
        private void OnEntityUpdate()
        {
            if (m_Spellbook.BookType == Data.SpellBookTypes.Magic)
                CreateMageryGumplings();
        }

        // ================================================================================
        // Child control creation
        // The spellbook is laid out as such:
        // 1. A list of all spells in the book. Clicking on a spell will turn to that spell's page.
        // 2. One page per spell in the book. Icon, runes, reagents, etc.
        // ================================================================================
        private GumpPic m_PageCornerLeft;
        private GumpPic m_PageCornerRight;
        private int m_MaxPage = 0;

        private void CreateMageryGumplings()
        {
            ClearControls();

            AddControl(new GumpPic(this, 0, 0, 0x08AC, 0)); // spellbook background

            m_PageCornerLeft = (GumpPic)AddControl(new GumpPic(this, 50, 8, 0x08BB, 0)); // page turn left
            LastControl.GumpLocalID = 0;
            LastControl.MouseClickEvent += PageCorner_MouseClickEvent;
            LastControl.MouseDoubleClickEvent += PageCorner_MouseDoubleClickEvent;

            m_PageCornerRight = (GumpPic)AddControl(new GumpPic(this, 321, 8, 0x08BC, 0)); // page turn right
            LastControl.GumpLocalID = 1;
            LastControl.MouseClickEvent += PageCorner_MouseClickEvent;
            LastControl.MouseDoubleClickEvent += PageCorner_MouseDoubleClickEvent;

            for (int i = 0; i < 4; i++) // spell circles 1 - 4
            {
                AddControl(new GumpPic(this, 60 + i * 35, 174, 0x08B1 + i, 0));
                LastControl.GumpLocalID = i;
                LastControl.MouseClickEvent += SpellCircle_MouseClickEvent;
            }
            for (int i = 0; i < 4; i++) // spell circles 5 - 8
            {
                AddControl(new GumpPic(this, 226 + i * 34, 174, 0x08B5 + i, 0));
                LastControl.GumpLocalID = i + 4;
                LastControl.MouseClickEvent += SpellCircle_MouseClickEvent;
            }

            // indexes are on pages 1 - 4. Spells are on pages 5+.
            m_Indexes = new HtmlGumpling[8];
            for (int i = 0; i < 8; i++)
            {
                m_Indexes[i] = (HtmlGumpling)AddControl(
                    new HtmlGumpling(this, 68 + (i % 2) * 154, 8, 120, 200, 0, 0,
                        string.Format("<span style='font-family=ascii6;'><span color='#008'><center>{0}</center></span><br/>",
                        Magery.CircleNames[i])), 
                    1 + (i / 2));
            }

            // add indexes and spell pages.
            m_MaxPage = 5;
            int currentSpellPage = 6;
            bool isRightPage = false;
            for (int spellCircle = 0; spellCircle < 8; spellCircle++)
            {
                for (int spellIndex = 0; spellIndex < 8; spellIndex++)
                {
                    int spellIndexAll = spellCircle * 8 + spellIndex;
                    if (m_Spellbook.HasSpell(spellIndexAll))
                    {
                        m_Indexes[spellCircle].Text += string.Format("<a href='page={1}' color='#654' hovercolor='#973' activecolor='#611' style='font-family=ascii9; text-decoration=none;'>{0}</a><br/>", Magery.Spells[spellIndexAll % 8].Name, currentSpellPage);
                        if (isRightPage)
                        {
                            currentSpellPage++;
                            isRightPage = false;
                        }
                        else
                        {
                            m_MaxPage += 1;
                            isRightPage = true;
                        }
                    }
                }
            }

            ActivePage = 1;
        }

        private void SpellCircle_MouseClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            ActivePage = (sender.GumpLocalID / 2) + 1;
        }

        private void PageCorner_MouseClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;

            if (sender.GumpLocalID == 0)
            {
                ActivePage -= 1;
                if (ActivePage < 1)
                    ActivePage = 1;
            }
            else
            {
                ActivePage += 1;
                if (ActivePage > m_MaxPage)
                    ActivePage = m_MaxPage;
            }
        }

        private void PageCorner_MouseDoubleClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;

            if (sender.GumpLocalID == 0)
            {
                ActivePage = 1;
            }
            else
            {
                ActivePage = m_MaxPage;
            }
        }
    }
}
