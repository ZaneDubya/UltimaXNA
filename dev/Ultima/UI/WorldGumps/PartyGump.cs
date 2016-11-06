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
        const int ButtonIndexKick = 100;
        const int ButtonIndexTell = 200;

        public PartyGump()
            : base(0, 0) {
            IsMoveable = true;
            AddControl(new ResizePic(this, 0, 0, 2600, 350, PlayerState.Partying.InParty ? 500 : 425));
            AddControl(new TextLabelAscii(this, 105, 15, 2, 902, "Party Manifest"));
            AddControl(new TextLabelAscii(this, 34, 51, 1, 902, "Kick"));
            AddControl(new TextLabelAscii(this, 84, 51, 1, 902, "Tell"));
            AddControl(new TextLabelAscii(this, 160, 45, 2, 902, "Member Name"));
            int lineY = 0;
            bool playerIsLeader = PlayerState.Partying.LeaderSerial == WorldModel.PlayerSerial;
            for (int i = 0; i < 10; i++)
            {
                if (i < PlayerState.Partying.Members.Count)
                {
                    bool memberIsPlayer = PlayerState.Partying.GetMember(i).Serial == WorldModel.PlayerSerial;
                    if (!memberIsPlayer)
                    {
                        if (playerIsLeader)
                        {
                            AddControl(new Button(this, 35, 70 + lineY, 4017, 4018, ButtonTypes.Activate, PlayerState.Partying.Members[i].Serial, ButtonIndexKick + i));// KICK BUTTON
                        }
                        AddControl(new Button(this, 85, 70 + lineY, 4029, 4030, ButtonTypes.Activate, PlayerState.Partying.Members[i].Serial, ButtonIndexTell + i));// tell BUTTON
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
            if (PlayerState.Partying.InParty)
            {
                int lootGumpUp = PlayerState.Partying.AllowPartyLoot ? 4008 : 4002;
                string txtLootStatus = PlayerState.Partying.AllowPartyLoot ? "Party CAN loot me" : "Party CANNOT loot me";
                AddControl(new TextLabelAscii(this, 100, 75 + lineY, 2, 902, txtLootStatus));
                AddControl(new Button(this, 65, 75 + lineY, lootGumpUp, lootGumpUp + 2, ButtonTypes.Activate, ButtonIndexLoot, 0));
                (LastControl as Button).GumpOverID = lootGumpUp + 1;
                lineY += 25;
                string txtLeave = (playerIsLeader) ? "Disband the party" : "Leave the party";
                AddControl(new TextLabelAscii(this, 100, 75 + lineY, 2, 902, txtLeave));
                AddControl(new Button(this, 65, 75 + lineY, 4017, 4019, ButtonTypes.Activate, 0, ButtonIndexLeave));
                (LastControl as Button).GumpOverID = 4018;
                lineY += 25;
                AddControl(new TextLabelAscii(this, 100, 75 + lineY, 2, 902, "Add new member"));
                AddControl(new Button(this, 65, 75 + lineY, 4005, 4007, ButtonTypes.Activate, 0, ButtonIndexAdd));
                (LastControl as Button).GumpOverID = 4006;
                /* msx752 proposed feature addition: Mark an enemy.
                lineY += 25;
                AddControl(new TextLabelAscii(this, 100, 75 + lineY, 2, 902, "Mark an enemy"));
                if (playerIsLeader)
                {
                    AddControl(new Button(this, 65, 75 + lineY, 4005, 4006, ButtonTypes.Activate, 0, ButtonIndexSetTarget));
                    (LastControl as Button).GumpOverID = 4007;
                }*/
            }
            else
            {
                AddControl(new TextLabelAscii(this, 100, 75 + lineY, 2, 902, "Create a party"));
                AddControl(new Button(this, 65, 75 + lineY, 4005, 4007, ButtonTypes.Activate, 0, ButtonIndexAdd));
                (LastControl as Button).GumpOverID = 4006;
            }
        }

        public override void OnButtonClick(int buttonID) {
            if (buttonID == -1) {
                return;
            }
            bool playerInParty = PlayerState.Partying.Members.Count > 1;
            bool playerIsLeader = PlayerState.Partying.LeaderSerial == WorldModel.PlayerSerial;
            if (buttonID >= ButtonIndexTell) {
                int serial = PlayerState.Partying.GetMember(buttonID - ButtonIndexTell).Serial;
                PlayerState.Partying.SendTell(serial);
            }
            else if (buttonID >= ButtonIndexKick) {
                int serial = PlayerState.Partying.GetMember(buttonID - ButtonIndexKick).Serial;
                PlayerState.Partying.RemoveMember(serial);
            }
            else if (buttonID == ButtonIndexLoot && playerInParty) {
                if (PlayerState.Partying.AllowPartyLoot) {
                    PlayerState.Partying.AllowPartyLoot = false;
                }
                else {
                    PlayerState.Partying.AllowPartyLoot = true;
                }
                PlayerState.Partying.RefreshPartyGumps();
            }
            else if (buttonID == ButtonIndexLeave) {
                if (playerInParty)
                {
                    PlayerState.Partying.LeaveParty();
                }
            }
            else if (buttonID == ButtonIndexAdd) {
                if (!playerInParty || playerIsLeader) {
                    PlayerState.Partying.RequestAddPartyMemberTarget();
                }
            }
        }
    }
}