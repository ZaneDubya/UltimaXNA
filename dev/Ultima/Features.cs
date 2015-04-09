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
        private static FeatureFlags _flags;

        public static bool EnableT2A
        {
            get { return _flags.HasFlag(FeatureFlags.TheSecondAge); }
        }

        public static bool EnableRen
        {
            get { return _flags.HasFlag(FeatureFlags.Renaissance); }
        }

        public static bool EnableThirdDawn
        {
            get { return _flags.HasFlag(FeatureFlags.ThirdDawn); }
        }

        public static bool EnableLBR
        {
            get { return _flags.HasFlag(FeatureFlags.LordBlackthornsRevenge); }
        }

        public static bool EnableAOS
        {
            get { return _flags.HasFlag(FeatureFlags.AgeOfShadows); }
        }

        public static bool Enable6CharSlots
        {
            get { return _flags.HasFlag(FeatureFlags.CharacterSlot6); }
        }

        public static bool EnableSE
        {
            get { return _flags.HasFlag(FeatureFlags.SameraiEmpire); }
        }

        public static bool EnableML
        {
            get { return _flags.HasFlag(FeatureFlags.MondainsLegacy); }
        }

        public static bool Enable8thSplash
        {
            get { return _flags.HasFlag(FeatureFlags.Splash8); }
        }

        public static bool Enable9thSplash
        {
            get { return _flags.HasFlag(FeatureFlags.Splash9); }
        }

        public static bool Enable10thAge
        {
            get { return _flags.HasFlag(FeatureFlags.TenthAge); }
        }

        public static bool EnableMoreStorage
        {
            get { return _flags.HasFlag(FeatureFlags.MoreStorage); }
        }

        public static bool Enable7CharSlots
        {
            get { return _flags.HasFlag(FeatureFlags.TheSecondAge); }
        }

        public static bool Enable10thAgeFaces
        {
            get { return _flags.HasFlag(FeatureFlags.TenthAgeFaces); }
        }

        public static bool EnableTrialAccount
        {
            get { return _flags.HasFlag(FeatureFlags.TrialAccount); }
        }

        public static bool Enable11thAge
        {
            get { return _flags.HasFlag(FeatureFlags.EleventhAge); }
        }

        public static bool EnableSA
        {
            get { return _flags.HasFlag(FeatureFlags.StygianAbys); }
        }
        
        public static void SetFlags(FeatureFlags flags)
        {
            _flags |= flags;
        }
    }
}