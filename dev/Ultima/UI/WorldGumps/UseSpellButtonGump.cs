/***************************************************************************
 *   UseSpellButtonGump.cs
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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    public class UseSpellButtonGump : Gump
    {
        // private variables
        private SpellDefinition m_Spell;
        private GumpPic m_SpellButton;
        // services
        private readonly WorldModel m_World;

        public UseSpellButtonGump(SpellDefinition spell)
            : base(spell.ID, 0)
        {
            while (UserInterface.GetControl<UseSpellButtonGump>(spell.ID) != null)
            {
                UserInterface.GetControl<UseSpellButtonGump>(spell.ID).Dispose();
            }

            m_Spell = spell;
            m_World = Services.Get<WorldModel>();

            IsMoveable = true;
            HandlesMouseInput = true;

            m_SpellButton = (GumpPic)AddControl(new GumpPic(this, 0, 0, spell.GumpIconSmallID, 0));
            m_SpellButton.HandlesMouseInput = true;
            m_SpellButton.MouseDoubleClickEvent += EventMouseDoubleClick;
        }

        public override void Dispose()
        {
            m_SpellButton.MouseDoubleClickEvent -= EventMouseDoubleClick;
            base.Dispose();
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
        }

        private void EventMouseDoubleClick(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            m_World.Interaction.CastSpell(m_Spell.ID);
        }
    }
}