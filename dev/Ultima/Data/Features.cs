/***************************************************************************
 *   Features.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.UltimaPackets.Server;

namespace UltimaXNA.UltimaVars
{
    internal static class Features
    {
        private static FeatureFlags m_flags;

        public static bool EnableT2A
        {
            get { return m_flags.HasFlag(FeatureFlags.TheSecondAge); }
        }

        public static bool EnableRen
        {
            get { return m_flags.HasFlag(FeatureFlags.Renaissance); }
        }

        public static bool EnableThirdDawn
        {
            get { return m_flags.HasFlag(FeatureFlags.ThirdDawn); }
        }

        public static bool EnableLBR
        {
            get { return m_flags.HasFlag(FeatureFlags.LordBlackthornsRevenge); }
        }

        public static bool EnableAOS
        {
            get { return m_flags.HasFlag(FeatureFlags.AgeOfShadows); }
        }

        public static bool Enable6CharSlots
        {
            get { return m_flags.HasFlag(FeatureFlags.CharacterSlot6); }
        }

        public static bool EnableSE
        {
            get { return m_flags.HasFlag(FeatureFlags.SameraiEmpire); }
        }

        public static bool EnableML
        {
            get { return m_flags.HasFlag(FeatureFlags.MondainsLegacy); }
        }

        public static bool Enable8thSplash
        {
            get { return m_flags.HasFlag(FeatureFlags.Splash8); }
        }

        public static bool Enable9thSplash
        {
            get { return m_flags.HasFlag(FeatureFlags.Splash9); }
        }

        public static bool Enable10thAge
        {
            get { return m_flags.HasFlag(FeatureFlags.TenthAge); }
        }

        public static bool EnableMoreStorage
        {
            get { return m_flags.HasFlag(FeatureFlags.MoreStorage); }
        }

        public static bool Enable7CharSlots
        {
            get { return m_flags.HasFlag(FeatureFlags.TheSecondAge); }
        }

        public static bool Enable10thAgeFaces
        {
            get { return m_flags.HasFlag(FeatureFlags.TenthAgeFaces); }
        }

        public static bool EnableTrialAccount
        {
            get { return m_flags.HasFlag(FeatureFlags.TrialAccount); }
        }

        public static bool Enable11thAge
        {
            get { return m_flags.HasFlag(FeatureFlags.EleventhAge); }
        }

        public static bool EnableSA
        {
            get { return m_flags.HasFlag(FeatureFlags.StygianAbys); }
        }
        
        public static void SetFlags(FeatureFlags flags)
        {
            m_flags |= flags;
        }
    }
}