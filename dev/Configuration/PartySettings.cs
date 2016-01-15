using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.UI.WorldGumps;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Configuration
{
    public class PartySettings
    {
        public class PartyMember
        {
            public Mobile Player { get; set; }
            public bool isLeader { get; set; }
            public bool isLootable { get; set; } //only client Entity
            public PartyMember(Serial _serial, bool _isleader)
            {
                isLeader = _isleader;
                Player = WorldModel.Entities.GetObject<Mobile>(_serial, false);
                INetworkClient m_Network = ServiceRegistry.GetService<INetworkClient>();
                m_Network.Send(new PartyQueryStats(_serial));//I THINK CHECK FOR STATUS
            }
        }

        public enum PCommandType //Control text state
        {
            Add,
            [DefaultValue("rem")]
            Remove,
            Accept,
            Decline,
            Quit,
            Loot, // (on-off) control is secondary command
            List,
            Public,
            [DefaultValue("hlp")]
            HelpMenu,//list all party member with index number for private message
            Unknown,
        }
        public class PartyCommand
        {
            private PCommandType _CType;
            public PCommandType PrimaryCmd { get { return _CType; } set { _CType = value; } }

            private string _secondarycmd;//for example: UserName which is adding or deleting
            public string SecondaryCmd { get { return _secondarycmd; } set { _secondarycmd = value; } }

            private string _msg;
            public string PlayerMessage { get { return _msg; } set { _msg = value; } }

            public PartyCommand(string _text)
            {
                string[] cmds = _text.Split(new string[] { " " }, StringSplitOptions.None);
                if (_text.ToLower() == "add")
                {
                    PrimaryCmd = PCommandType.Add;
                }
                else if (cmds[0].ToLower() == "rem")
                {
                    PrimaryCmd = PCommandType.Remove;
                    int number1 = 0;
                    bool isInt1 = int.TryParse(cmds[1], out number1);
                    if (isInt1)
                        SecondaryCmd = number1.ToString();
                    else
                        SecondaryCmd = "unknowncmd";
                }
                else if (_text.ToLower() == "accept")
                {
                    PrimaryCmd = PCommandType.Accept;
                }
                else if (_text.ToLower() == "decline")
                {
                    PrimaryCmd = PCommandType.Decline;
                }
                else if (_text.ToLower() == "quit")
                {
                    PrimaryCmd = PCommandType.Quit;
                }
                else if (cmds[0].ToLower() == "loot")
                {
                    PrimaryCmd = PCommandType.Loot;
                    if (cmds.Length > 1)
                    {
                        if (cmds[1].ToLower() == "on")
                            SecondaryCmd = "on";
                        else if (cmds[1].ToLower() == "off")
                            SecondaryCmd = "off";
                        return;
                    }
                    SecondaryCmd = "unknowncmd";
                }
                else if (_text.ToLower() == "list")//list of party members (new)
                {
                    PrimaryCmd = PCommandType.List;
                }
                else if (_text.ToLower() == "hlp")//about of party commands (new)
                {
                    PrimaryCmd = PCommandType.HelpMenu;
                }
                else
                {
                    //public party message
                    PrimaryCmd = PCommandType.Public;
                    PlayerMessage = _text;
                    return;
                }
            }
        }
        public enum PartyState //controlling party state
        {
            None = 0,
            Joining = 1,
            Joined = 2,
            Leader = 3 //special status for invite and remove control
        }

        private static PartyState m_State;
        private static List<PartyMember> m_PartyMembers = new List<PartyMember>();
        private static Serial leaderSerial;
        public static PartyState Status
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
                else if (m_State == PartyState.Joined && m_PartyMembers.Find(p => p.Player == WorldModel.Entities.GetPlayerEntity()).isLeader)
                    return PartyState.Leader;//he has full access

                return m_State;
            }
        }

        public static int SelfIndex //get my party index
        { get { return m_PartyMembers.FindIndex(p => p.Player == WorldModel.Entities.GetPlayerEntity()); } }

        public static PartyMember Self //get me
        { get { return m_PartyMembers[SelfIndex]; } }

        public static Mobile Leader { get { return m_PartyMembers.Find(p => p.isLeader == true).Player; } }

        public static List<Mobile> List
        { get { return m_PartyMembers.Select(p => p.Player).ToList(); } }

        public static void PartyStateControl(string text, int hue)
        {
            INetworkClient m_Network = ServiceRegistry.GetService<INetworkClient>();
            WorldModel m_world = ServiceRegistry.GetService<WorldModel>();
            PartyCommand PCmd = new PartyCommand(text);//controlling command
            switch (Status)
            {
                case PartyState.None:
                    if (PCmd.PrimaryCmd == PCommandType.Add)//add member
                    {
                        PartySettings.Status = PartySettings.PartyState.Joining;
                        PartySettings.AddMember(WorldModel.Entities.GetPlayerEntity().Serial, true);
                        m_Network.Send(new PartyAddMember());
                    }
                    else if (PCmd.PrimaryCmd == PCommandType.HelpMenu)
                        showPartyHelp();
                    else
                        m_world.Interaction.ChatMessage("Wrong command you don't have a party. Please use /hlp", 3, 10, false);
                    break;
                case PartyState.Joining:
                    if (PCmd.PrimaryCmd == PCommandType.Add)//add member
                    {
                        LeaveParty();
                        Status = PartyState.Joining;
                        AddMember(WorldModel.Entities.GetPlayerEntity().Serial, true);
                        m_Network.Send(new PartyAddMember());
                        return;
                    }
                    else if (PCmd.PrimaryCmd == PCommandType.Accept && Leader != WorldModel.Entities.GetPlayerEntity())//accept party
                    {
                        m_Network.Send(new PartyAccept(Leader));
                        return;
                    }
                    else if (PCmd.PrimaryCmd == PCommandType.Quit)
                    {
                        LeaveParty();
                        return;
                    }
                    else if (PCmd.PrimaryCmd == PCommandType.Decline && Leader != WorldModel.Entities.GetPlayerEntity())//decline decline party
                    {
                        m_Network.Send(new PartyDecline(Leader));
                        return;
                    }
                    else if (PCmd.PrimaryCmd == PCommandType.HelpMenu)
                        showPartyHelp();
                    else
                        LeaveParty();
                    m_world.Interaction.ChatMessage("Wrong command. You can use '/accept' or '/decline'.", 3, 10, false);
                    break;
                case PartyState.Leader:
                case PartyState.Joined:
                    if (PCmd.PrimaryCmd == PCommandType.Add && Leader == WorldModel.Entities.GetPlayerEntity()) //he is party leader
                    {
                        m_Network.Send(new PartyAddMember());
                    }
                    else if (PCmd.PrimaryCmd == PCommandType.Remove && PCmd.SecondaryCmd != "unknowncmd")
                    {
                        int index1 = int.Parse(PCmd.SecondaryCmd);
                        if (getMember(index1) != null)
                            m_Network.Send(new PartyRemoveMember(index1));//wrong packet how can send a serial ?
                        else
                            m_world.Interaction.ChatMessage("Wrong party index. please first use '/hlp'.", 3, 10, false);
                    }
                    else if (PCmd.PrimaryCmd == PCommandType.HelpMenu)
                    {
                        showPartyHelp();
                    }
                    else if (PCmd.PrimaryCmd == PCommandType.List)
                    {
                        for (int i = 0; i < m_PartyMembers.Count; i++)
                        {
                            if (m_PartyMembers[i].isLeader)
                                m_world.Interaction.ChatMessage(string.Format("[{0}: {1}][LEADER]", i.ToString(), m_PartyMembers[i].Player.Name), 3, 53, true);
                            else
                                m_world.Interaction.ChatMessage(string.Format("[{0}: {1}]", i.ToString(), m_PartyMembers[i].Player.Name), 3, 55, true);
                        }
                    }
                    else if (PCmd.PrimaryCmd == PCommandType.Quit)
                    {
                        LeaveParty();
                    }
                    else if (PCmd.PrimaryCmd == PCommandType.Loot)
                    {
                        if (PCmd.SecondaryCmd == "on")
                            m_Network.Send(new PartyCanLoot(true));
                        else if (PCmd.SecondaryCmd == "off")
                            m_Network.Send(new PartyCanLoot(false));
                        else
                            m_world.Interaction.ChatMessage("Wrong command. Please use '/loot on' or '/loot off'.", 3, 10, false);
                    }
                    else if (PCmd.PrimaryCmd == PCommandType.Public)
                    {
                        m_Network.Send(new PartyPublicMessage(PCmd.PlayerMessage));
                    }
                    break;
                default:
                    m_world.Interaction.ChatMessage("PARTY SYSTEM ERROR !!!", 3, 30, false);
                    break;
            }
        }
        public static void showPartyHelp()
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

        public static void RefreshPartyStatusBar()
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
        public static void LeaveParty()
        {
            INetworkClient m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_Network.Send(new PartyQuit());
            Status = PartyState.None;
            m_PartyMembers.Clear();
            leaderSerial = 0;
        }
        public static PartyMember getMember(int index)
        {
            if (index >= 0 && index < m_PartyMembers.Count)
                return m_PartyMembers[index];
            else
                return null;
        }
        public static PartyMember getMember(Serial _serial)
        {
            return m_PartyMembers.Find(p => p.Player.Serial == _serial);
        }

        public static void AddMember(Serial _serial, bool _isleader)
        {
            if (_serial == leaderSerial)//after refresh list we don't know who is leader
                _isleader = true;

            int index = m_PartyMembers.FindIndex(p => p.Player.Serial == _serial);//if already in ?
            if (index != -1)//remove and add new member (refreshing ?)
                m_PartyMembers.RemoveAt(index);

            m_PartyMembers.Add(new PartyMember(_serial, _isleader));

            if (_isleader)//first add leader
                leaderSerial = _serial;
        }

        public static void RemoveMember(Serial _serial)
        {
            int index = m_PartyMembers.FindIndex(p => p.Player.Serial == _serial);
            if (index != -1)
            {
                m_PartyMembers.RemoveAt(index);
            }
        }

    }
}
