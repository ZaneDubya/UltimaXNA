namespace UltimaXNA.Ultima.Input
{
    public enum MacroType : int
    {
        None,
        // text:
        Say,
        Emote,
        Whisper,
        Yell,

        // these types have sub values:
        UseSkill, 
        CastSpell, 
        OpenGump, 
        CloseGump,
        Move, 
        ArmDisarm,

        // actions, no sub values:
        ToggleWarPeace,
        Paste,
        MinimizeWindow,
        MaximizeWindow,
        EmoteBow,
        EmoteSalute,
        QuitGame,
        ShowAllNames,
        LastTarget,
        TargetSelf,
        WaitForTarget,
        NextTarget,
        CloseAllGumps,
        SetAlwaysRun,
        Delay,
    }
}
