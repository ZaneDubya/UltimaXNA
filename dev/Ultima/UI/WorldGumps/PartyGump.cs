using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Configuration;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    public class PartyGump : Gump
    {
        Button[] kickBtn = new Button[10];
        Button[] tellBtn = new Button[10];
        Button btnLoot;
        TextLabelAscii txtLoot;

        public PartyGump()
            : base(0, 0)
        {
            if (PartySettings.List.Count == 1)//party bug fixing
                PartySettings.LeaveParty();

            IsMoveable = true;
            AddControl(new ResizePic(this, 0, 0, 2600, 350, 500));
            AddControl(new TextLabelAscii(this, 105, 15, 2, 1, @"Party Manifest"));
            AddControl(new TextLabelAscii(this, 30, 45, 2, 132, @"Kick"));
            AddControl(new TextLabelAscii(this, 95, 45, 2, 67, @"Tell"));
            AddControl(new TextLabelAscii(this, 160, 45, 2, 112, @"Member Name"));
            ///line 1

            int lineC = 0;
            int memberCount = 0;
            for (int i = 0; i < PartySettings.List.Count; i++)
            {
                if (PartySettings.Status == PartySettings.PartyState.None || PartySettings.Status == PartySettings.PartyState.Joining)
                    break;

                if (!PartySettings.getMember(i).isLeader && PartySettings.getMember(PartySettings.SelfIndex).isLeader)
                    AddControl(kickBtn[i] = new Button(this, 35, 70 + lineC, 4017, 4018, ButtonTypes.Activate, PartySettings.List[i].Serial, 100 + i));// KICK BUTTON

                AddControl(new TextLabelAscii(this, 65, 70 + lineC, 2, 1, string.Format("[ {0} ]", i)));
                if (WorldModel.Entities.GetPlayerEntity().Serial != PartySettings.List[i].Serial) //you can't send a message to self
                    AddControl(tellBtn[i] = new Button(this, 100, 70 + lineC, 4029, 4030, ButtonTypes.Activate, PartySettings.List[i].Serial, 200 + i));// tell BUTTON
                AddControl(new ResizePic(this, 130, 70 + lineC, 3000, 195, 25));
                AddControl(new TextLabelAscii(this, 135, 72 + lineC, 2, 98, PartySettings.List[i].Name));//member name
                lineC += 30;
                memberCount++;
            }
            for (int i = (0 + memberCount); i < 10; i++)
            {
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
            if (PartySettings.Status != PartySettings.PartyState.None && PartySettings.Status != PartySettings.PartyState.Joining)
            {
                if (PartySettings.Self.isLootable)
                {
                    gumID1 = 4008;
                    gumpID2 = 4008;
                    txtLoot.Text = txtLoot.Text.Replace("CANNOT", "CAN");
                }
            }
            if (PartySettings.Status != PartySettings.PartyState.None)
                AddControl(btnLoot = new Button(this, 65, 75 + lineC, gumID1, gumpID2, ButtonTypes.Activate, 0, 0));// loot BUTTON
            lineC += 25;
            string text = "Leave the party";
            if (PartySettings.Status == PartySettings.PartyState.Leader)
                text = "Disband the party";//SPECİAL MESSAGE FOR LEADER ??

            AddControl(new TextLabelAscii(this, 100, 75 + lineC, 2, 1, text));
            if (PartySettings.Status != PartySettings.PartyState.None)
                AddControl(new Button(this, 65, 75 + lineC, 4017, 4018, ButtonTypes.Activate, 0, 1));// leave BUTTON
            lineC += 25;
            AddControl(new TextLabelAscii(this, 100, 75 + lineC, 2, 1, @"Add new member"));
            if (PartySettings.Status != PartySettings.PartyState.Joined)
                AddControl(new Button(this, 65, 75 + lineC, 4005, 4006, ButtonTypes.Activate, 0, 2));// add BUTTON
            lineC += 25;

            AddControl(new TextLabelAscii(this, 100, 75 + lineC, 2, 1, @"Mark an enemy"));//this is special command for leader
            if (PartySettings.Status == PartySettings.PartyState.Leader)
                AddControl(new Button(this, 65, 75 + lineC, 4005, 4006, ButtonTypes.Activate, 0, 3));// set a target BUTTON

            if (PartySettings.Status == PartySettings.PartyState.Joined || PartySettings.Status == PartySettings.PartyState.Leader)
                PartySettings.RefreshPartyStatusBar();
        }

        public override void OnButtonClick(int buttonID)
        {
            if (buttonID == -1)
                return;
            INetworkClient m_Network = ServiceRegistry.GetService<INetworkClient>();
            if (buttonID >= 200)
            {
                int _serial = tellBtn[buttonID - 200].ButtonParameter;//private message: player serial
                m_Network.Send(new PartyPrivateMessage((Serial)_serial, "make a dynamic message type"));
            }
            else if (buttonID >= 100)
            {
                int _serial = kickBtn[buttonID - 100].ButtonParameter;//deleting player serial
                m_Network.Send(new PartyRemoveMember(_serial));
                PartySettings.RemoveMember(_serial);
                if (PartySettings.List.Count == 1)//fixing party bug
                {
                    PartySettings.LeaveParty();
                }
            }
            else if (buttonID == 0 && PartySettings.Status != PartySettings.PartyState.None && PartySettings.Status != PartySettings.PartyState.Joining)
            {
                if (PartySettings.Self.isLootable)
                {
                    m_Network.Send(new PartyCanLoot(false));
                    btnLoot.GumpUpID = 4002;
                    btnLoot.GumpDownID = 4002;
                    PartySettings.Self.isLootable = false;
                    txtLoot.Text = txtLoot.Text.Replace("CAN", "CANNOT");
                }
                else
                {
                    m_Network.Send(new PartyCanLoot(true));
                    btnLoot.GumpUpID = 4008;
                    btnLoot.GumpDownID = 4008;
                    PartySettings.Self.isLootable = true;
                    txtLoot.Text = txtLoot.Text.Replace("CANNOT", "CAN");
                }
            }
            else if (buttonID == 1)
            {
                PartySettings.LeaveParty();//leaving party
            }
            else if (buttonID == 2)
            {
                if (PartySettings.Status == PartySettings.PartyState.None)
                {
                    PartySettings.Status = PartySettings.PartyState.Joining;
                    PartySettings.AddMember(WorldModel.Entities.GetPlayerEntity().Serial, true);
                    m_Network.Send(new PartyAddMember());
                }
                else if (PartySettings.Status == PartySettings.PartyState.Leader)
                {
                    m_Network.Send(new PartyAddMember());
                }
                else
                {
                    //not access
                }
            }
            else if (buttonID == 3)
            {
                m_Network.Send(new PartyPublicMessage("All member attack to ---PLAYERNAME---"));
            }
        }
    }
}
