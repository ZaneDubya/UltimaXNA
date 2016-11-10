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
    public static class Features
    {
        static FeatureFlags m_Flags;
        public static void SetFlags(FeatureFlags flags)
        {
            m_Flags |= flags;
        }

        public static bool T2A => m_Flags.HasFlag(FeatureFlags.TheSecondAge);
        public static bool UOR => m_Flags.HasFlag(FeatureFlags.Renaissance);
        public static bool ThirdDawn => m_Flags.HasFlag(FeatureFlags.ThirdDawn);
        public static bool LBR => m_Flags.HasFlag(FeatureFlags.LordBlackthornsRevenge);
        public static bool AOS => m_Flags.HasFlag(FeatureFlags.AgeOfShadows);
        public static bool CharSlots6 => m_Flags.HasFlag(FeatureFlags.CharacterSlot6);
        public static bool SE => m_Flags.HasFlag(FeatureFlags.SameraiEmpire);
        public static bool ML => m_Flags.HasFlag(FeatureFlags.MondainsLegacy);
        public static bool Splash8th => m_Flags.HasFlag(FeatureFlags.Splash8);
        public static bool Splash9th => m_Flags.HasFlag(FeatureFlags.Splash9);
        public static bool TenthAge => m_Flags.HasFlag(FeatureFlags.TenthAge);
        public static bool MoreStorage => m_Flags.HasFlag(FeatureFlags.MoreStorage);
        public static bool CharSlots7 => m_Flags.HasFlag(FeatureFlags.TheSecondAge);
        public static bool TenthAgeFaces => m_Flags.HasFlag(FeatureFlags.TenthAgeFaces);
        public static bool TrialAccount => m_Flags.HasFlag(FeatureFlags.TrialAccount);
        public static bool EleventhAge => m_Flags.HasFlag(FeatureFlags.EleventhAge);
        public static bool SA => m_Flags.HasFlag(FeatureFlags.StygianAbyss);

        public static bool TooltipsEnabled => AOS;
    }
}