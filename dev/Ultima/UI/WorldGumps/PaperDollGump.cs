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
using System.Collections.Generic;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Core.Input.Windows;
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

        public Mobile Mobile
        {
            get;
            private set;
        }

        UserInterfaceService m_UserInterface;
        WorldModel m_World;
        INetworkClient m_Client;

        private bool m_IsWarMode;
        private Button m_WarModeBtn;

        private readonly int[] PeaceModeBtnGumps = new int[] { 0x07e5, 0x07e6, 0x07e7 };
        private readonly int[] WarModeBtnGumps = new int[] { 0x07e8, 0x07e9, 0x07ea };

        public PaperDollGump()
            : base(0, 0)
        {
            
        }

        public PaperDollGump(Serial serial)
            : this()
        {
            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(serial, false);
            if (mobile != null)
            {
                Mobile = mobile;
                BuildGump();
            }
        }

        private void BuildGump()
        {
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_World = ServiceRegistry.GetService<WorldModel>();
            m_Client = ServiceRegistry.GetService<INetworkClient>();

            IsMoveable = true;
            SaveOnWorldStop = true;
            GumpLocalID = Mobile.Serial;

            if (Mobile.IsClientEntity)
            {
                AddControl(new GumpPic(this, 0, 0, 0x07d0, 0));

                // HELP
                AddControl(new Button(this, 185, 44 + 27 * 0, 0x07ef, 0x07f0, ButtonTypes.Activate, 0,
                    (int)Buttons.Help));
                ((Button)LastControl).GumpOverID = 0x07f1;
                // OPTIONS
                AddControl(new Button(this, 185, 44 + 27 * 1, 0x07d6, 0x07d7, ButtonTypes.Activate, 0,
                    (int)Buttons.Options));
                ((Button)LastControl).GumpOverID = 0x07d8;
                // LOG OUT
                AddControl(new Button(this, 185, 44 + 27 * 2, 0x07d9, 0x07da, ButtonTypes.Activate, 0,
                    (int)Buttons.LogOut));
                ((Button)LastControl).GumpOverID = 0x07db;
                // QUESTS
                AddControl(new Button(this, 185, 44 + 27 * 3, 0x57b5, 0x57b7, ButtonTypes.Activate, 0,
                    (int)Buttons.Quests));
                ((Button)LastControl).GumpOverID = 0x57b6;
                // SKILLS
                AddControl(new Button(this, 185, 44 + 27 * 4, 0x07df, 0x07e0, ButtonTypes.Activate, 0,
                    (int)Buttons.Skills));
                ((Button)LastControl).GumpOverID = 0x07e1;
                // GUILD
                AddControl(new Button(this, 185, 44 + 27 * 5, 0x57b2, 0x57b4, ButtonTypes.Activate, 0,
                    (int)Buttons.Guild));
                ((Button)LastControl).GumpOverID = 0x57b3;
                // PEACE / WAR
                m_IsWarMode = Mobile.Flags.IsWarMode;
                int[] btngumps = m_IsWarMode ? WarModeBtnGumps : PeaceModeBtnGumps;
                m_WarModeBtn = (Button)AddControl(new Button(this, 185, 44 + 27 * 6, btngumps[0], btngumps[1], ButtonTypes.Activate, 0,
                    (int)Buttons.PeaceWarToggle));
                ((Button)LastControl).GumpOverID = btngumps[2];
                // STATUS
                AddControl(new Button(this, 185, 44 + 27 * 7, 0x07eb, 0x07ec, ButtonTypes.Activate, 0,
                    (int)Buttons.Status));
                ((Button)LastControl).GumpOverID = 0x07ed;

                // Virtue menu
                AddControl(new GumpPic(this, 80, 8, 0x0071, 0));
                LastControl.MouseDoubleClickEvent += VirtueMenu_MouseDoubleClickEvent;

                // Special moves book
                // AddControl(new GumpPic(this, 158, 200, 0x2B34, 0));
                // LastControl.MouseDoubleClickEvent += SpecialMoves_MouseDoubleClickEvent;

                // equipment slots for hat/earrings/neck/ring/bracelet
                AddControl(new EquipmentSlot(this, 2, 76, Mobile, EquipLayer.Helm));
                AddControl(new EquipmentSlot(this, 2, 76 + 22 * 1, Mobile, EquipLayer.Earrings));
                AddControl(new EquipmentSlot(this, 2, 76 + 22 * 2, Mobile, EquipLayer.Neck));
                AddControl(new EquipmentSlot(this, 2, 76 + 22 * 3, Mobile, EquipLayer.Ring));
                AddControl(new EquipmentSlot(this, 2, 76 + 22 * 4, Mobile, EquipLayer.Bracelet));

                // Paperdoll control!
                AddControl(new PaperDollInteractable(this, 8, 21)
                {
                    SourceEntity = Mobile
                });
            }
            else
            {
                AddControl(new GumpPic(this, 0, 0, 0x07d1, 0));

                // Paperdoll
                AddControl(new PaperDollInteractable(this, 8, 21)
                {
                    SourceEntity = Mobile
                });
            }

            // name and title
            AddControl(new HtmlGumpling(this, 36, 262, 180, 42, 0, 0, string.Format("<span color=#aaa style='font-family:uni0;'>{0}", Mobile.Name)));
            AddControl(new HtmlGumpling(this, 35, 262, 180, 42, 0, 0, string.Format("<span color=#222 style='font-family:uni0;'>{0}", Mobile.Name)));
        }

        private void SpecialMoves_MouseDoubleClickEvent(AControl control, int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                // open special moves book.
            }
        }

        private void VirtueMenu_MouseDoubleClickEvent(AControl control, int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                m_Client.Send(new GumpMenuSelectPacket(Mobile.Serial, 0x000001CD, 0x00000001, new int[1] { Mobile.Serial }, null));
        }

        protected override void OnInitialize()
        {
            SetSavePositionName(Mobile.IsClientEntity ? "paperdoll_self" : "paperdoll");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (Mobile != null && Mobile.IsDisposed)
                Mobile = null;
            if (Mobile == null)
            {
                Dispose();
                return;
            }

            // switch the graphics on the peace/war btn if this is the player entity and warmode flag has changed.
            if (Mobile.IsClientEntity && m_IsWarMode != Mobile.Flags.IsWarMode)
            {
                m_IsWarMode = Mobile.Flags.IsWarMode;
                int[] btngumps = m_IsWarMode ? WarModeBtnGumps : PeaceModeBtnGumps;
                m_WarModeBtn.GumpUpID = btngumps[0];
                m_WarModeBtn.GumpDownID = btngumps[1];
                m_WarModeBtn.GumpOverID = btngumps[2];
            }

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
                    // MsgBoxGump g = MsgBoxGump.Show("Quit Ultima Online?", MsgBoxTypes.OkCancel);
                    // g.OnClose = logout_OnClose;
                    m_UserInterface.AddControl(new LogoutGump(), 0, 0);
                    break;
                case Buttons.Quests:
                    m_Client.Send(new QuestGumpRequestPacket(Mobile.Serial));
                    break;
                case Buttons.Skills:
                    m_Client.Send(new GetPlayerStatusPacket(0x05, Mobile.Serial));
                    if (m_UserInterface.GetControl<SkillsGump>() == null)
                        m_UserInterface.AddControl(new SkillsGump(), 80, 80);
                    else
                        m_UserInterface.RemoveControl<SkillsGump>();
                    break;
                case Buttons.Guild:
                    m_Client.Send(new GuildGumpRequestPacket(Mobile.Serial));
                    break;
                case Buttons.PeaceWarToggle:
                    m_World.Interaction.ToggleWarMode();
                    break;
                case Buttons.Status:
                    if (m_UserInterface.GetControl<StatusGump>() == null)
                    {
                        m_Client.Send(new GetPlayerStatusPacket(0x04, Mobile.Serial));
                        m_UserInterface.AddControl(new StatusGump(), 200, 400);
                    }
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
            if (Mobile == null)
                return base.GetHashCode();
            else
                return Mobile.Serial;
        }

        public override bool SaveGump(out Dictionary<string, object> data)
        {
            data = new Dictionary<string, object>();
            data.Add("serial", (int)Mobile.Serial);
            return true;
        }

        public override bool RestoreGump(Dictionary<string, object> data)
        {
            int serial;
            if (data.ContainsKey("serial"))
            {
                serial = (int)data["serial"];
                Mobile mobile = WorldModel.Entities.GetObject<Mobile>(serial, false);
                if (mobile != null)
                {
                    Mobile = mobile;
                    BuildGump();
                    return true;
                }
            }
            return false;
        }
    }
}
