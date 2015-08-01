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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class MobileHealthTrackerGump : Gump
    {
        public Mobile Mobile
        {
            get;
            private set;
        }

        private GumpPic m_Background;
        private GumpPicWithWidth[] m_Bars;
        private GumpPic[] m_BarBGs;

        public MobileHealthTrackerGump(Mobile mobile)
            : base(mobile.Serial, 0)
        {
            while (UserInterface.GetControl<MobileHealthTrackerGump>(mobile.Serial) != null)
            {
                UserInterface.GetControl<MobileHealthTrackerGump>(mobile.Serial).Dispose();
            }

            IsMoveable = true;
            HandlesMouseInput = true;

            Mobile = mobile;

            if (Mobile.IsClientEntity)
            {
                AddControl(m_Background = new GumpPic(this, 0, 0, 0x0803, 0));
                m_BarBGs = new GumpPic[3];
                AddControl(m_BarBGs[0] = new GumpPic(this, 34, 10, 0x0805, 0));
                AddControl(m_BarBGs[1] = new GumpPic(this, 34, 24, 0x0805, 0));
                AddControl(m_BarBGs[2] = new GumpPic(this, 34, 38, 0x0805, 0));
                m_Bars = new GumpPicWithWidth[3];
                AddControl(m_Bars[0] = new GumpPicWithWidth(this, 34, 10, 0x0806, 0, 1f));
                AddControl(m_Bars[1] = new GumpPicWithWidth(this, 34, 24, 0x0806, 0, 1f));
                AddControl(m_Bars[2] = new GumpPicWithWidth(this, 34, 38, 0x0806, 0, 1f));
            }
            else
            {
                AddControl(m_Background = new GumpPic(this, 0, 0, 0x0804, 0));
                m_BarBGs = new GumpPic[1];
                AddControl(m_BarBGs[0] = new GumpPic(this, 34, 38, 0x0805, 0));
                m_Bars = new GumpPicWithWidth[1];
                AddControl(m_Bars[0] = new GumpPicWithWidth(this, 34, 38, 0x0806, 0, 1f));
                AddControl(new HtmlGumpling(this, 17, 13, 120, 20, 0, 0, String.Format("<span color='#000' style='font-family:uni0;'>{0}", mobile.Name)));
            }
        }

        public override void Update(double totalMS, double frameMS)
        {
            m_Bars[0].PercentWidthDrawn = ((float)Mobile.Health.Current / Mobile.Health.Max);
            if (Mobile.Flags.IsBlessed)
                m_Bars[0].GumpID = 0x0809;
            else if (Mobile.Flags.IsPoisoned)
                m_Bars[0].GumpID = 0x0808;

            if (Mobile.IsClientEntity)
            {
                if (Mobile.Flags.IsWarMode)
                    m_Background.GumpID = 0x0807;
                else
                    m_Background.GumpID = 0x0803;
                m_Bars[1].PercentWidthDrawn = ((float)Mobile.Stamina.Current / Mobile.Stamina.Max);
                m_Bars[2].PercentWidthDrawn = ((float)Mobile.Mana.Current / Mobile.Mana.Max);
            }
            else
            {
                // this doesn't change anything, but might as well leave it in incase we do want to change the graphic
                // based on some future condition.
                m_Background.GumpID = 0x0804;
            }

            base.Update(totalMS, frameMS);
        }
    }
}
