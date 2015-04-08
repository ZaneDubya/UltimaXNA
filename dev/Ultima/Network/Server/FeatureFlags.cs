using System;

namespace UltimaXNA.UltimaPackets.Server
{
    [Flags]
    public enum FeatureFlags : uint
    {
        TheSecondAge = 0x1,
        Renaissance = 0x2,
        ThirdDawn = 0x4,
        LordBlackthornsRevenge = 0x8,
        AgeOfShadows = 0x10,
        CharacterSlot6 = 0x20,
        SameraiEmpire = 0x40,
        MondainsLegacy = 0x80,
        Splash8 = 0x100,
        Splash9 = 0x200,
        TenthAge = 0x400,
        MoreStorage = 0x800,
        CharacterSlot7 = 0x1000,
        TenthAgeFaces = 0x2000,
        TrialAccount = 0x4000,
        EleventhAge = 0x8000,
        StygianAbys = 0x10000,
    }
}