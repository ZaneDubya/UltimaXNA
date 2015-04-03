/***************************************************************************
 *   PaperDollGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaEntities;
using UltimaXNA.Core.Rendering;
using UltimaXNA.UltimaGUI.Controls;
using UltimaXNA.UltimaWorld;
using UltimaXNA.Core;

namespace UltimaXNA.UltimaGUI.WorldGumps
{
    class PaperDollGump: Gump
    {
        enum Buttons
        {
            Help,
            Options,
            LogOut,
            Quests,
            Skills,
            Guild,
            PeaceWarToggle,
            Status
        }

        public Mobile Parent
        {
            get;
            private set;
        }

        public PaperDollGump(Mobile parent)
            : base(0, 0)
        {
            Parent = parent;

            IsMovable = true;

            if (parent == (Mobile)EntityManager.GetPlayerObject())
            {
                AddControl(new GumpPic(this, 0, 0, 0, 0x07d0, 0));
                LastControl.MakeDragger(this);
                LastControl.MakeCloseTarget(this);

                // HELP
                AddControl(new Button(this, 0, 185, 44 + 27*0, 0x07ef, 0x07f0, ButtonTypes.Activate, 0,
                    (int) Buttons.Help));
                ((Button) LastControl).GumpOverID = 0x07f1;
                // OPTIONS
                AddControl(new Button(this, 0, 185, 44 + 27*1, 0x07d6, 0x07d7, ButtonTypes.Activate, 0,
                    (int) Buttons.Options));
                ((Button) LastControl).GumpOverID = 0x07d8;
                // LOG OUT
                AddControl(new Button(this, 0, 185, 44 + 27*2, 0x07d9, 0x07da, ButtonTypes.Activate, 0,
                    (int) Buttons.LogOut));
                ((Button) LastControl).GumpOverID = 0x07db;
                // QUESTS
                AddControl(new Button(this, 0, 185, 44 + 27*3, 0x57b5, 0x57b7, ButtonTypes.Activate, 0,
                    (int) Buttons.Quests));
                ((Button) LastControl).GumpOverID = 0x57b6;
                // SKILLS
                AddControl(new Button(this, 0, 185, 44 + 27*4, 0x07df, 0x07e0, ButtonTypes.Activate, 0,
                    (int) Buttons.Skills));
                ((Button) LastControl).GumpOverID = 0x07e1;
                // GUILD
                AddControl(new Button(this, 0, 185, 44 + 27*5, 0x57b2, 0x57b4, ButtonTypes.Activate, 0,
                    (int) Buttons.Guild));
                ((Button) LastControl).GumpOverID = 0x57b3;
                // PEACE / WAR
                AddControl(new Button(this, 0, 185, 44 + 27*6, 0x07e5, 0x07e6, ButtonTypes.Activate, 0,
                    (int) Buttons.PeaceWarToggle));
                ((Button) LastControl).GumpOverID = 0x07e7;
                // STATUS
                AddControl(new Button(this, 0, 185, 44 + 27*7, 0x07eb, 0x07ec, ButtonTypes.Activate, 0,
                    (int) Buttons.Status));
                ((Button) LastControl).GumpOverID = 0x07ed;

                // Paperdoll
                AddControl(new PaperDollInteractable(this, 0, 8, 21)
                {
                    SourceEntity = Parent
                });
            }
            else
            {
                AddControl(new GumpPic(this, 0, 0, 0, 0x07d1, 0));
                LastControl.MakeDragger(this);
                LastControl.MakeCloseTarget(this);

                // Paperdoll
                AddControl(new PaperDollInteractable(this, 0, 8, 21)
                {
                    SourceEntity = Parent
                });
            }


        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.Help:
                    break;
                case Buttons.Options:
                    break;
                case Buttons.LogOut:
                    MsgBox g = WorldInteraction.MsgBox("Quit Ultima Online?", MsgBoxTypes.OkCancel);
                    g.OnClose = logout_OnClose;
                    break;
                case Buttons.Quests:
                    break;
                case Buttons.Skills:
                    Engine.UserInterface.AddControl(new SkillsGump(), 80, 80, GUIManager.AddGumpType.Toggle);
                    break;
                case Buttons.Guild:
                    break;
                case Buttons.PeaceWarToggle:
                    WorldInteraction.ToggleWarMode();
                    break;
                case Buttons.Status:
                    Engine.UserInterface.AddControl(new StatusGump(), 200, 400, GUIManager.AddGumpType.Toggle);
                    break;
            }
        }

        void logout_OnClose()
        {
            WorldInteraction.DisconnectToLoginScreen();
        }

        public override bool Equals(object obj)
        {
            // base equality handles null and cannot cast to same type.
            if (!base.Equals(obj))
                return false;

            // If parameter cannot be cast to PaperDollGump return false.
            PaperDollGump p = obj as PaperDollGump;
            if (p == null)
            {
                return false;
            }

            return p.Parent.Serial == this.Parent.Serial;
        }

        public override int GetHashCode()
        {
            if (Parent == null)
            {
                return base.GetHashCode();
            }

            else return Parent.Serial;
        }
    }
}
