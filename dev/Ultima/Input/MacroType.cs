namespace UltimaXNA.Ultima.Input
{
    public enum MacroType : int
    {
        None,
        // these types have sub values:
        UseSkill, 
        CastSpell, 
        OpenGump, 
        CloseGump,
        Move, 
        ArmDisarm, 

        // text:
        Say,
        Emote,
        Whisper,
        Yell,

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
        Delay,
        CloseAllGumps,
        SetAlwaysRun
    }
}
