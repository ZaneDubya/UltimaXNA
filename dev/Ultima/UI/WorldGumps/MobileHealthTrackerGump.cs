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
        private Mobile m_Mobile;

        private GumpPic m_Background;
        private GumpPic[] m_Bars;
        private GumpPic[] m_BarBGs;

        public MobileHealthTrackerGump(Mobile mobile)
            : base(mobile.Serial, 0)
        {
            m_Mobile = mobile;

            if (m_Mobile.IsClientEntity)
            {
                m_Background = new GumpPic(this, 0, 0, 0x0803, 0);
                m_Bars = new GumpPic[3];
                m_Bars[0] = new GumpPic(this, 38, 12, 0x0806, 0);
                m_Bars[1] = new GumpPic(this, 38, 26, 0x0806, 0);
                m_Bars[2] = new GumpPic(this, 38, 40, 0x0806, 0);
                m_BarBGs = new GumpPic[3];
                m_BarBGs[0] = new GumpPic(this, 38, 12, 0x0805, 0);
                m_BarBGs[1] = new GumpPic(this, 38, 26, 0x0805, 0);
                m_BarBGs[2] = new GumpPic(this, 38, 40, 0x0805, 0);
            }
            else
            {
                m_Background = new GumpPic(this, 0, 0, 0x0804, 0);
                m_Bars = new GumpPic[1];
                m_Bars[0] = new GumpPic(this, 38, 12, 0x0806, 0);
                m_BarBGs = new GumpPic[1];
                m_BarBGs[0] = new GumpPic(this, 38, 12, 0x0805, 0);
            }
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_Mobile.IsClientEntity)
            {
                if (m_Mobile.Flags.IsWarMode)
                    m_Background.GumpID = 0x0807;
                else
                    m_Background.GumpID = 0x0803;
            }
            else
            {
                m_Background.GumpID = 0x0804;
            }

            base.Update(totalMS, frameMS);
        }
    }
}
