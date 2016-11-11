/***************************************************************************
 *   PartyHealthTrackerGump.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.Player.Partying;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class PartyHealthTrackerGump : Gump
    {
        Serial m_Serial;

        Button btnPrivateMsg;
        GumpPic[] m_BarBGs;
        GumpPicWithWidth[] m_Bars;
        TextLabel m_Name;

        public PartyHealthTrackerGump(PartyMember member)
            : base(member.Serial, 0)
        {
            while (UserInterface.GetControl<MobileHealthTrackerGump>() != null)
            {
                UserInterface.GetControl<MobileHealthTrackerGump>(member.Serial).Dispose();
            }
            IsMoveable = false;
            IsUncloseableWithRMB = true;
            m_Serial = member.Serial;
            //AddControl(m_Background = new ResizePic(this, 0, 0, 3000, 131, 48));//I need opacity %1 background

            AddControl(m_Name = new TextLabel(this, 1, 0, 1, member.Name));
            //m_Background.MouseDoubleClickEvent += Background_MouseDoubleClickEvent; //maybe private message calling?
            m_BarBGs = new GumpPic[3];
            int sameX = 15;
            int sameY = 3;
            if (WorldModel.Entities.GetPlayerEntity().Serial != member.Serial)//you can't send a message to self
            {
                AddControl(btnPrivateMsg = new Button(this, 0, 20, 11401, 11402, ButtonTypes.Activate, member.Serial, 0));//private party message / use bandage ??
            }
            AddControl(m_BarBGs[0] = new GumpPic(this, sameX, 15 + sameY, 9750, 0));
            AddControl(m_BarBGs[1] = new GumpPic(this, sameX, 24 + sameY, 9750, 0));
            AddControl(m_BarBGs[2] = new GumpPic(this, sameX, 33 + sameY, 9750, 0));
            m_Bars = new GumpPicWithWidth[3];
            AddControl(m_Bars[0] = new GumpPicWithWidth(this, sameX, 15 + sameY, 40, 0, 1f));//I couldn't find correct visual
            AddControl(m_Bars[1] = new GumpPicWithWidth(this, sameX, 24 + sameY, 9751, 0, 1f));//I couldn't find correct visual
            AddControl(m_Bars[2] = new GumpPicWithWidth(this, sameX, 33 + sameY, 41, 0, 1f));//I couldn't find correct visual

            // bars should not handle mouse input, pass it to the background gump.
            for (int i = 0; i < m_BarBGs.Length; i++)//???
            {
                m_Bars[i].HandlesMouseInput = false;
            }
        }

        public override void OnButtonClick(int buttonID)
        {
            if (buttonID == 0)//private message
            {
                PlayerState.Partying.BeginPrivateMessage(btnPrivateMsg.ButtonParameter);
            }
        }

        public override void Update(double totalMS, double frameMS)
        {
            PartyMember member = PlayerState.Partying.GetMember(m_Serial);
            if (member == null)
            {
                Dispose();
                return;
            }
            m_Name.Text = member.Name;
            Mobile mobile = member.Mobile;
            if (mobile == null)
            {
                m_Bars[0].PercentWidthDrawn = m_Bars[1].PercentWidthDrawn = m_Bars[2].PercentWidthDrawn = 0f;
            }
            else
            {
                m_Bars[0].PercentWidthDrawn = ((float)mobile.Health.Current / mobile.Health.Max);
                m_Bars[1].PercentWidthDrawn = ((float)mobile.Mana.Current / mobile.Mana.Max);
                m_Bars[2].PercentWidthDrawn = ((float)mobile.Stamina.Current / mobile.Stamina.Max);
                // I couldn't find correct visual
                //if (Mobile.Flags.IsBlessed)
                //    m_Bars[0].GumpID = 0x0809;
                //else if (Mobile.Flags.IsPoisoned)
                //    m_Bars[0].GumpID = 0x0808;
            }
            base.Update(totalMS, frameMS);
        }
    }
}