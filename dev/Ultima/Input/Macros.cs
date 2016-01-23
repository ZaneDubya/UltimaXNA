using System.Collections.Generic;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.Input
{
    public static class Macros
    {
        public static List<MacroDefinition> Types = new List<MacroDefinition>();
        public static List<MacroDefinition> Skills = new List<MacroDefinition>();
        public static List<MacroDefinition> Spells = new List<MacroDefinition>();
        public static List<MacroDefinition> Gumps = new List<MacroDefinition>();
        public static List<MacroDefinition> Moves = new List<MacroDefinition>();
        public static List<MacroDefinition> ArmDisarms = new List<MacroDefinition>();

        public static PlayerMacros Player = new PlayerMacros();
        public static MacroDefinition NullMacro;

        static Macros()
        {
            Types.Add(NullMacro = new MacroDefinition("NONE", MacroType.None));

            // --------------------------------------------------------------------------------------------------------------
            // Text Macros
            // --------------------------------------------------------------------------------------------------------------
            Types.Add(new MacroDefinition("Say", MacroType.Say));
            Types.Add(new MacroDefinition("Emote", MacroType.Emote));
            Types.Add(new MacroDefinition("Whisper", MacroType.Whisper));
            Types.Add(new MacroDefinition("Yell", MacroType.Yell));

            // --------------------------------------------------------------------------------------------------------------
            // Action Macros with sub-values
            // --------------------------------------------------------------------------------------------------------------
            Types.Add(new MacroDefinition("UseSkill", MacroType.UseSkill));
            Types.Add(new MacroDefinition("CastSpell", MacroType.CastSpell));
            Types.Add(new MacroDefinition("Open Gump", MacroType.OpenGump));
            Types.Add(new MacroDefinition("Close Gump", MacroType.CloseGump));
            Types.Add(new MacroDefinition("Move", MacroType.Move));
            Types.Add(new MacroDefinition("Arm/Disarm", MacroType.ArmDisarm));

            // --------------------------------------------------------------------------------------------------------------
            // Action Macros with no sub-values
            // --------------------------------------------------------------------------------------------------------------
            Types.Add(new MacroDefinition("Toggle War/Peace", MacroType.ToggleWarPeace));
            Types.Add(new MacroDefinition("Paste", MacroType.Paste));
            Types.Add(new MacroDefinition("Minimize", MacroType.MinimizeWindow));
            Types.Add(new MacroDefinition("Maximize", MacroType.MaximizeWindow));
            Types.Add(new MacroDefinition("Bow", MacroType.EmoteBow));
            Types.Add(new MacroDefinition("Salute", MacroType.EmoteSalute));
            Types.Add(new MacroDefinition("QuitGame", MacroType.QuitGame));
            Types.Add(new MacroDefinition("AllNames", MacroType.ShowAllNames));
            Types.Add(new MacroDefinition("LastTarget", MacroType.LastTarget));
            Types.Add(new MacroDefinition("TargetSelf", MacroType.TargetSelf));
            Types.Add(new MacroDefinition("Wait For Target", MacroType.WaitForTarget));
            Types.Add(new MacroDefinition("Target Next", MacroType.NextTarget));
            Types.Add(new MacroDefinition("Close All Gumps", MacroType.CloseAllGumps));
            Types.Add(new MacroDefinition("AlwaysRun", MacroType.SetAlwaysRun));
            Types.Add(new MacroDefinition("Delay", MacroType.Delay));

            // These haven't been implemented in the client yet:
            // what about continue?
            // actionTypes.Add(new MacroDefinition("AttackLast", MacroType.None)); - what does this do?
            // actionTypes.Add(new MacroDefinition("Delay", MacroType.Delay)); - not yet implmented in client
            // actionTypes.Add(new MacroDefinition("CircleTrans", MacroType.None)); - not yet implmented in client
            // Definitions.Add(new MacroDefinition("Save Desktop", MacroType.None));
            // actionTypes.Add(new MacroDefinition("KillGumpOpen", MacroType.None)); - what does this do?
            // actionTypes.Add(new MacroDefinition("OpenDoor", MacroType.None)); - not yet implmented in client
            // actionTypes.Add(new MacroDefinition("LastSkill", MacroType.None)); - not yet implmented in client
            // actionTypes.Add(new MacroDefinition("LastSpell", MacroType.None)); - not yet added 
            // actionTypes.Add(new MacroDefinition("LastObject", MacroType.None)); - what does this do?
            // actionTypes.Add(new MacroDefinition("PrimaryAbility", MacroType.None));
            // actionTypes.Add(new MacroDefinition("SecondaryAbility", MacroType.None));
            // actionTypes.Add(new MacroDefinition("EquipLastWeapon", MacroType.None));
            // actionTypes.Add(new MacroDefinition("SetUpdateRange", MacroType.None));
            // actionTypes.Add(new MacroDefinition("ModifyUpdateRange", MacroType.None));
            // actionTypes.Add(new MacroDefinition("IncreaseUpdateRange", MacroType.None));
            // actionTypes.Add(new MacroDefinition("DecreaseUpdateRange", MacroType.None));
            // actionTypes.Add(new MacroDefinition("MaxUpdateRange", MacroType.None));
            // actionTypes.Add(new MacroDefinition("MinUpdateRange", MacroType.None));
            // actionTypes.Add(new MacroDefinition("DefaultUpdateRange", MacroType.None));
            // actionTypes.Add(new MacroDefinition("UpdateRangeInfo", MacroType.None));
            // actionTypes.Add(new MacroDefinition("EnableRangeColor", MacroType.None));
            // actionTypes.Add(new MacroDefinition("DisableRangeColor", MacroType.None));
            // actionTypes.Add(new MacroDefinition("ToggleRangeColor", MacroType.None));
            // actionTypes.Add(new MacroDefinition("InvokeVirtue", MacroType.None));
            // actionTypes.Add(new MacroDefinition("SelectNext", MacroType.None));
            // actionTypes.Add(new MacroDefinition("SelectPrevious", MacroType.None));
            // actionTypes.Add(new MacroDefinition("SelectNearest", MacroType.None));
            // actionTypes.Add(new MacroDefinition("AttackSelectedTarget", MacroType.None));
            // actionTypes.Add(new MacroDefinition("UseSelectedTarget", MacroType.None));
            // actionTypes.Add(new MacroDefinition("CurrentTarget", MacroType.None));
            // actionTypes.Add(new MacroDefinition("Target System On/Off", MacroType.None));
            // actionTypes.Add(new MacroDefinition("ToggleBuffIconWindow", MacroType.None));
            // actionTypes.Add(new MacroDefinition("BandageSelfBandageTarget", MacroType.None));

            // --------------------------------------------------------------------------------------------------------------
            // Move Macros
            // --------------------------------------------------------------------------------------------------------------
            Moves.Add(new MacroDefinition("NW (top)", MacroType.Move, (int)Direction.Up));
            Moves.Add(new MacroDefinition("N (top-right)", MacroType.Move, (int)Direction.North));
            Moves.Add(new MacroDefinition("NE (right)", MacroType.Move, (int)Direction.Right));
            Moves.Add(new MacroDefinition("E (bottom-right)", MacroType.Move, (int)Direction.East));
            Moves.Add(new MacroDefinition("SE (bottom)", MacroType.Move, (int)Direction.Down));
            Moves.Add(new MacroDefinition("S (bottom-left)", MacroType.Move, (int)Direction.South));
            Moves.Add(new MacroDefinition("SW (left)", MacroType.Move, (int)Direction.Left));
            Moves.Add(new MacroDefinition("W (top-left)", MacroType.Move, (int)Direction.West));

            // --------------------------------------------------------------------------------------------------------------
            // Gump Macros
            // --------------------------------------------------------------------------------------------------------------
            Gumps.Add(new MacroDefinition("Configuration", MacroType.OpenGump, (int)MacroDisplay.Configuration));
            Gumps.Add(new MacroDefinition("Paperdoll", MacroType.OpenGump, (int)MacroDisplay.Paperdoll));
            Gumps.Add(new MacroDefinition("Status", MacroType.OpenGump, (int)MacroDisplay.Status));
            Gumps.Add(new MacroDefinition("Journal", MacroType.OpenGump, (int)MacroDisplay.Journal));
            Gumps.Add(new MacroDefinition("Skills", MacroType.OpenGump, (int)MacroDisplay.Skills));
            Gumps.Add(new MacroDefinition("MageSpellbook", MacroType.OpenGump, (int)MacroDisplay.MageSpellbook));
            Gumps.Add(new MacroDefinition("Chat", MacroType.OpenGump, (int)MacroDisplay.Chat));
            Gumps.Add(new MacroDefinition("Backpack", MacroType.OpenGump, (int)MacroDisplay.Backpack));
            Gumps.Add(new MacroDefinition("Overview", MacroType.OpenGump, (int)MacroDisplay.Overview));
            Gumps.Add(new MacroDefinition("Mail", MacroType.OpenGump, (int)MacroDisplay.Mail));
            Gumps.Add(new MacroDefinition("PartyManifest", MacroType.OpenGump, (int)MacroDisplay.PartyManifest));
            Gumps.Add(new MacroDefinition("PartyChat", MacroType.OpenGump, (int)MacroDisplay.PartyChat));
            Gumps.Add(new MacroDefinition("NecroSpellbook", MacroType.OpenGump, (int)MacroDisplay.NecroSpellbook));
            Gumps.Add(new MacroDefinition("PaladinSpellbook", MacroType.OpenGump, (int)MacroDisplay.PaladinSpellbook));
            Gumps.Add(new MacroDefinition("CombatBook", MacroType.OpenGump, (int)MacroDisplay.CombatBook));
            Gumps.Add(new MacroDefinition("BushidoSpellbook", MacroType.OpenGump, (int)MacroDisplay.BushidoSpellbook));
            Gumps.Add(new MacroDefinition("NinjitsuSpellbook", MacroType.OpenGump, (int)MacroDisplay.NinjitsuSpellbook));
            Gumps.Add(new MacroDefinition("Guild", MacroType.OpenGump, (int)MacroDisplay.Guild));
            Gumps.Add(new MacroDefinition("SpellWeavingSpellbook", MacroType.OpenGump, (int)MacroDisplay.SpellWeavingSpellbook));
            Gumps.Add(new MacroDefinition("QuestLog", MacroType.OpenGump, (int)MacroDisplay.QuestLog));

            // --------------------------------------------------------------------------------------------------------------
            // Use Skill Macros
            // --------------------------------------------------------------------------------------------------------------
            foreach (KeyValuePair<int, SkillEntry> pair in PlayerState.Skills.List)
            {
                SkillEntry skill = pair.Value;
                if (skill.HasUseButton)
                {
                    Skills.Add(new MacroDefinition(skill.Name, MacroType.UseSkill, skill.Index));
                }
            }

            // --------------------------------------------------------------------------------------------------------------
            // Cast Spell Macros
            // --------------------------------------------------------------------------------------------------------------
            foreach (SpellDefinition spell in SpellsMagery.Spells)
            {
                Spells.Add(new MacroDefinition(spell.Name, MacroType.CastSpell, spell.ID));
            }

            // --------------------------------------------------------------------------------------------------------------
            // Arm/Disarm Macros
            // --------------------------------------------------------------------------------------------------------------
            ArmDisarms.Add(new MacroDefinition("Main Hand", MacroType.ArmDisarm, (int)MacroArmDisarm.MainHand));
            ArmDisarms.Add(new MacroDefinition("Off Hand", MacroType.ArmDisarm, (int)MacroArmDisarm.OffHand));
        }
    }
}
