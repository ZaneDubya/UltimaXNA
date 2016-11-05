using System.Collections.Generic;
using System.Linq;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Network.Client.PartySystem;
using UltimaXNA.Ultima.Network.Server.GeneralInfo;
using UltimaXNA.Ultima.UI.WorldGumps;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Player.Partying
{
    public class PartySystem
    {
        Serial m_LeaderSerial;
        List<PartyMember> m_PartyMembers = new List<PartyMember>();
        PartyState m_State;

        public Mobile Leader => m_PartyMembers.Find(p => p.IsLeader == true).Player;
        public List<Mobile> List => m_PartyMembers.Select(p => p.Player).ToList();
        public PartyMember Self => m_PartyMembers[SelfIndex];
        public int SelfIndex => m_PartyMembers.FindIndex(p => p.Player == WorldModel.Entities.GetPlayerEntity());

        public PartyState Status
        {
            set
            {
                m_State = value;
            }
            get
            {
                //is he leader ?? and is he has a party ?
                if (m_State == PartyState.Joined && m_PartyMembers.Find(p => p.Player == WorldModel.Entities.GetPlayerEntity()) == null)
                    LeaveParty();
                else if (m_State == PartyState.Joined && m_PartyMembers.Find(p => p.Player == WorldModel.Entities.GetPlayerEntity()).IsLeader)
                    return PartyState.Leader;//he has full access
                return m_State;
            }
        }

        public void ReceivePartyMemberList(PartyMemberListInfo info)
        {
            Status = PartyState.Joined;
            m_PartyMembers.Clear();
            for (int i = 0; i < info.Count; i++)
                AddMember(info.Serials[i], false);
            RefreshPartyStatusBar();
        }

        public void ReceiveRemovePartyMember(PartyRemoveMemberInfo info)
        {
            Status = PartyState.Joined;
            m_PartyMembers.Clear();
            for (int i = 0; i < info.Count; i++)
                AddMember(info.Serials[i], false);
            RefreshPartyStatusBar();
        }

        public void ReceiveInvitation(PartyInvitationInfo info)
        {
            Status = PartyState.Joining;
            AddMember(info.PartyLeaderSerial, true);
        }

        public void AddMember(Serial serial, bool isleader)
        {
            if (serial == m_LeaderSerial)//after refresh list we don't know who is leader
                isleader = true;
            int index = m_PartyMembers.FindIndex(p => p.Player.Serial == serial);//if already in ?
            if (index != -1)//remove and add new member (refreshing ?)
                m_PartyMembers.RemoveAt(index);
            m_PartyMembers.Add(new PartyMember(serial, isleader));
            if (isleader)//first add leader
                m_LeaderSerial = serial;
        }

        public PartyMember GetMember(int index)
        {
            if (index >= 0 && index < m_PartyMembers.Count)
                return m_PartyMembers[index];
            return null;
        }

        public PartyMember GetMember(Serial serial)
        {
            return m_PartyMembers.Find(p => p.Player.Serial == serial);
        }

        public void LeaveParty()
        {
            INetworkClient m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_Network.Send(new PartyLeavePacket());
            Status = PartyState.None;
            m_PartyMembers.Clear();
            m_LeaderSerial = 0;
            UserInterfaceService m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_UserInterface.RemoveControl<PartyHealthTrackerGump>();
        }

        public void PartyStateControl(string text, int hue)
        {
            INetworkClient m_Network = ServiceRegistry.GetService<INetworkClient>();
            WorldModel m_world = ServiceRegistry.GetService<WorldModel>();
            PartyCommand PCmd = new PartyCommand(text);//controlling command
            switch (Status)
            {
                case PartyState.None:
                    if (PCmd.PrimaryCmd == PartyCommandType.Add)//add member
                    {
                        Status = PartyState.Joining;
                        AddMember(WorldModel.Entities.GetPlayerEntity().Serial, true);
                        m_Network.Send(new PartyAddMemberPacket());
                    }
                    else if (PCmd.PrimaryCmd == PartyCommandType.HelpMenu)
                        ShowPartyHelp();
                    else
                        m_world.Interaction.ChatMessage("Wrong command you don't have a party. Please use /hlp", 3, 10, false);
                    break;

                case PartyState.Joining:
                    if (PCmd.PrimaryCmd == PartyCommandType.Add)//add member
                    {
                        LeaveParty();
                        Status = PartyState.Joining;
                        AddMember(WorldModel.Entities.GetPlayerEntity().Serial, true);
                        m_Network.Send(new PartyAddMemberPacket());
                        return;
                    }
                    else if (PCmd.PrimaryCmd == PartyCommandType.Accept && Leader != WorldModel.Entities.GetPlayerEntity())//accept party
                    {
                        m_Network.Send(new PartyAcceptPacket(Leader));
                        return;
                    }
                    else if (PCmd.PrimaryCmd == PartyCommandType.Quit)
                    {
                        LeaveParty();
                        return;
                    }
                    else if (PCmd.PrimaryCmd == PartyCommandType.Decline && Leader != WorldModel.Entities.GetPlayerEntity())//decline decline party
                    {
                        m_Network.Send(new PartyDeclinePacket(Leader));
                        return;
                    }
                    else if (PCmd.PrimaryCmd == PartyCommandType.HelpMenu)
                        ShowPartyHelp();
                    else
                        LeaveParty();
                    m_world.Interaction.ChatMessage("Wrong command. You can use '/accept' or '/decline'.", 3, 10, false);
                    break;

                case PartyState.Leader:
                case PartyState.Joined:
                    if (PCmd.PrimaryCmd == PartyCommandType.Add && Leader == WorldModel.Entities.GetPlayerEntity()) //he is party leader
                    {
                        m_Network.Send(new PartyAddMemberPacket());
                    }
                    else if (PCmd.PrimaryCmd == PartyCommandType.Remove && PCmd.SecondaryCmd != "unknowncmd")
                    {
                        int index1 = int.Parse(PCmd.SecondaryCmd);
                        if (GetMember(index1) != null)
                            m_Network.Send(new PartyRemoveMemberPacket(index1));//wrong packet how can send a serial ?
                        else
                            m_world.Interaction.ChatMessage("Wrong party index. please first use '/hlp'.", 3, 10, false);
                    }
                    else if (PCmd.PrimaryCmd == PartyCommandType.HelpMenu)
                    {
                        ShowPartyHelp();
                    }
                    else if (PCmd.PrimaryCmd == PartyCommandType.List)
                    {
                        for (int i = 0; i < m_PartyMembers.Count; i++)
                        {
                            if (m_PartyMembers[i].IsLeader)
                                m_world.Interaction.ChatMessage(string.Format("[{0}: {1}][LEADER]", i.ToString(), m_PartyMembers[i].Player.Name), 3, 53, true);
                            else
                                m_world.Interaction.ChatMessage(string.Format("[{0}: {1}]", i.ToString(), m_PartyMembers[i].Player.Name), 3, 55, true);
                        }
                    }
                    else if (PCmd.PrimaryCmd == PartyCommandType.Quit)
                    {
                        LeaveParty();
                    }
                    else if (PCmd.PrimaryCmd == PartyCommandType.Loot)
                    {
                        if (PCmd.SecondaryCmd == "on")
                            m_Network.Send(new PartyCanLootPacket(true));
                        else if (PCmd.SecondaryCmd == "off")
                            m_Network.Send(new PartyCanLootPacket(false));
                        else
                            m_world.Interaction.ChatMessage("Wrong command. Please use '/loot on' or '/loot off'.", 3, 10, false);
                    }
                    else if (PCmd.PrimaryCmd == PartyCommandType.Public)
                    {
                        m_Network.Send(new PartyPublicMessagePacket(PCmd.PlayerMessage));
                    }
                    break;

                default:
                    m_world.Interaction.ChatMessage("PARTY SYSTEM ERROR !!!", 3, 30, false);
                    break;
            }
        }

        public void RefreshPartyStatusBar()
        {
            UserInterfaceService m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_UserInterface.RemoveControl<PartyHealthTrackerGump>();
            for (int i = 0; i < List.Count; i++)
                m_UserInterface.AddControl(new PartyHealthTrackerGump(List[i].Serial), 5, 40 + (48 * i));

            if (m_UserInterface.GetControl<PartyGump>() != null)
            {
                m_UserInterface.RemoveControl<PartyGump>();
                m_UserInterface.AddControl(new PartyGump(), 200, 40);
            }
        }

        public void RemoveMember(Serial serial)
        {
            int index = m_PartyMembers.FindIndex(p => p.Player.Serial == serial);
            if (index != -1)
            {
                m_PartyMembers.RemoveAt(index);
            }
        }

        public void ShowPartyHelp()
        {
            WorldModel m_world = ServiceRegistry.GetService<WorldModel>();
            m_world.Interaction.ChatMessage("/add                       - add a new member or create a party", 3, 51, true);
            m_world.Interaction.ChatMessage("/rem {PartyIndex}          - party member who is dispanded from party", 3, 51, true);
            m_world.Interaction.ChatMessage("/accept                    - joining a party", 3, 51, true);
            m_world.Interaction.ChatMessage("/decline                   - rejecting a party", 3, 51, true);
            m_world.Interaction.ChatMessage("/list                      - party member list", 3, 51, true);
            m_world.Interaction.ChatMessage("/loot {on/off}             - party members can loot to you", 3, 51, true);
            m_world.Interaction.ChatMessage("/{message}                 - public party message", 3, 51, true);
            //m_world.Interaction.ChatMessage("/trg                       - mark an enemy (leader command)", 3, 51, true);//new
        }
    }
}