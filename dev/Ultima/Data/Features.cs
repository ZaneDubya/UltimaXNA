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

using UltimaXNA.Ultima.Network.Server;

namespace UltimaXNA.Ultima.Data
{
    internal static class Features
    {
        private static FeatureFlags m_Flags;

        public static bool EnableT2A
        {
            get { return m_Flags.HasFlag(FeatureFlags.TheSecondAge); }
        }

        public static bool EnableRen
        {
            get { return m_Flags.HasFlag(FeatureFlags.Renaissance); }
        }

        public static bool EnableThirdDawn
        {
            get { return m_Flags.HasFlag(FeatureFlags.ThirdDawn); }
        }

        public static bool EnableLBR
        {
            get { return m_Flags.HasFlag(FeatureFlags.LordBlackthornsRevenge); }
        }

        public static bool EnableAOS
        {
            get { return m_Flags.HasFlag(FeatureFlags.AgeOfShadows); }
        }

        public static bool Enable6CharSlots
        {
            get { return m_Flags.HasFlag(FeatureFlags.CharacterSlot6); }
        }

        public static bool EnableSE
        {
            get { return m_Flags.HasFlag(FeatureFlags.SameraiEmpire); }
        }

        public static bool EnableML
        {
            get { return m_Flags.HasFlag(FeatureFlags.MondainsLegacy); }
        }

        public static bool Enable8thSplash
        {
            get { return m_Flags.HasFlag(FeatureFlags.Splash8); }
        }

        public static bool Enable9thSplash
        {
            get { return m_Flags.HasFlag(FeatureFlags.Splash9); }
        }

        public static bool Enable10thAge
        {
            get { return m_Flags.HasFlag(FeatureFlags.TenthAge); }
        }

        public static bool EnableMoreStorage
        {
            get { return m_Flags.HasFlag(FeatureFlags.MoreStorage); }
        }

        public static bool Enable7CharSlots
        {
            get { return m_Flags.HasFlag(FeatureFlags.TheSecondAge); }
        }

        public static bool Enable10thAgeFaces
        {
            get { return m_Flags.HasFlag(FeatureFlags.TenthAgeFaces); }
        }

        public static bool EnableTrialAccount
        {
            get { return m_Flags.HasFlag(FeatureFlags.TrialAccount); }
        }

        public static bool Enable11thAge
        {
            get { return m_Flags.HasFlag(FeatureFlags.EleventhAge); }
        }

        public static bool EnableSA
        {
            get { return m_Flags.HasFlag(FeatureFlags.StygianAbys); }
        }
        
        public static void SetFlags(FeatureFlags flags)
        {
            m_Flags |= flags;
        }
    }
}