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
using System.Collections.Generic;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class SpellbookGump : Gump
    {
        // ============================================================================================================
        // Private variables
        // ============================================================================================================
        private SpellBook m_Spellbook;
        private HtmlGumpling[] m_CircleHeaders;
        private HtmlGumpling[] m_Indexes;

        // ============================================================================================================
        // Private services 
        // ============================================================================================================
        private WorldModel m_World;

        // ============================================================================================================
        // Ctor, Update, Dispose
        // ============================================================================================================
        public SpellbookGump(SpellBook entity, int itemID)
            : base(entity.Serial, 0)
        {
            m_World = ServiceRegistry.GetService<WorldModel>();

            m_Spellbook = entity;
            m_Spellbook.OnEntityUpdated += OnEntityUpdate;

            IsMoveable = true;

            if (m_Spellbook.BookType != SpellBookTypes.Unknown)
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

        // ============================================================================================================
        // OnEntityUpdate - called when spellbook entity is updated by server.
        // ============================================================================================================
        private void OnEntityUpdate()
        {
            if (m_Spellbook.BookType == SpellBookTypes.Magic)
                CreateMageryGumplings();
        }

        // ============================================================================================================
        // Child control creation
        // The spellbook is laid out as such:
        // 1. A list of all spells in the book. Clicking on a spell will turn to that spell's page.
        // 2. One page per spell in the book. Icon, runes, reagents, etc.
        // ============================================================================================================
        private GumpPic m_PageCornerLeft;
        private GumpPic m_PageCornerRight;
        private int m_MaxPage = 0;
        private List<KeyValuePair<int, int>> m_SpellList = new List<KeyValuePair<int, int>>();

        private void CreateMageryGumplings()
        {
            ClearControls();

            AddControl(new GumpPic(this, 0, 0, 0x08AC, 0)); // spellbook background

            AddControl(m_PageCornerLeft = new GumpPic(this, 50, 8, 0x08BB, 0)); // page turn left
            m_PageCornerLeft.GumpLocalID = 0;
            m_PageCornerLeft.MouseClickEvent += PageCorner_MouseClickEvent;
            m_PageCornerLeft.MouseDoubleClickEvent += PageCorner_MouseDoubleClickEvent;

            AddControl(m_PageCornerRight = new GumpPic(this, 321, 8, 0x08BC, 0)); // page turn right
            m_PageCornerRight.GumpLocalID = 1;
            m_PageCornerRight.MouseClickEvent += PageCorner_MouseClickEvent;
            m_PageCornerRight.MouseDoubleClickEvent += PageCorner_MouseDoubleClickEvent;

            for (int i = 0; i < 4; i++) // spell circles 1 - 4
            {
                AddControl(new GumpPic(this, 60 + i * 35, 174, 0x08B1 + i, 0));
                LastControl.GumpLocalID = i;
                LastControl.MouseClickEvent += SpellCircle_MouseClickEvent; // unsubscribe from these?
            }
            for (int i = 0; i < 4; i++) // spell circles 5 - 8
            {
                AddControl(new GumpPic(this, 226 + i * 34, 174, 0x08B5 + i, 0));
                LastControl.GumpLocalID = i + 4;
                LastControl.MouseClickEvent += SpellCircle_MouseClickEvent; // unsubscribe from these?
            }

            // indexes are on pages 1 - 4. Spells are on pages 5+.
            m_CircleHeaders = new HtmlGumpling[8];
            for (int i = 0; i < 8; i++)
            {
                m_CircleHeaders[i] = (HtmlGumpling)AddControl(
                    new HtmlGumpling(this, 64 + (i % 2) * 148, 10, 130, 200, 0, 0,
                        string.Format("<span color='#004' style='font-family=uni0;'><center>{0}</center></span>", SpellsMagery.CircleNames[i])),
                        1 + (i / 2));
            }
            m_Indexes = new HtmlGumpling[8];
            for (int i = 0; i < 8; i++)
            {
                m_Indexes[i] = (HtmlGumpling)AddControl(
                    new HtmlGumpling(this, 64 + (i % 2) * 156, 28, 130, 200, 0, 0, string.Empty),
                    1 + (i / 2));
            }

            m_MaxPage = 4;

            // Begin checking which spells are in the spellbook and add them to m_Spells list

            int totalSpells = 0;
            m_SpellList.Clear();
            for (int spellCircle = 0; spellCircle < 8; spellCircle++)
            {
                for (int spellIndex = 1; spellIndex <= 8; spellIndex++)
                {
                    if (m_Spellbook.HasSpell(spellCircle, spellIndex))
                    {
                        m_SpellList.Add(new KeyValuePair<int, int>(spellCircle, spellIndex));
                        totalSpells++;
                    }
                }
            }

            m_MaxPage = m_MaxPage + ((totalSpells + 1) / 2); // The number of additional spell info pages needed

            SetActivePage(1);
        }

        private void CreateSpellPage(int page, bool rightPage, int circle, SpellDefinition spell)
        {
            // header: "NTH CIRCLE"
            AddControl(new HtmlGumpling(this, 64 + (rightPage ? 148 : 0), 10, 130, 200, 0, 0,
                string.Format("<span color='#004' style='font-family=uni0;'><center>{0}</center></span>", SpellsMagery.CircleNames[circle])),
                page);
            // icon and spell name
            AddControl(new HtmlGumpling(this, 56 + (rightPage ? 156 : 0), 38, 130, 44, 0, 0,
                string.Format("<a href='spellicon={0}'><gumpimg src='{1}'/></a>",
                spell.ID, spell.GumpIconID - 0x1298)),
                page);
            AddControl(new HtmlGumpling(this, 104 + (rightPage ? 156 : 0), 38, 88, 40, 0, 0, string.Format(
                "<a href='spell={0}' color='#542' hovercolor='#875' activecolor='#420' style='font-family=uni0; text-decoration=none;'>{1}</a>",
                spell.ID, spell.Name)),
                page);
            // reagents.
            AddControl(new HtmlGumpling(this, 56 + (rightPage ? 156 : 0), 84, 146, 106, 0, 0, string.Format(
                "<span color='#400' style='font-family=uni0;'>Reagents:</span><br/><span style='font-family=ascii6;'>{0}</span>", spell.CreateReagentListString(", "))),
                page);
        }

        public override void OnHtmlInputEvent(string href, MouseEvent e)
        {
            if (e == MouseEvent.DoubleClick)
            {
                    string[] hrefs = href.Split('=');
                    if (hrefs.Length != 2)
                        return;
                    else if (hrefs[0] == "page")
                    {
                        int page;
                        if (int.TryParse(hrefs[1], out page))
                            m_World.Interaction.CastSpell(page-4);
                    }
                    else if (hrefs[0] == "spell")
                    {
                        int spell;
                        if (int.TryParse(hrefs[1], out spell))
                            m_World.Interaction.CastSpell(spell);
                    }
                    else if (hrefs[0] == "spellicon")
                    {
                        int spell;
                        if (int.TryParse(hrefs[1], out spell))
                            m_World.Interaction.CastSpell(spell);
                    }
            }
            else if (e == MouseEvent.Click)
            {
                string[] hrefs = href.Split('=');
                if (hrefs.Length != 2)
                    return;
                if (hrefs[0] == "page")
                {
                    int page;
                    if (int.TryParse(hrefs[1], out page))
                        SetActivePage(page);
                }
            }
            else if (e == MouseEvent.DragBegin)
            {
                string[] hrefs = href.Split('=');
                if (hrefs.Length != 2)
                    return;
                if (hrefs[0] == "spellicon")
                {
                    int spellIndex;
                    if (!int.TryParse(hrefs[1], out spellIndex))
                        return;
                    SpellDefinition spell = SpellsMagery.GetSpell(spellIndex);
                    if (spell.ID == spellIndex)
                    {
                        InputManager input = ServiceRegistry.GetService<InputManager>();
                        UseSpellButtonGump gump = new UseSpellButtonGump(spell);
                        UserInterface.AddControl(gump, input.MousePosition.X - 22, input.MousePosition.Y - 22);
                        UserInterface.AttemptDragControl(gump, input.MousePosition, true);
                    }
                }
            }
        }

        private void SpellCircle_MouseClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            SetActivePage(sender.GumpLocalID / 2 + 1);
        }

        private void PageCorner_MouseClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;

            if (sender.GumpLocalID == 0)
            {
                SetActivePage(ActivePage - 1);
            }
            else
            {
                SetActivePage(ActivePage + 1);
            }
        }

        private void PageCorner_MouseDoubleClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;

            if (sender.GumpLocalID == 0)
            {
                SetActivePage(1);
            }
            else
            {
                SetActivePage(m_MaxPage);
            }
        }

        private void SetActivePage(int page)
        {
            if (page < 1)
                page = 1;
            if (page > m_MaxPage)
                page = m_MaxPage;

            int currentPage = page;
            int currentSpellCircle = currentPage * 2 - 2; // chooses the right spell circle to print on index page
            int currentSpellInfoIndex = currentPage * 2 - 10; // keeps track of which spell info page to print
            for (int currentCol = 0; currentCol < 2; currentCol++)
            {
                bool isRightPage = (currentCol + 1 == 2);
                currentSpellInfoIndex += currentCol; 

                // Create Spell Index page
                if (currentPage <= 4)
                {
                    m_Indexes[currentSpellCircle].Text = "";
                    foreach (KeyValuePair<int, int> spell in m_SpellList)
                    {
                        if (spell.Key == currentSpellCircle)
                        {
                            int currentSpellInfoPage = m_SpellList.IndexOf(spell) / 2;
                            m_Indexes[currentSpellCircle].Text += string.Format("<a href='page={1}' color='#532' hovercolor='#800' activecolor='#611' style='font-family=uni0; text-decoration=none;'>{0}</a><br/>",
                                SpellsMagery.GetSpell(currentSpellCircle * 8 + spell.Value).Name,
                                5 + currentSpellInfoPage);
                        }
                    }
                    currentSpellCircle++;
                }
                else
                {
                    // Create Spell Info Page
                    if (currentSpellInfoIndex < m_SpellList.Count)
                    {
                        CreateSpellPage(page, isRightPage, m_SpellList[currentSpellInfoIndex].Key, SpellsMagery.GetSpell(m_SpellList[currentSpellInfoIndex].Key * 8 + m_SpellList[currentSpellInfoIndex].Value));
                    }
                }
            }

            ActivePage = page;
            // hide the page corners if we're at the first or final page.
            m_PageCornerLeft.Page = (ActivePage != 1) ? 0 : int.MaxValue;
            m_PageCornerRight.Page = (ActivePage != m_MaxPage) ? 0 : int.MaxValue;
        }
    }
}
