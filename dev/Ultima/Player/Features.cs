/***************************************************************************
 *   Features.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Ultima.Data
{
    public class Features
    {
        FeatureFlags m_Flags;
        public void SetFlags(FeatureFlags flags)
        {
            m_Flags |= flags;
        }

        public bool T2A => m_Flags.HasFlag(FeatureFlags.TheSecondAge);
        public bool UOR => m_Flags.HasFlag(FeatureFlags.Renaissance);
        public bool ThirdDawn => m_Flags.HasFlag(FeatureFlags.ThirdDawn);
        public bool LBR => m_Flags.HasFlag(FeatureFlags.LordBlackthornsRevenge);
        public bool AOS => m_Flags.HasFlag(FeatureFlags.AgeOfShadows);
        public bool CharSlots6 => m_Flags.HasFlag(FeatureFlags.CharacterSlot6);
        public bool SE => m_Flags.HasFlag(FeatureFlags.SameraiEmpire);
        public bool ML => m_Flags.HasFlag(FeatureFlags.MondainsLegacy);
        public bool Splash8th => m_Flags.HasFlag(FeatureFlags.Splash8);
        public bool Splash9th => m_Flags.HasFlag(FeatureFlags.Splash9);
        public bool TenthAge => m_Flags.HasFlag(FeatureFlags.TenthAge);
        public bool MoreStorage => m_Flags.HasFlag(FeatureFlags.MoreStorage);
        public bool CharSlots7 => m_Flags.HasFlag(FeatureFlags.TheSecondAge);
        public bool TenthAgeFaces => m_Flags.HasFlag(FeatureFlags.TenthAgeFaces);
        public bool TrialAccount => m_Flags.HasFlag(FeatureFlags.TrialAccount);
        public bool EleventhAge => m_Flags.HasFlag(FeatureFlags.EleventhAge);
        public bool SA => m_Flags.HasFlag(FeatureFlags.StygianAbyss);

        public bool TooltipsEnabled => AOS;
    }
}