using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Configuration;
using UltimaXNA.Ultima.UI.Controls;

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    public class PartyGump : Gump
    {
        public PartyGump()
            : base(0, 0)
        {
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
                    AddControl(new Button(this, 35, 70 + lineC, 4017, 4018, ButtonTypes.Activate, 1, 0));// KICK BUTTON

                AddControl(new TextLabelAscii(this, 65, 70 + lineC, 2, 1, string.Format("[ {0} ]", i)));
                AddControl(new Button(this, 100, 70 + lineC, 4029, 4030, ButtonTypes.Activate, 1, 0));// tell BUTTON
                AddControl(new ResizePic(this, 130, 70 + lineC, 3000, 195, 25));
                AddControl(new TextLabelAscii(this, 135, 72 + lineC, 2, 98, PartySettings.List[i].Name));//member name
                lineC += 30;
                memberCount++;
            }
            for (int i = (0 + memberCount); i < 10; i++)
            {
                AddControl(new Button(this, 35, 70 + lineC, 4017, 4018, ButtonTypes.Activate, 1, 0));// KICK BUTTON
                AddControl(new TextLabelAscii(this, 65, 70 + lineC, 2, 1, string.Format("[ {0} ]", i)));
                AddControl(new Button(this, 100, 70 + lineC, 4029, 4030, ButtonTypes.Activate, 1, 0));// tell BUTTON
                AddControl(new ResizePic(this, 130, 70 + lineC, 3000, 195, 25));
                AddControl(new TextLabelAscii(this, 205, 72 + lineC, 2, 1, @"NONE"));//member name
                lineC += 30;
            }
            ///
            AddControl(new TextLabelAscii(this, 100, 75 + lineC, 2, 1, @"Party CANNOT loot me"));
            AddControl(new Button(this, 65, 75 + lineC, 4017, 4005, ButtonTypes.Activate, 1, 0));// loot BUTTON
            lineC += 30;
            string text = "Leave the party";
            if (PartySettings.Status == PartySettings.PartyState.Leader)
                text = "Disband the party";

            AddControl(new TextLabelAscii(this, 100, 75 + lineC, 2, 1, text));
            AddControl(new Button(this, 65, 75 + lineC, 4017, 4018, ButtonTypes.Activate, 1, 0));// leave BUTTON
            lineC += 30;
            AddControl(new TextLabelAscii(this, 100, 75 + lineC, 2, 1, @"Add new member"));
            if (PartySettings.Status != PartySettings.PartyState.Joined)
                AddControl(new Button(this, 65, 75 + lineC, 4005, 4006, ButtonTypes.Activate, 1, 0));// add BUTTON
        }
    }
}
