/***************************************************************************
 *   SpellbookGump.cs
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
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class SpellbookGump : Gump
    {
        // Private variables
        SpellBook m_Spellbook;

        // Services
        private WorldModel m_World;

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
                AddControl(new GumpPic(this, 0, 0, 0, 0x08AC, 0));
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

        private void CreateSpellbookGumplings()
        {
            ClearControls();
            AddControl(new GumpPic(this, 0, 0, 0, 0x08AC, 0));
        }

        private void OnEntityUpdate()
        {
            if (m_Spellbook.BookType != Data.SpellBookTypes.Unknown)
                CreateSpellbookGumplings();
        }
    }
}
