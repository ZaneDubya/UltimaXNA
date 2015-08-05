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
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class SpellbookGump : Gump
    {
        // ================================================================================
        // Private variables
        // ================================================================================
        SpellBook m_Spellbook;
        HtmlGumpling[] m_CircleHeaders;
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
            base.Dispose();
        }

        // ================================================================================
        // OnEntityUpdate - called when spellbook entity is updated by server.
        // ================================================================================
        private void OnEntityUpdate()
        {
            if (m_Spellbook.BookType == SpellBookTypes.Magic)
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
            m_CircleHeaders = new HtmlGumpling[8];
            for (int i = 0; i < 8; i++)
            {
                m_CircleHeaders[i] = (HtmlGumpling)AddControl(
                    new HtmlGumpling(this, 64 + (i % 2) * 148, 10, 130, 200, 0, 0,
                        string.Format("<span color='#004' style='font-family=uni0;'><center>{0}</center></span>", Magery.CircleNames[i])),
                        1 + (i / 2));
            }
            m_Indexes = new HtmlGumpling[8];
            for (int i = 0; i < 8; i++)
            {
                m_Indexes[i] = (HtmlGumpling)AddControl(
                    new HtmlGumpling(this, 64 + (i % 2) * 156, 28, 130, 200, 0, 0, string.Empty),
                    1 + (i / 2));
            }

            // add indexes and spell pages.
            m_MaxPage = 4;
            int currentSpellPage = 5;
            bool isRightPage = false;
            for (int spellCircle = 0; spellCircle < 8; spellCircle++)
            {
                for (int spellIndex = 0; spellIndex < 8; spellIndex++)
                {
                    int spellIndexAll = spellCircle * 8 + spellIndex;
                    if (m_Spellbook.HasSpell(spellIndexAll))
                    {
                        m_Indexes[spellCircle].Text += string.Format("<a href='page={1}' color='#532' hovercolor='#800' activecolor='#611' style='font-family=uni0; text-decoration=none;'>{0}</a><br/>",
                            Magery.Spells[spellIndexAll].Name, 
                            currentSpellPage);
                        CreateSpellPage(currentSpellPage, isRightPage, spellCircle, Magery.Spells[spellIndexAll]);
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

            SetActivePage(1);
        }

        private void CreateSpellPage(int page, bool rightPage, int circle, SpellDefinition spell)
        {
            // header: "NTH CIRCLE"
            AddControl(new HtmlGumpling(this, 64 + (rightPage ? 148 : 0), 10, 130, 200, 0, 0,
                string.Format("<span color='#004' style='font-family=uni0;'><center>{0}</center></span>", Magery.CircleNames[circle])),
                page);
            // icon and spell name
            AddControl(new HtmlGumpling(this, 56 + (rightPage ? 156 : 0), 38, 130, 44, 0, 0,
                string.Format("<a href='spellicon={0}'><gumpimg src='{1}'/></a>",
                spell.ID, spell.GumpIconID - 0x1298)),
                page);
            ((HtmlGumpling)LastControl).OnDragHRef += OnSpellDrag;
            AddControl(new HtmlGumpling(this, 104 + (rightPage ? 156 : 0), 38, 88, 40, 0, 0, string.Format(
                "<a href='spell={0}' color='#542' hovercolor='#875' activecolor='#420' style='font-family=uni0; text-decoration=none;'>{1}</a>", 
                spell.ID, spell.Name)), 
                page);
            // reagents.
            AddControl(new HtmlGumpling(this, 56 + (rightPage ? 156 : 0), 84, 146, 106, 0, 0, string.Format(
                "<span color='#400' style='font-family=uni0;'>Reagents:</span><br/><span style='font-family=ascii6;'>{0}</span>", spell.CreateReagentListString(", "))),
                page);
        }

        public override void ActivateByHREF(string href)
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
            else if (hrefs[0] == "spell")
            {
                int spell;
                if (int.TryParse(hrefs[1], out spell))
                    m_World.Interaction.CastSpell(spell + 1);
            }
            else if (hrefs[0] == "spellicon")
            {
                int spell;
                if (int.TryParse(hrefs[1], out spell))
                    m_World.Interaction.CastSpell(spell + 1);
            }
        }

        private void OnSpellDrag(string href)
        {
            string[] hrefs = href.Split('=');
            if (hrefs.Length != 2)
                return;
            if (hrefs[0] == "spellicon")
            {
                int spellIndex;
                if (!int.TryParse(hrefs[1], out spellIndex))
                    return;
                SpellDefinition spell = Magery.Spells[spellIndex];
                InputManager input = ServiceRegistry.GetService<InputManager>();
                UseSpellButtonGump gump = new UseSpellButtonGump(spell);
                UserInterface.AddControl(gump, input.MousePosition.X - 22, input.MousePosition.Y - 22);
                UserInterface.AttemptDragControl(gump, input.MousePosition, true);
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
            ActivePage = page;
            // hide the page corners if we're at the first or final page.
            m_PageCornerLeft.Page = (ActivePage != 1) ? 0 : int.MaxValue;
            m_PageCornerRight.Page = (ActivePage != m_MaxPage) ? 0 : int.MaxValue;
        }
    }
}
