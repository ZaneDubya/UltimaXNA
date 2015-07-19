/***************************************************************************
 *   PaperDollGump.cs
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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.Network.Client;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
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

        UserInterfaceService m_UserInterface;
        WorldModel m_World;
        INetworkClient m_Client;

        public PaperDollGump(Mobile parent)
            : base(parent.Serial, 0)
        {
            Parent = parent;

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_World = ServiceRegistry.GetService<WorldModel>();
            m_Client = ServiceRegistry.GetService<INetworkClient>();

            IsMovable = true;

            if (parent == (Mobile)WorldModel.Entities.GetPlayerObject())
            {
                AddControl(new GumpPic(this, 0, 0, 0x07d0, 0));

                // HELP
                AddControl(new Button(this, 185, 44 + 27*0, 0x07ef, 0x07f0, ButtonTypes.Activate, 0,
                    (int) Buttons.Help));
                ((Button) LastControl).GumpOverID = 0x07f1;
                // OPTIONS
                AddControl(new Button(this, 185, 44 + 27*1, 0x07d6, 0x07d7, ButtonTypes.Activate, 0,
                    (int) Buttons.Options));
                ((Button) LastControl).GumpOverID = 0x07d8;
                // LOG OUT
                AddControl(new Button(this, 185, 44 + 27*2, 0x07d9, 0x07da, ButtonTypes.Activate, 0,
                    (int) Buttons.LogOut));
                ((Button) LastControl).GumpOverID = 0x07db;
                // QUESTS
                AddControl(new Button(this, 185, 44 + 27*3, 0x57b5, 0x57b7, ButtonTypes.Activate, 0,
                    (int) Buttons.Quests));
                ((Button) LastControl).GumpOverID = 0x57b6;
                // SKILLS
                AddControl(new Button(this, 185, 44 + 27*4, 0x07df, 0x07e0, ButtonTypes.Activate, 0,
                    (int) Buttons.Skills));
                ((Button) LastControl).GumpOverID = 0x07e1;
                // GUILD
                AddControl(new Button(this, 185, 44 + 27*5, 0x57b2, 0x57b4, ButtonTypes.Activate, 0,
                    (int) Buttons.Guild));
                ((Button) LastControl).GumpOverID = 0x57b3;
                // PEACE / WAR
                AddControl(new Button(this, 185, 44 + 27*6, 0x07e5, 0x07e6, ButtonTypes.Activate, 0,
                    (int) Buttons.PeaceWarToggle));
                ((Button) LastControl).GumpOverID = 0x07e7;
                // STATUS
                AddControl(new Button(this, 185, 44 + 27*7, 0x07eb, 0x07ec, ButtonTypes.Activate, 0,
                    (int) Buttons.Status));
                ((Button) LastControl).GumpOverID = 0x07ed;

                // Paperdoll
                AddControl(new PaperDollInteractable(this, 8, 21)
                {
                    SourceEntity = Parent
                });
            }
            else
            {
                AddControl(new GumpPic(this, 0, 0, 0x07d1, 0));

                // Paperdoll
                AddControl(new PaperDollInteractable(this, 8, 21)
                {
                    SourceEntity = Parent
                });
            }
        }

        protected override void OnInitialize()
        {
            SetSavePositionName(Parent.IsClientEntity ? "paperdoll_self" : "paperdoll");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.Help:
                    m_Client.Send(new RequestHelpPacket());
                    break;
                case Buttons.Options:
                    if (m_UserInterface.GetControl<OptionsGump>() == null)
                        m_UserInterface.AddControl(new OptionsGump(), 80, 80);
                    else
                        m_UserInterface.RemoveControl<OptionsGump>();
                    break;
                case Buttons.LogOut:
                    MsgBoxGump g = MsgBoxGump.Show("Quit Ultima Online?", MsgBoxTypes.OkCancel);
                    g.OnClose = logout_OnClose;
                    break;
                case Buttons.Quests:
                    break;
                case Buttons.Skills:
                    if (m_UserInterface.GetControl<SkillsGump>() == null)
                        m_UserInterface.AddControl(new SkillsGump(), 80, 80);
                    else
                        m_UserInterface.RemoveControl<SkillsGump>();
                    break;
                case Buttons.Guild:
                    break;
                case Buttons.PeaceWarToggle:
                    m_World.Interaction.ToggleWarMode();
                    break;
                case Buttons.Status:
                    if (m_UserInterface.GetControl<StatusGump>() == null)
                        m_UserInterface.AddControl(new StatusGump(), 200, 400);
                    else
                        m_UserInterface.RemoveControl<StatusGump>();
                    break;
            }
        }

        void logout_OnClose()
        {
            m_World.Disconnect();
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
