/***************************************************************************
 *   MobileHealthTrackerGump.cs
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
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Network.Client;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class MobileHealthTrackerGump : Gump
    {
        public Mobile Mobile
        {
            get;
            private set;
        }

        private GumpPic m_Background;
        private GumpPicWithWidth[] m_Bars;
        private GumpPic[] m_BarBGs;
        private TextEntry m_NameEntry;
        private readonly WorldModel m_World;
        private INetworkClient m_Network;

        public MobileHealthTrackerGump(Mobile mobile)
            : base(mobile.Serial, 0)
        {
            while (UserInterface.GetControl<MobileHealthTrackerGump>(mobile.Serial) != null)
            {
                UserInterface.GetControl<MobileHealthTrackerGump>(mobile.Serial).Dispose();
            }

            IsMoveable = true;

            Mobile = mobile;
            m_World = ServiceRegistry.GetService<WorldModel>();
            m_Network = ServiceRegistry.GetService<INetworkClient>();

            if (Mobile.IsClientEntity)
            {
                AddControl(m_Background = new GumpPic(this, 0, 0, 0x0803, 0));
                m_Background.MouseDoubleClickEvent += Background_MouseDoubleClickEvent;
                m_BarBGs = new GumpPic[3];
                AddControl(m_BarBGs[0] = new GumpPic(this, 34, 10, 0x0805, 0));
                AddControl(m_BarBGs[1] = new GumpPic(this, 34, 24, 0x0805, 0));
                AddControl(m_BarBGs[2] = new GumpPic(this, 34, 38, 0x0805, 0));
                m_Bars = new GumpPicWithWidth[3];
                AddControl(m_Bars[0] = new GumpPicWithWidth(this, 34, 10, 0x0806, 0, 1f));
                AddControl(m_Bars[1] = new GumpPicWithWidth(this, 34, 24, 0x0806, 0, 1f));
                AddControl(m_Bars[2] = new GumpPicWithWidth(this, 34, 38, 0x0806, 0, 1f));
            }
            else
            {
                AddControl(m_Background = new GumpPic(this, 0, 0, 0x0804, 0));
                m_Background.MouseDoubleClickEvent += Background_MouseDoubleClickEvent;
                m_BarBGs = new GumpPic[1];
                AddControl(m_BarBGs[0] = new GumpPic(this, 34, 38, 0x0805, 0));
                m_Bars = new GumpPicWithWidth[1];
                AddControl(m_Bars[0] = new GumpPicWithWidth(this, 34, 38, 0x0806, 0, 1f));
                AddControl(m_NameEntry = new TextEntry(this, 17, 16, 124, 20, 0, 0, 99, mobile.Name));
                SetupMobileNameEntry();
            }

            // bars should not handle mouse input, pass it to the background gump.
            for (int i = 0; i < m_BarBGs.Length; i++)
            {
                m_BarBGs[i].HandlesMouseInput = false;
                m_Bars[i].HandlesMouseInput = false;
            }
        }

        public override void Dispose()
        {
            m_Background.MouseDoubleClickEvent -= Background_MouseDoubleClickEvent;
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            m_Bars[0].PercentWidthDrawn = ((float)Mobile.Health.Current / Mobile.Health.Max);
            if (Mobile.Flags.IsBlessed)
                m_Bars[0].GumpID = 0x0809;
            else if (Mobile.Flags.IsPoisoned)
                m_Bars[0].GumpID = 0x0808;

            if (Mobile.IsClientEntity)
            {
                if (Mobile.Flags.IsWarMode)
                    m_Background.GumpID = 0x0807;
                else
                    m_Background.GumpID = 0x0803;
                m_Bars[1].PercentWidthDrawn = ((float)Mobile.Stamina.Current / Mobile.Stamina.Max);
                m_Bars[2].PercentWidthDrawn = ((float)Mobile.Mana.Current / Mobile.Mana.Max);
            }
            else
            {
                // this doesn't change anything, but might as well leave it in incase we do want to change the graphic
                // based on some future condition.
                m_Background.GumpID = 0x0804;
                if (Mobile.PlayerCanChangeName != m_NameEntry.IsEditable)
                    SetupMobileNameEntry();
            }

            base.Update(totalMS, frameMS);
        }

        private void Background_MouseDoubleClickEvent(AControl caller, int x, int y, MouseButton button)
        {
            if (Mobile.IsClientEntity)
            {
                StatusGump.Toggle(Mobile.Serial);
            }
            else
            {
                m_World.Interaction.LastTarget = Mobile.Serial;

                if (WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode)
                {
                    // Attack Reds and Greys
                    if (Mobile.Notoriety == 0x3 || Mobile.Notoriety == 0x4 || Mobile.Notoriety == 0x5 || Mobile.Notoriety == 0x6)
                    {
                        m_Network.Send(new AttackRequestPacket(Mobile.Serial));
                    }
                    // CrimeQuery is enabled, ask before attacking others
                    else if (Settings.UserInterface.CrimeQuery)
                    {
                        MsgBoxGump g = MsgBoxGump.Show("This may flag you criminal!", MsgBoxTypes.OkCancel);
                        g.OnClose = OnCloseCrimeQueryMsgBox;
                    }
                    // CrimeQuery is disabled, so attack without asking
                    else
                    {
                        m_Network.Send(new AttackRequestPacket(Mobile.Serial));
                    }
                }
                //Open
                else
                {
                    m_World.Interaction.DoubleClick(Mobile);
                }
            }
        }

        void OnCloseCrimeQueryMsgBox()
        {
            m_Network.Send(new AttackRequestPacket(Mobile.Serial));
        }

        private void SetupMobileNameEntry()
        {
            if (Mobile.PlayerCanChangeName)
            {
                m_NameEntry.IsEditable = true;
                m_NameEntry.LeadingHtmlTag = "<span color='#808' style='font-family:uni0;'>";
            }
            else
            {
                m_NameEntry.IsEditable = false;
                m_NameEntry.LeadingHtmlTag = "<span color='#444' style='font-family:uni0;'>";
            }
        }

        public override void OnKeyboardReturn(int textID, string text)
        {
            INetworkClient client = ServiceRegistry.GetService<INetworkClient>();
            client.Send(new RenameCharacterPacket(Mobile.Serial, text));
        }
    }
}
