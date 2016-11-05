/***************************************************************************
 *   PartyGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.Player.Partying;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.UI.WorldGumps {
    public class PartyGump : Gump {
        const int ButtonIndexLeave = 1;
        const int ButtonIndexAdd = 2;
        const int ButtonIndexSetTarget = 3;
        const int ButtonIndexKick = 100;
        const int ButtonIndexTell = 200;

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
            AddControl(new TextLabelAscii(this, 105, 15, 2, 1, "Party Manifest"));
            AddControl(new TextLabelAscii(this, 30, 45, 2, 132, "Kick"));
            AddControl(new TextLabelAscii(this, 95, 45, 2, 67, "Tell"));
            AddControl(new TextLabelAscii(this, 160, 45, 2, 112, "Member Name"));
            int lineY = 0;
            int memberCount = 0;
            for (int i = 0; i < PlayerState.Partying.List.Count; i++) {
                if (PlayerState.Partying.Status == PartyState.None || PlayerState.Partying.Status == PartyState.Joining) {
                    break;
                }
                if (!PlayerState.Partying.GetMember(i).IsLeader && PlayerState.Partying.GetMember(PlayerState.Partying.SelfIndex).IsLeader) {
                    AddControl(kickBtn[i] = new Button(this, 35, 70 + lineY, 4017, 4018, ButtonTypes.Activate, PlayerState.Partying.List[i].Serial, ButtonIndexTell + i));// KICK BUTTON
                }
                AddControl(new TextLabelAscii(this, 65, 70 + lineY, 2, 1, $"[ {i} ]"));
                if (WorldModel.Entities.GetPlayerEntity().Serial != PlayerState.Partying.List[i].Serial) {
                    AddControl(tellBtn[i] = new Button(this, 100, 70 + lineY, 4029, 4030, ButtonTypes.Activate, PlayerState.Partying.List[i].Serial, ButtonIndexTell + i));// tell BUTTON
                }
                AddControl(new ResizePic(this, 130, 70 + lineY, 3000, 195, 25));
                AddControl(new TextLabelAscii(this, 135, 72 + lineY, 2, 98, PlayerState.Partying.List[i].Name));//member name
                lineY += 30;
                memberCount++;
            }
            for (int i = (0 + memberCount); i < 10; i++) {
                AddControl(kickBtn[i] = new Button(this, 35, 70 + lineY, 4017, 4018, ButtonTypes.Activate, 1, -1));// KICK
                AddControl(new TextLabelAscii(this, 65, 70 + lineY, 2, 1, $"[ {i} ]"));
                AddControl(tellBtn[i] = new Button(this, 100, 70 + lineY, 4029, 4030, ButtonTypes.Activate, 1, -1));// tell
                AddControl(new ResizePic(this, 130, 70 + lineY, 3000, 195, 25));
                AddControl(new TextLabelAscii(this, 205, 72 + lineY, 2, 1, "NONE"));//member name
                lineY += 30;
            }
            int gumpID1 = PlayerState.Partying.AllowPartyLoot ? 4008 : 4002;
            int gumpID2 = PlayerState.Partying.AllowPartyLoot ? 4008 : 4002;
            string txtLootStatus = PlayerState.Partying.AllowPartyLoot ? "Party CAN loot me" :" Party CANNOT loot me";
            AddControl(txtLoot = new TextLabelAscii(this, 100, 75 + lineY, 2, 1, txtLootStatus));
            if (PlayerState.Partying.Status != PartyState.None) {
                AddControl(btnLoot = new Button(this, 65, 75 + lineY, gumpID1, gumpID2, ButtonTypes.Activate, 0, 0));// loot BUTTON
            }
            lineY += 25;
            string txtLeave = (PlayerState.Partying.Status == PartyState.Leader) ? "Disband the party" : "Leave the party";
            AddControl(new TextLabelAscii(this, 100, 75 + lineY, 2, 1, txtLeave));
            if (PlayerState.Partying.Status != PartyState.None) {
                AddControl(new Button(this, 65, 75 + lineY, 4017, 4018, ButtonTypes.Activate, 0, ButtonIndexLeave));// leave BUTTON
            }
            lineY += 25;
            AddControl(new TextLabelAscii(this, 100, 75 + lineY, 2, 1, "Add new member"));
            if (PlayerState.Partying.Status != PartyState.Joined) {
                AddControl(new Button(this, 65, 75 + lineY, 4005, 4006, ButtonTypes.Activate, 0, ButtonIndexAdd));// add BUTTON
            }
            lineY += 25;
            AddControl(new TextLabelAscii(this, 100, 75 + lineY, 2, 1, "Mark an enemy"));
            if (PlayerState.Partying.Status == PartyState.Leader) {
                AddControl(new Button(this, 65, 75 + lineY, 4005, 4006, ButtonTypes.Activate, 0, ButtonIndexSetTarget));// set a target BUTTON
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
            if (buttonID >= ButtonIndexTell) {
                int serial = tellBtn[buttonID - ButtonIndexTell].ButtonParameter;
                PlayerState.Partying.SendTell(serial);
            }
            else if (buttonID >= ButtonIndexTell) {
                int serial = kickBtn[buttonID - ButtonIndexTell].ButtonParameter;
                PlayerState.Partying.RemoveMember(serial);
                if (PlayerState.Partying.List.Count == 1) {
                    PlayerState.Partying.LeaveParty();
                }
            }
            else if (buttonID == 0 && PlayerState.Partying.Status != PartyState.None && PlayerState.Partying.Status != PartyState.Joining) {
                if (PlayerState.Partying.AllowPartyLoot) {
                    btnLoot.GumpUpID = 4002;
                    btnLoot.GumpDownID = 4002;
                    PlayerState.Partying.AllowPartyLoot = false;
                    txtLoot.Text = "Party CANNOT loot me";
                }
                else {
                    btnLoot.GumpUpID = 4008;
                    btnLoot.GumpDownID = 4008;
                    PlayerState.Partying.AllowPartyLoot = true;
                    txtLoot.Text = "Party CAN loot me";
                }
            }
            else if (buttonID == ButtonIndexLeave) {
                PlayerState.Partying.LeaveParty();//leaving party
            }
            else if (buttonID == ButtonIndexAdd) {
                if (PlayerState.Partying.Status == PartyState.None) {
                    PlayerState.Partying.Status = PartyState.Joining;
                    PlayerState.Partying.AddMember(WorldModel.Entities.GetPlayerEntity().Serial, true);
                    PlayerState.Partying.RequestAddPartyMemberTarget();
                }
                else if (PlayerState.Partying.Status == PartyState.Leader) {
                    PlayerState.Partying.RequestAddPartyMemberTarget();
                }
                else {
                    //no access
                }
            }
            else if (buttonID == ButtonIndexSetTarget) {
                PlayerState.Partying.SetPartyTarget();
            }
        }
    }
}