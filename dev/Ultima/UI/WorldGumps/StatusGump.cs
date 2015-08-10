/***************************************************************************
 *   StatusGump.cs
 *   Based on code by surcouf94
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class StatusGump : Gump
    {
        private Mobile m_Mobile = WorldModel.Entities.GetPlayerEntity();
        double m_RefreshTime = 0d;

        private TextLabelAscii[] m_Labels = new TextLabelAscii[(int)MobileStats.Max];

        private enum MobileStats
        {
            Name,
            Strength,
            Dexterity,
            Intelligence,
            HealthCurrent,
            HealthMax,
            StaminaCurrent,
            StaminaMax,
            ManaCurrent,
            ManaMax,
            Followers,
            WeightCurrent,
            WeightMax,
            StatCap,
            Luck,
            Gold,
            AR,
            RF,
            RC,
            RP,
            RE,
            Damage,
            Max
        }

        public StatusGump()
            : base(0, 0)
        {
            IsMoveable = true;
            AddControl(new GumpPic(this, 0, 0, 0x2A6C, 0));

            m_Labels[(int)MobileStats.Name] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 54, 44, 1, 6, m_Mobile.Name));
            m_Labels[(int)MobileStats.Strength] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 88, 71, 1, 6, m_Mobile.Strength.ToString()));
            m_Labels[(int)MobileStats.Dexterity] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 88, 99, 1, 6, m_Mobile.Dexterity.ToString()));
            m_Labels[(int)MobileStats.Intelligence] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 87, 127, 1, 6, m_Mobile.Intelligence.ToString()));

            m_Labels[(int)MobileStats.HealthCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 67, 1, 6, m_Mobile.Health.Current.ToString()));
            m_Labels[(int)MobileStats.HealthMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 77, 1, 6, m_Mobile.Health.Max.ToString()));

            m_Labels[(int)MobileStats.StaminaCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 94, 1, 6, m_Mobile.Stamina.Current.ToString()));
            m_Labels[(int)MobileStats.StaminaMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 105, 1, 6, m_Mobile.Stamina.Max.ToString()));

            m_Labels[(int)MobileStats.ManaCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 148, 122, 1, 6, m_Mobile.Mana.Current.ToString()));
            m_Labels[(int)MobileStats.ManaMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 148, 133, 1, 6, m_Mobile.Mana.Max.ToString()));

            m_Labels[(int)MobileStats.Followers] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 289, 127, 1, 6, ConcatCurrentMax(m_Mobile.Followers.Current, m_Mobile.Followers.Max)));

            m_Labels[(int)MobileStats.WeightCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 212, 121, 1, 6, m_Mobile.Weight.Current.ToString()));
            m_Labels[(int)MobileStats.WeightMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 212, 132, 1, 6, m_Mobile.Weight.Max.ToString()));

            m_Labels[(int)MobileStats.StatCap] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 219, 71, 1, 6, m_Mobile.StatCap.ToString()));
            m_Labels[(int)MobileStats.Luck] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 218, 99, 1, 6, m_Mobile.Luck.ToString()));
            m_Labels[(int)MobileStats.Gold] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 282, 99, 1, 6, m_Mobile.Gold.ToString()));

            m_Labels[(int)MobileStats.AR] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 70, 1, 6, m_Mobile.ArmorRating.ToString()));
            m_Labels[(int)MobileStats.RF] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 85, 1, 6, m_Mobile.ResistFire.ToString()));
            m_Labels[(int)MobileStats.RC] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 100, 1, 6, m_Mobile.ResistCold.ToString()));
            m_Labels[(int)MobileStats.RP] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 114, 1, 6, m_Mobile.ResistPoison.ToString()));
            m_Labels[(int)MobileStats.RE] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 129, 1, 6, m_Mobile.ResistEnergy.ToString()));

            m_Labels[(int)MobileStats.Damage] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 277, 70, 1, 6, ConcatCurrentMax(m_Mobile.DamageMin, m_Mobile.DamageMax)));
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("status");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_RefreshTime + 0.5d < totalMS) //need to update
            {
                m_RefreshTime = totalMS;
                // we can just set these without checking if they've changed.
                // The label will only update if the value has changed.
                m_Labels[(int)MobileStats.Name].Text = m_Mobile.Name;
                m_Labels[(int)MobileStats.Strength].Text = m_Mobile.Strength.ToString();
                m_Labels[(int)MobileStats.Dexterity].Text = m_Mobile.Dexterity.ToString();
                m_Labels[(int)MobileStats.Intelligence].Text = m_Mobile.Intelligence.ToString();

                m_Labels[(int)MobileStats.HealthCurrent].Text = m_Mobile.Health.Current.ToString();
                m_Labels[(int)MobileStats.HealthMax].Text = m_Mobile.Health.Max.ToString();

                m_Labels[(int)MobileStats.StaminaCurrent].Text = m_Mobile.Stamina.Current.ToString();
                m_Labels[(int)MobileStats.StaminaMax].Text = m_Mobile.Stamina.Max.ToString();

                m_Labels[(int)MobileStats.ManaCurrent].Text = m_Mobile.Mana.Current.ToString();
                m_Labels[(int)MobileStats.ManaMax].Text = m_Mobile.Mana.Max.ToString();

                m_Labels[(int)MobileStats.Followers].Text = ConcatCurrentMax(m_Mobile.Followers.Current, m_Mobile.Followers.Max);

                m_Labels[(int)MobileStats.WeightCurrent].Text = m_Mobile.Weight.Current.ToString();
                m_Labels[(int)MobileStats.WeightMax].Text = m_Mobile.Weight.Max.ToString();

                m_Labels[(int)MobileStats.StatCap].Text = m_Mobile.StatCap.ToString();
                m_Labels[(int)MobileStats.Luck].Text = m_Mobile.Luck.ToString();
                m_Labels[(int)MobileStats.Gold].Text = m_Mobile.Gold.ToString();

                m_Labels[(int)MobileStats.AR].Text = m_Mobile.ArmorRating.ToString();
                m_Labels[(int)MobileStats.RF].Text = m_Mobile.ResistFire.ToString();
                m_Labels[(int)MobileStats.RC].Text = m_Mobile.ResistCold.ToString();
                m_Labels[(int)MobileStats.RP].Text = m_Mobile.ResistPoison.ToString();
                m_Labels[(int)MobileStats.RE].Text = m_Mobile.ResistEnergy.ToString();

                m_Labels[(int)MobileStats.Damage].Text = ConcatCurrentMax(m_Mobile.DamageMin, m_Mobile.DamageMax);
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);
        }

        private string ConcatCurrentMax(int min, int max)
        {
            return string.Format("{0}/{1}", min, max);
        }
    }
}