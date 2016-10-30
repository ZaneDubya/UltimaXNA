using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Network.Client.PartySystem;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.Player.Partying;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.UI.WorldGumps {
    public class PartyGump : Gump {
        Button btnLoot;
        Button[] kickBtn = new Button[10];
        Button[] tellBtn = new Button[10];
        TextLabelAscii txtLoot;

        public PartyGump()
            : base(0, 0) {
            if (PlayerState.Partying.List.Count == 1) {
                PlayerState.Partying.LeaveParty();
            }
            IsMoveable = true;
            AddControl(new ResizePic(this, 0, 0, 2600, 350, 500));
            AddControl(new TextLabelAscii(this, 105, 15, 2, 1, @"Party Manifest"));
            AddControl(new TextLabelAscii(this, 30, 45, 2, 132, @"Kick"));
            AddControl(new TextLabelAscii(this, 95, 45, 2, 67, @"Tell"));
            AddControl(new TextLabelAscii(this, 160, 45, 2, 112, @"Member Name"));
            int lineC = 0;
            int memberCount = 0;
            for (int i = 0; i < PlayerState.Partying.List.Count; i++) {
                if (PlayerState.Partying.Status == PartyState.None || PlayerState.Partying.Status == PartyState.Joining) {
                    break;
                }
                if (!PlayerState.Partying.GetMember(i).IsLeader && PlayerState.Partying.GetMember(PlayerState.Partying.SelfIndex).IsLeader) {
                    AddControl(kickBtn[i] = new Button(this, 35, 70 + lineC, 4017, 4018, ButtonTypes.Activate, PlayerState.Partying.List[i].Serial, 100 + i));// KICK BUTTON
                }
                AddControl(new TextLabelAscii(this, 65, 70 + lineC, 2, 1, string.Format("[ {0} ]", i)));
                if (WorldModel.Entities.GetPlayerEntity().Serial != PlayerState.Partying.List[i].Serial) {
                    AddControl(tellBtn[i] = new Button(this, 100, 70 + lineC, 4029, 4030, ButtonTypes.Activate, PlayerState.Partying.List[i].Serial, 200 + i));// tell BUTTON
                }
                AddControl(new ResizePic(this, 130, 70 + lineC, 3000, 195, 25));
                AddControl(new TextLabelAscii(this, 135, 72 + lineC, 2, 98, PlayerState.Partying.List[i].Name));//member name
                lineC += 30;
                memberCount++;
            }
            for (int i = (0 + memberCount); i < 10; i++) {
                AddControl(kickBtn[i] = new Button(this, 35, 70 + lineC, 4017, 4018, ButtonTypes.Activate, 1, -1));// KICK
                AddControl(new TextLabelAscii(this, 65, 70 + lineC, 2, 1, string.Format("[ {0} ]", i)));
                AddControl(tellBtn[i] = new Button(this, 100, 70 + lineC, 4029, 4030, ButtonTypes.Activate, 1, -1));// tell
                AddControl(new ResizePic(this, 130, 70 + lineC, 3000, 195, 25));
                AddControl(new TextLabelAscii(this, 205, 72 + lineC, 2, 1, @"NONE"));//member name
                lineC += 30;
            }
            ///
            AddControl(txtLoot = new TextLabelAscii(this, 100, 75 + lineC, 2, 1, @"Party CANNOT loot me"));
            int gumID1 = 4002, gumpID2 = 4002;
            if (PlayerState.Partying.Status != PartyState.None && PlayerState.Partying.Status != PartyState.Joining) {
                if (PlayerState.Partying.Self.IsLootable) {
                    gumID1 = 4008;
                    gumpID2 = 4008;
                    txtLoot.Text = txtLoot.Text.Replace("CANNOT", "CAN");
                }
            }
            if (PlayerState.Partying.Status != PartyState.None) {
                AddControl(btnLoot = new Button(this, 65, 75 + lineC, gumID1, gumpID2, ButtonTypes.Activate, 0, 0));// loot BUTTON
            }
            lineC += 25;
            string text = "Leave the party";
            if (PlayerState.Partying.Status == PartyState.Leader) {
                text = "Disband the party";//SPECİAL MESSAGE FOR LEADER ??
            }
            AddControl(new TextLabelAscii(this, 100, 75 + lineC, 2, 1, text));
            if (PlayerState.Partying.Status != PartyState.None) {
                AddControl(new Button(this, 65, 75 + lineC, 4017, 4018, ButtonTypes.Activate, 0, 1));// leave BUTTON
            }
            lineC += 25;
            AddControl(new TextLabelAscii(this, 100, 75 + lineC, 2, 1, @"Add new member"));
            if (PlayerState.Partying.Status != PartyState.Joined) {
                AddControl(new Button(this, 65, 75 + lineC, 4005, 4006, ButtonTypes.Activate, 0, 2));// add BUTTON
            }
            lineC += 25;
            AddControl(new TextLabelAscii(this, 100, 75 + lineC, 2, 1, @"Mark an enemy"));//this is special command for leader
            if (PlayerState.Partying.Status == PartyState.Leader) {
                AddControl(new Button(this, 65, 75 + lineC, 4005, 4006, ButtonTypes.Activate, 0, 3));// set a target BUTTON
            }
            if (PlayerState.Partying.Status == PartyState.Joined || PlayerState.Partying.Status == PartyState.Leader) {
                PlayerState.Partying.RefreshPartyStatusBar();
            }
        }

        public override void OnButtonClick(int buttonID) {
            if (buttonID == -1) {
                return;
            }
            INetworkClient m_Network = ServiceRegistry.GetService<INetworkClient>();
            if (buttonID >= 200) {
                int serial = tellBtn[buttonID - 200].ButtonParameter;//private message: player serial
                m_Network.Send(new PartyPrivateMessagePacket((Serial)serial, "make a dynamic message type"));//need improve
            }
            else if (buttonID >= 100) {
                int serial = kickBtn[buttonID - 100].ButtonParameter;//deleting player serial
                m_Network.Send(new PartyRemoveMemberPacket(serial));
                PlayerState.Partying.RemoveMember(serial);
                if (PlayerState.Partying.List.Count == 1) {
                    PlayerState.Partying.LeaveParty();
                }
            }
            else if (buttonID == 0 && PlayerState.Partying.Status != PartyState.None && PlayerState.Partying.Status != PartyState.Joining) {
                if (PlayerState.Partying.Self.IsLootable) {
                    m_Network.Send(new PartyCanLootPacket(false));
                    btnLoot.GumpUpID = 4002;
                    btnLoot.GumpDownID = 4002;
                    PlayerState.Partying.Self.IsLootable = false;
                    txtLoot.Text = txtLoot.Text.Replace("CAN", "CANNOT");
                }
                else {
                    m_Network.Send(new PartyCanLootPacket(true));
                    btnLoot.GumpUpID = 4008;
                    btnLoot.GumpDownID = 4008;
                    PlayerState.Partying.Self.IsLootable = true;
                    txtLoot.Text = txtLoot.Text.Replace("CANNOT", "CAN");
                }
            }
            else if (buttonID == 1) {
                PlayerState.Partying.LeaveParty();//leaving party
            }
            else if (buttonID == 2) {
                if (PlayerState.Partying.Status == PartyState.None) {
                    PlayerState.Partying.Status = PartyState.Joining;
                    PlayerState.Partying.AddMember(WorldModel.Entities.GetPlayerEntity().Serial, true);
                    m_Network.Send(new PartyAddMemberPacket());
                }
                else if (PlayerState.Partying.Status == PartyState.Leader) {
                    m_Network.Send(new PartyAddMemberPacket());
                }
                else {
                    //no access
                }
            }
            else if (buttonID == 3) {
                m_Network.Send(new PartyPublicMessagePacket("All member attack to ---PLAYERNAME---"));//need improve
            }
        }
    }
}