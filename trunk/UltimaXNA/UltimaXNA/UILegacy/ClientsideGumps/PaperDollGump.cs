﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Client;
using UltimaXNA.Entities;
using UltimaXNA.Input;
using UltimaXNA.UILegacy.Gumplings;
using UltimaXNA.Graphics;

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

        Mobile _Parent;

        public PaperDollGump(Mobile parent)
            : base(0, 0)
        {
            _Parent = parent;

            IsMovable = true;
            AddControl(new GumpPic(this, 0, 0, 0, 0x07d0, 0));
            LastControl.MakeDragger(this);
            LastControl.MakeCloseTarget(this);

            // HELP
            AddControl(new Button(this, 0, 185, 44 + 27 * 0, 0x07ef, 0x07f0, ButtonTypes.Activate, 0, (int)Buttons.Help));
            ((Button)LastControl).GumpOverID = 0x07f1;
            // OPTIONS
            AddControl(new Button(this, 0, 185, 44 + 27 * 1, 0x07d6, 0x07d7, ButtonTypes.Activate, 0, (int)Buttons.Options));
            ((Button)LastControl).GumpOverID = 0x07d8;
            // LOG OUT
            AddControl(new Button(this, 0, 185, 44 + 27 * 2, 0x07d9, 0x07da, ButtonTypes.Activate, 0, (int)Buttons.LogOut));
            ((Button)LastControl).GumpOverID = 0x07db;
            // QUESTS
            AddControl(new Button(this, 0, 185, 44 + 27 * 3, 0x57b5, 0x57b7, ButtonTypes.Activate, 0, (int)Buttons.Quests));
            ((Button)LastControl).GumpOverID = 0x57b6;
            // SKILLS
            AddControl(new Button(this, 0, 185, 44 + 27 * 4, 0x07df, 0x07e0, ButtonTypes.Activate, 0, (int)Buttons.Skills));
            ((Button)LastControl).GumpOverID = 0x07e1;
            // GUILD
            AddControl(new Button(this, 0, 185, 44 + 27 * 5, 0x57b2, 0x57b4, ButtonTypes.Activate, 0, (int)Buttons.Guild));
            ((Button)LastControl).GumpOverID = 0x57b3;
            // PEACE / WAR
            AddControl(new Button(this, 0, 185, 44 + 27 * 6, 0x07e5, 0x07e6, ButtonTypes.Activate, 0, (int)Buttons.PeaceWarToggle));
            ((Button)LastControl).GumpOverID = 0x07e7;
            // STATUS
            AddControl(new Button(this, 0, 185, 44 + 27 * 7, 0x07eb, 0x07ec, ButtonTypes.Activate, 0, (int)Buttons.Status));
            ((Button)LastControl).GumpOverID = 0x07ed;

            // Paperdoll
            AddControl(new PaperDollInteractable(this, 0, 8, 21));
            ((PaperDollInteractable)LastControl).SourceEntity = _Parent;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
                    UltimaClient.Send(new Client.Packets.Client.RequestWarModePacket(!_Parent.IsWarMode));
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
