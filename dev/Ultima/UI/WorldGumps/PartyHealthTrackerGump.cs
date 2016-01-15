/***************************************************************************
 *   MobileHealthTrackerGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Input;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Configuration;
using UltimaXNA.Ultima.World;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class PartyHealthTrackerGump : Gump
    {
        public Mobile Mobile
        {
            get;
            private set;
        }

        private ResizePic m_Background;
        private GumpPicWithWidth[] m_Bars;
        private GumpPic[] m_BarBGs;

        public PartyHealthTrackerGump(Serial _serial)
            : base(_serial, 0)
        {
            while (UserInterface.GetControl<MobileHealthTrackerGump>() != null)
            {
                UserInterface.GetControl<MobileHealthTrackerGump>(_serial).Dispose();
            }
            IsMoveable = false;
            Mobile = WorldModel.Entities.GetObject<Mobile>(_serial, false);
            if (Mobile == null)
            {
                Dispose();
                PartySettings.RemoveMember(_serial);
                return;
            }
            AddControl(m_Background = new ResizePic(this, 0, 0, 3000, 131, 48));//I couldn't find correct visual
            int color = 1;//normal party member
            if (PartySettings.getMember(Mobile.Serial).isLeader)
                color = 20;
            AddControl(new TextLabelAscii(this, 1, 3, 9, color, Mobile.Name));
            //m_Background.MouseDoubleClickEvent += Background_MouseDoubleClickEvent; //maybe private message calling?
            m_BarBGs = new GumpPic[3];
            int sameX = 10;
            int sameY = 0;
            AddControl(m_BarBGs[0] = new GumpPic(this, sameX, 15 + sameY, 2053, 0));
            AddControl(m_BarBGs[1] = new GumpPic(this, sameX, 25 + sameY, 2053, 0));
            AddControl(m_BarBGs[2] = new GumpPic(this, sameX, 35 + sameY, 2053, 0));
            m_Bars = new GumpPicWithWidth[3];
            AddControl(m_Bars[0] = new GumpPicWithWidth(this, sameX, 15 + sameY, 2057, 0, 1f));//I couldn't find correct visual
            AddControl(m_Bars[1] = new GumpPicWithWidth(this, sameX, 25 + sameY, 2054, 0, 1f));//I couldn't find correct visual
            AddControl(m_Bars[2] = new GumpPicWithWidth(this, sameX, 35 + sameY, 2056, 0, 1f));//I couldn't find correct visual

            // bars should not handle mouse input, pass it to the background gump.
            for (int i = 0; i < m_BarBGs.Length; i++)
            {
                //m_BarBGs[i].HandlesMouseInput = false;
                m_Bars[i].HandlesMouseInput = false;
            }
        }

        public override void Dispose()
        {
            //m_Background.MouseDoubleClickEvent -= Background_MouseDoubleClickEvent;
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (Mobile == null)
            {
                return;
            }
            m_Bars[0].PercentWidthDrawn = ((float)Mobile.Health.Current / Mobile.Health.Max);
            //I couldn't find correct visual
            //if (Mobile.Flags.IsBlessed)
            //    m_Bars[0].GumpID = 0x0809;
            //else if (Mobile.Flags.IsPoisoned)
            //    m_Bars[0].GumpID = 0x0808;

            m_Bars[1].PercentWidthDrawn = ((float)Mobile.Mana.Current / Mobile.Mana.Max);

            m_Bars[2].PercentWidthDrawn = ((float)Mobile.Stamina.Current / Mobile.Stamina.Max);

            base.Update(totalMS, frameMS);
        }
        private void Background_MouseDoubleClickEvent(AControl caller, int x, int y, MouseButton button)
        {
            //CALL PRIVATE MESSAGE METHOD ???
        }
    }
}
