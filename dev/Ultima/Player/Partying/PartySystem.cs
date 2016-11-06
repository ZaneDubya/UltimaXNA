/***************************************************************************
 *   PartySystem.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.Network.Client.Partying;
using UltimaXNA.Ultima.Network.Server.GeneralInfo;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.WorldGumps;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.Player.Partying
{
    public class PartySystem
    {
        Serial m_LeaderSerial;
        Serial m_InvitingPartyLeader;
        List<PartyMember> m_PartyMembers = new List<PartyMember>();
        bool m_AllowPartyLoot;

        public Serial LeaderSerial => m_LeaderSerial;
        public List<PartyMember> Members => m_PartyMembers;
        public bool InParty => m_PartyMembers.Count > 1;

        public bool AllowPartyLoot
        {
            get
            {
                return m_AllowPartyLoot;
            }
            set
            {
                m_AllowPartyLoot = value;
                INetworkClient network = ServiceRegistry.GetService<INetworkClient>();
                network.Send(new PartyCanLootPacket(m_AllowPartyLoot));
            }
        }

        public void ReceivePartyMemberList(PartyMemberListInfo info)
        {
            m_PartyMembers.Clear();
            for (int i = 0; i < info.Count; i++)
                AddMember(info.Serials[i]);
            RefreshPartyGumps();
        }

        public void ReceiveRemovePartyMember(PartyRemoveMemberInfo info)
        {
            m_PartyMembers.Clear();
            for (int i = 0; i < info.Count; i++)
                AddMember(info.Serials[i]);
            RefreshPartyGumps();
        }

        public void ReceiveInvitation(PartyInvitationInfo info)
        {
            m_InvitingPartyLeader = info.PartyLeaderSerial;
        }

        public void AddMember(Serial serial)
        {
            int index = m_PartyMembers.FindIndex(p => p.Mobile.Serial == serial);
            if (index != -1)
            {
                m_PartyMembers.RemoveAt(index);
            }
            m_PartyMembers.Add(new PartyMember(serial));
            INetworkClient network = ServiceRegistry.GetService<INetworkClient>();
            network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, serial));
        }

        public PartyMember GetMember(int index)
        {
            if (index >= 0 && index < m_PartyMembers.Count)
                return m_PartyMembers[index];
            return null;
        }

        public PartyMember GetMember(Serial serial)
        {
            return m_PartyMembers.Find(p => p.Mobile.Serial == serial);
        }

        public void LeaveParty()
        {
            INetworkClient network = ServiceRegistry.GetService<INetworkClient>();
            network.Send(new PartyLeavePacket());
            m_PartyMembers.Clear();
            m_LeaderSerial = Serial.Null;
            UserInterfaceService ui = ServiceRegistry.GetService<UserInterfaceService>();
            ui.RemoveControl<PartyHealthTrackerGump>();
        }

        public void DoPartyCommand(string text)
        {
            // I do this a little differently than the legacy client. With legacy, if you type "/add this other player,
            // please ?" the client will detect the first word is "add" and request an add player target. Instead, I
            // interpret this as a message, and send the message "add this other player, please?" as a party message.
            INetworkClient network = ServiceRegistry.GetService<INetworkClient>();
            WorldModel world = ServiceRegistry.GetService<WorldModel>();
            string command = text.ToLower();
            bool playerInParty = PlayerState.Partying.Members.Count > 1;
            bool playerIsLeader = PlayerState.Partying.LeaderSerial == WorldModel.PlayerSerial;
            bool commandHandled = false;
            switch (command)
            {
                case "help":
                    ShowPartyHelp();
                    commandHandled = true;
                    break;
                case "add":
                    if (!playerInParty)
                    {
                        world.Interaction.ChatMessage("Who would you like to add to your party?", 3, 10, false);
                        m_LeaderSerial = WorldModel.PlayerSerial;
                        network.Send(new PartyRequestAddTargetPacket());
                    }
                    else if (playerInParty && playerIsLeader)
                    {
                        world.Interaction.ChatMessage("Who would you like to add to your party?", 3, 10, false);
                        network.Send(new PartyRequestAddTargetPacket());
                    }
                    else if (playerInParty && !playerIsLeader)
                    {
                        world.Interaction.ChatMessage("You may only add members to the party if you are the leader.", 3, 10, false);
                    }
                    commandHandled = true;
                    break;
                case "rem":
                case "remove":
                    if (playerInParty && playerIsLeader)
                    {
                        world.Interaction.ChatMessage("Who would you like to remove from your party?", 3, 10, false);
                        network.Send(new PartyRequestRemoveTargetPacket());
                    }
                    commandHandled = true;
                    break;
                case "accept":
                    if (!playerInParty && m_InvitingPartyLeader.IsValid)
                    {
                        network.Send(new PartyAcceptPacket(m_InvitingPartyLeader));
                        m_LeaderSerial = m_InvitingPartyLeader;
                        m_InvitingPartyLeader = Serial.Null;
                    }
                    commandHandled = true;
                    break;
                case "decline":
                    if (!playerInParty && m_InvitingPartyLeader.IsValid)
                    {
                        network.Send(new PartyDeclinePacket(m_InvitingPartyLeader));
                        m_InvitingPartyLeader = Serial.Null;
                    }
                    commandHandled = true;
                    break;
                case "quit":
                    LeaveParty();
                    commandHandled = true;
                    break;
            }
            if (!commandHandled)
            {
                network.Send(new PartyPublicMessagePacket(text));
            }
        }

        internal void SetPartyTarget()
        {
            INetworkClient network = ServiceRegistry.GetService<INetworkClient>();
            network.Send(new PartyPublicMessagePacket("All member attack to ---PLAYERNAME---"));//need improve
        }

        internal void RequestAddPartyMemberTarget()
        {
            INetworkClient network = ServiceRegistry.GetService<INetworkClient>();
            network.Send(new PartyRequestAddTargetPacket());
        }

        internal void SendTell(Serial serial)
        {
            INetworkClient network = ServiceRegistry.GetService<INetworkClient>();
            network.Send(new PartyPrivateMessagePacket(serial, "make a dynamic message type"));//need improve
        }

        public void RefreshPartyGumps()
        {
            UserInterfaceService ui = ServiceRegistry.GetService<UserInterfaceService>();
            ui.RemoveControl<PartyHealthTrackerGump>();
            for (int i = 0; i < Members.Count; i++)
            {
                ui.AddControl(new PartyHealthTrackerGump(Members[i].Serial), 5, 40 + (48 * i));
            }
            Gump gump;
            if ((gump = ui.GetControl<PartyGump>()) != null)
            {
                int x = gump.X;
                int y = gump.Y;
                ui.RemoveControl<PartyGump>();
                ui.AddControl(new PartyGump(), x, y);
            }
        }

        public void RemoveMember(Serial serial)
        {
            INetworkClient network = ServiceRegistry.GetService<INetworkClient>();
            network.Send(new PartyRemoveMemberPacket(serial));
            int index = m_PartyMembers.FindIndex(p => p.Mobile.Serial == serial);
            if (index != -1)
            {
                m_PartyMembers.RemoveAt(index);
            }
        }

        public void ShowPartyHelp()
        {
            WorldModel m_world = ServiceRegistry.GetService<WorldModel>();
            m_world.Interaction.ChatMessage("/add       - add a new member or create a party", 3, 51, true);
            m_world.Interaction.ChatMessage("/rem       - kick a member from your party", 3, 51, true);
            m_world.Interaction.ChatMessage("/accept    - join a party", 3, 51, true);
            m_world.Interaction.ChatMessage("/decline   - decline a party invitation", 3, 51, true);
            m_world.Interaction.ChatMessage("/quit      - leave your current party", 3, 51, true);
        }
    }
}