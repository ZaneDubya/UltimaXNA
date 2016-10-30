using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.Player.Partying;

namespace UltimaXNA.Ultima.Network.Server.GeneralInfo {
    /// <summary>
    /// Subcommand 0x06: General party info. Incomplete?
    /// </summary>
    class PartyInfo : IGeneralInfo {
        public ushort partyMessageHue = 68;
        public string partyMessage = string.Empty;
        public string partyMessager = string.Empty;

        public PartyInfo(PacketReader reader) {
            int num = reader.ReadByte();
            switch (num) {
                case 1:
                    //party member list here
                    int memberCount = reader.ReadByte();
                    for (int i = 0; i < memberCount; i++)
                        PlayerState.Partying.AddMember(reader.ReadInt32(), false);

                    PlayerState.Partying.Status = PartyState.Joined;
                    PlayerState.Partying.RefreshPartyStatusBar();
                    break;
                case 2:
                    //remove party member and refresh list
                    int newPartyCount = reader.ReadByte();
                    int _remoredMember = reader.ReadInt32();
                    PlayerState.Partying.RemoveMember(_remoredMember);//removing

                    for (int i = 0; i < newPartyCount; i++) {//new list coming
                        PlayerState.Partying.AddMember(reader.ReadInt32(), false);
                    }

                    PlayerState.Partying.Status = PartyState.Joined;
                    PlayerState.Partying.RefreshPartyStatusBar();
                    break;
                case 3://private message?
                case 4://public message?
                    int serial = reader.ReadInt32();
                    string writerMessage = reader.ReadUnicodeString();
                    PartyMember member = PlayerState.Partying.GetMember((Serial)serial);//getting from list
                    string writerUserName = member.Player.Name;
                    switch (writerMessage) {
                        case "Help me.. I'm stunned !!"://this is for party coordination. we need auto send from Partymember who is under attack
                            partyMessageHue = 34;
                            break;
                        case "targeted to : "://we need new command (for party leader)
                            partyMessageHue = 50;
                            break;
                        default:
                            break;
                    }
                    if (num == 3)//PRIVATE party message
                        partyMessageHue = 58;//i need from option menu
                    else//PUBLIC party message
                        partyMessageHue = 68;//i need from option menu
                    partyMessage = writerMessage;
                    partyMessager = writerUserName;
                    break;
                case 7://PARTY INVITE PROGRESS
                    int _leaderSerial = reader.ReadInt32();
                    PlayerState.Partying.Status = PartyState.Joining;
                    PlayerState.Partying.AddMember(_leaderSerial, true);
                    break;
                default:
                    partyMessage = "ERROR";//TRACE.WARN??
                    PlayerState.Partying.LeaveParty();//
                    break;
            }
        }
    }
}
