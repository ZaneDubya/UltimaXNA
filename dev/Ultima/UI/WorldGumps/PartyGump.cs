/***************************************************************************
 *   PartyGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    public class PartyGump : Gump {
        const int ButtonIndexLoot = 0;
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
            if (PlayerState.Partying.Members.Count == 1) {
                PlayerState.Partying.LeaveParty();
            }
            IsMoveable = true;
            AddControl(new ResizePic(this, 0, 0, 2600, 350, 500));
            AddControl(new TextLabelAscii(this, 105, 15, 2, 902, "Party Manifest"));
            AddControl(new TextLabelAscii(this, 34, 51, 1, 902, "Kick"));
            AddControl(new TextLabelAscii(this, 84, 51, 1, 902, "Tell"));
            AddControl(new TextLabelAscii(this, 160, 45, 2, 902, "Member Name"));
            int lineY = 0;
            bool playerIsLeader = PlayerState.Partying.LeaderSerial == WorldModel.PlayerSerial;
            bool playerInParty = PlayerState.Partying.Members.Count > 1;
            for (int i = 0; i < 10; i++)
            {
                if (i < PlayerState.Partying.Members.Count)
                {
                    bool memberIsPlayer = PlayerState.Partying.GetMember(i).Serial == WorldModel.PlayerSerial;
                    if (playerIsLeader && !memberIsPlayer)
                    {
                        AddControl(kickBtn[i] = new Button(this, 35, 70 + lineY, 4017, 4018, ButtonTypes.Activate, PlayerState.Partying.Members[i].Serial, ButtonIndexTell + i));// KICK BUTTON
                    }
                    if (!memberIsPlayer)
                    {
                        AddControl(tellBtn[i] = new Button(this, 85, 70 + lineY, 4029, 4030, ButtonTypes.Activate, PlayerState.Partying.Members[i].Serial, ButtonIndexTell + i));// tell BUTTON
                    }
                    AddControl(new ResizePic(this, 130, 70 + lineY, 3000, 195, 25));
                    AddControl(new HtmlGumpling(this, 130, 72 + lineY, 195, 20, 0, 0, $"<center><big><font color='#444'>{PlayerState.Partying.Members[i].Mobile.Name}"));
                }
                else
                {
                    AddControl(new ResizePic(this, 130, 70 + lineY, 3000, 195, 25));
                }
                lineY += 30;
            }
            int gumpID1 = PlayerState.Partying.AllowPartyLoot ? 4008 : 4002;
            int gumpID2 = PlayerState.Partying.AllowPartyLoot ? 4008 : 4002;
            string txtLootStatus = PlayerState.Partying.AllowPartyLoot ? "Party CAN loot me" : "Party CANNOT loot me";
            AddControl(txtLoot = new TextLabelAscii(this, 100, 75 + lineY, 2, 902, txtLootStatus));
            if (PlayerState.Partying.InParty) {
                AddControl(btnLoot = new Button(this, 65, 75 + lineY, gumpID1, gumpID2, ButtonTypes.Activate, ButtonIndexLoot, 0));
            }
            lineY += 25;
            string txtLeave = (playerIsLeader) ? "Disband the party" : "Leave the party";
            AddControl(new TextLabelAscii(this, 100, 75 + lineY, 2, 902, txtLeave));
            if (playerInParty) {
                AddControl(new Button(this, 65, 75 + lineY, 4017, 4018, ButtonTypes.Activate, 0, ButtonIndexLeave));
            }
            lineY += 25;
            AddControl(new TextLabelAscii(this, 100, 75 + lineY, 2, 902, "Add new member"));
            if (playerInParty) {
                AddControl(new Button(this, 65, 75 + lineY, 4005, 4006, ButtonTypes.Activate, 0, ButtonIndexAdd));
            }
            lineY += 25;
            AddControl(new TextLabelAscii(this, 100, 75 + lineY, 2, 902, "Mark an enemy"));
            if (playerIsLeader) {
                AddControl(new Button(this, 65, 75 + lineY, 4005, 4006, ButtonTypes.Activate, 0, ButtonIndexSetTarget));
            }
        }

        public override void OnButtonClick(int buttonID) {
            if (buttonID == -1) {
                return;
            }
            bool playerInParty = PlayerState.Partying.Members.Count > 1;
            bool playerIsLeader = PlayerState.Partying.LeaderSerial == WorldModel.PlayerSerial;
            if (buttonID >= ButtonIndexTell) {
                int serial = tellBtn[buttonID - ButtonIndexTell].ButtonParameter;
                PlayerState.Partying.SendTell(serial);
            }
            else if (buttonID >= ButtonIndexTell) {
                int serial = kickBtn[buttonID - ButtonIndexTell].ButtonParameter;
                PlayerState.Partying.RemoveMember(serial);
                if (PlayerState.Partying.Members.Count == 1) {
                    PlayerState.Partying.LeaveParty();
                }
            }
            else if (buttonID == ButtonIndexLoot && playerInParty) {
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
                PlayerState.Partying.LeaveParty();
            }
            else if (buttonID == ButtonIndexAdd) {
                if (!playerInParty) {
                    PlayerState.Partying.RequestAddPartyMemberTarget();
                }
                else if (playerIsLeader) {
                    PlayerState.Partying.RequestAddPartyMemberTarget();
                }
            }
            else if (buttonID == ButtonIndexSetTarget) {
                PlayerState.Partying.SetPartyTarget();
            }
        }
    }
}