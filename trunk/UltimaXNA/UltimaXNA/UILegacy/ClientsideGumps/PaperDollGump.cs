using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.ClientsideGumps
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

        public PaperDollGump()
            : base(0, 0)
        {
            IsMovable = true;
            AddGumpling(new GumpPic(this, 0, 0, 0, 0x07d0, 0));
            LastGumpling.MakeADragger(this);

            // HELP
            AddGumpling(new Button(this, 0, 185, 44 + 27 * 0, 0x07ef, 0x07f0, ButtonTypes.Activate, 0, (int)Buttons.Help));
            ((Button)LastGumpling).GumpOverID = 0x07f1;
            // OPTIONS
            AddGumpling(new Button(this, 0, 185, 44 + 27 * 1, 0x07d6, 0x07d7, ButtonTypes.Activate, 0, (int)Buttons.Options));
            ((Button)LastGumpling).GumpOverID = 0x07d8;
            // LOG OUT
            AddGumpling(new Button(this, 0, 185, 44 + 27 * 2, 0x07d9, 0x07da, ButtonTypes.Activate, 0, (int)Buttons.LogOut));
            ((Button)LastGumpling).GumpOverID = 0x07db;
            // QUESTS
            AddGumpling(new Button(this, 0, 185, 44 + 27 * 3, 0x57b5, 0x57b7, ButtonTypes.Activate, 0, (int)Buttons.Quests));
            ((Button)LastGumpling).GumpOverID = 0x57b6;
            // SKILLS
            AddGumpling(new Button(this, 0, 185, 44 + 27 * 4, 0x07df, 0x07e0, ButtonTypes.Activate, 0, (int)Buttons.Skills));
            ((Button)LastGumpling).GumpOverID = 0x07e1;
            // GUILD
            AddGumpling(new Button(this, 0, 185, 44 + 27 * 5, 0x57b2, 0x57b4, ButtonTypes.Activate, 0, (int)Buttons.Guild));
            ((Button)LastGumpling).GumpOverID = 0x57b3;
            // PEACE / WAR
            AddGumpling(new Button(this, 0, 185, 44 + 27 * 6, 0x07e5, 0x07e6, ButtonTypes.Activate, 0, (int)Buttons.PeaceWarToggle));
            ((Button)LastGumpling).GumpOverID = 0x07e7;
            // STATUS
            AddGumpling(new Button(this, 0, 185, 44 + 27 * 7, 0x07eb, 0x07ec, ButtonTypes.Activate, 0, (int)Buttons.Status));
            ((Button)LastGumpling).GumpOverID = 0x07ed;

            // Paperdoll
            AddGumpling(new PaperDollInteractable(this, 0, 8, 21));
            ((PaperDollInteractable)LastGumpling).SourceEntity = Entities.EntitiesCollection.GetPlayerObject();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
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
                    MsgBox g = _manager.MsgBox("Quit Ultima Online?", MsgBoxTypes.OkCancel);
                    g.OnClose = logout_OnClose;
                    break;
                case Buttons.Quests:
                    break;
                case Buttons.Skills:
                    _manager.ToggleGump_Local(new SkillsGump(), 80, 80);
                    break;
                case Buttons.Guild:
                    break;
                case Buttons.PeaceWarToggle:
                    break;
                case Buttons.Status:
                    _manager.ToggleGump_Local(new StatusGump(), 200, 400);
                    break;
            }
        }

        void logout_OnClose()
        {
            _manager.RequestLogout();
        }
    }
}
