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
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class SpellbookGump : Gump
    {
        // ================================================================================
        // Private variables
        // ================================================================================
        SpellBook m_Spellbook;

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
                CreateSpellbookGumplings();
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
            if (m_Spellbook.BookType != Data.SpellBookTypes.Unknown)
                CreateSpellbookGumplings();
        }

        // ================================================================================
        // Child control creation
        // The spellbook is laid out as such:
        // 1. A list of all spells in the book. Clicking on a spell will turn to that spell's page.
        // 2. One page per spell in the book. Icon, runes, reagents, etc.
        // ================================================================================
        private int[] m_PageNumbersForCircles, m_PageNumbersForSpells;
        
        private GumpPic m_PageCornerLeft;
        private GumpPic m_PageCornerRight;

        private void CreateSpellbookGumplings()
        {
            ClearControls();
            AddControl(new GumpPic(this, 0, 0, 0x08AC, 0)); // spellbook background
            m_PageCornerLeft = (GumpPic)AddControl(new GumpPic(this, 50, 8, 0x08BB, 0)); // page turn left
            m_PageCornerRight = (GumpPic)AddControl(new GumpPic(this, 321, 8, 0x08BC, 0)); // page turn right

            for (int i = 0; i < 4; i++) // spell circles 1 - 4
            {
                AddControl(new GumpPic(this, 60 + i * 35, 174, 0x08B1 + i, 0));
                LastControl.GumpLocalID = i + 1;
                LastControl.MouseClickEvent += SpellCircle_MouseClickEvent;
                LastControl.MouseDoubleClickEvent += SpellCircle_MouseDoubleClickEvent;
            }
            for (int i = 0; i < 4; i++) // spell circles 5 - 8
            {
                AddControl(new GumpPic(this, 226 + i * 34, 174, 0x08B5 + i, 0));
                LastControl.GumpLocalID = i + 5;
                LastControl.MouseClickEvent += SpellCircle_MouseClickEvent;
                LastControl.MouseDoubleClickEvent += SpellCircle_MouseDoubleClickEvent;
            }

            // add indexes

            // add spell pages
            for (int i = 0; i < 64; i++)
            {
                if (m_Spellbook.HasSpell(i))
                {
                    // add a page for this spell.
                }
            }
        }

        private void SpellCircle_MouseClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
        }

        private void SpellCircle_MouseDoubleClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
        }
    }
}
