using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.Input
{
    public static class Macros
    {
        public static List<MacroDefinition> Definitions = new List<MacroDefinition>();
        public static PlayerMacros Player = new PlayerMacros();
        public static MacroDefinition NullMacro;

        static Macros()
        {
            Definitions.Add(NullMacro = new MacroDefinition("NONE", MacroType.None));

            // --------------------------------------------------------------------------------------------------------------
            // Action Macros
            // --------------------------------------------------------------------------------------------------------------
            Definitions.Add(new MacroDefinition("Say", MacroType.Say));
            Definitions.Add(new MacroDefinition("Emote", MacroType.Emote));
            Definitions.Add(new MacroDefinition("Whisper", MacroType.Whisper));
            Definitions.Add(new MacroDefinition("Yell", MacroType.Yell));
            Definitions.Add(new MacroDefinition("Move", MacroType.Move));
            Definitions.Add(new MacroDefinition("Toggle War/Peace", MacroType.ToggleWarPeace));
            Definitions.Add(new MacroDefinition("Paste", MacroType.Paste));
            Definitions.Add(new MacroDefinition("Open Gump", MacroType.OpenGump));
            Definitions.Add(new MacroDefinition("Close Gump", MacroType.CloseGump));
            Definitions.Add(new MacroDefinition("Minimize", MacroType.MinimizeWindow));
            Definitions.Add(new MacroDefinition("Maximize", MacroType.MaximizeWindow));
            // actionTypes.Add(new MacroDefinition("OpenDoor", MacroType.None)); - not yet implmented in client
            Definitions.Add(new MacroDefinition("UseSkill", MacroType.UseSkill));
            // actionTypes.Add(new MacroDefinition("LastSkill", MacroType.None)); - not yet implmented in client
            Definitions.Add(new MacroDefinition("CastSpell", MacroType.CastSpell));
            // actionTypes.Add(new MacroDefinition("LastSpell", MacroType.None)); - not yet added 
            // actionTypes.Add(new MacroDefinition("LastObject", MacroType.None)); - what does this do?
            Definitions.Add(new MacroDefinition("Bow", MacroType.EmoteBow));
            Definitions.Add(new MacroDefinition("Salute", MacroType.EmoteSalute));
            Definitions.Add(new MacroDefinition("QuitGame", MacroType.QuitGame));
            Definitions.Add(new MacroDefinition("AllNames", MacroType.ShowAllNames));
            Definitions.Add(new MacroDefinition("LastTarget", MacroType.LastTarget));
            Definitions.Add(new MacroDefinition("TargetSelf", MacroType.TargetSelf));
            Definitions.Add(new MacroDefinition("Arm/Disarm", MacroType.ArmDisarm));
            Definitions.Add(new MacroDefinition("Wait For Target", MacroType.WaitForTarget));
            Definitions.Add(new MacroDefinition("Target Next", MacroType.NextTarget));
            // actionTypes.Add(new MacroDefinition("AttackLast", MacroType.None)); - what does this do?
            // actionTypes.Add(new MacroDefinition("Delay", MacroType.Delay)); - not yet implmented in client
            /// what about continue?
            // actionTypes.Add(new MacroDefinition("CircleTrans", MacroType.None)); - not yet implmented in client
            Definitions.Add(new MacroDefinition("Close All Gumps", MacroType.CloseAllGumps));
            Definitions.Add(new MacroDefinition("AlwaysRun", MacroType.SetAlwaysRun));
            Definitions.Add(new MacroDefinition("Save Desktop", MacroType.None));
            // actionTypes.Add(new MacroDefinition("KillGumpOpen", MacroType.None)); - what does this do?

            // These haven't been implemented in the client yet:
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
            Definitions.Add(new MacroDefinition("NW (top)", MacroType.Move, (int)Direction.Up));
            Definitions.Add(new MacroDefinition("N (top-right)", MacroType.Move, (int)Direction.North));
            Definitions.Add(new MacroDefinition("NE (right)", MacroType.Move, (int)Direction.Right));
            Definitions.Add(new MacroDefinition("E (bottom-right)", MacroType.Move, (int)Direction.East));
            Definitions.Add(new MacroDefinition("SE (bottom)", MacroType.Move, (int)Direction.Down));
            Definitions.Add(new MacroDefinition("S (bottom-left)", MacroType.Move, (int)Direction.South));
            Definitions.Add(new MacroDefinition("SW (left)", MacroType.Move, (int)Direction.Left));
            Definitions.Add(new MacroDefinition("W (top-left)", MacroType.Move, (int)Direction.West));

            // --------------------------------------------------------------------------------------------------------------
            // Display Macros
            // --------------------------------------------------------------------------------------------------------------
            Definitions.Add(new MacroDefinition("Configuration", MacroType.OpenGump, (int)MacroDisplay.Configuration));
            Definitions.Add(new MacroDefinition("Paperdoll", MacroType.OpenGump, (int)MacroDisplay.Paperdoll));
            Definitions.Add(new MacroDefinition("Status", MacroType.OpenGump, (int)MacroDisplay.Status));
            Definitions.Add(new MacroDefinition("Journal", MacroType.OpenGump, (int)MacroDisplay.Journal));
            Definitions.Add(new MacroDefinition("Skills", MacroType.OpenGump, (int)MacroDisplay.Skills));
            Definitions.Add(new MacroDefinition("MageSpellbook", MacroType.OpenGump, (int)MacroDisplay.MageSpellbook));
            Definitions.Add(new MacroDefinition("Chat", MacroType.OpenGump, (int)MacroDisplay.Chat));
            Definitions.Add(new MacroDefinition("Backpack", MacroType.OpenGump, (int)MacroDisplay.Backpack));
            Definitions.Add(new MacroDefinition("Overview", MacroType.OpenGump, (int)MacroDisplay.Overview));
            Definitions.Add(new MacroDefinition("Mail", MacroType.OpenGump, (int)MacroDisplay.Mail));
            Definitions.Add(new MacroDefinition("PartyManifest", MacroType.OpenGump, (int)MacroDisplay.PartyManifest));
            Definitions.Add(new MacroDefinition("PartyChat", MacroType.OpenGump, (int)MacroDisplay.PartyChat));
            Definitions.Add(new MacroDefinition("NecroSpellbook", MacroType.OpenGump, (int)MacroDisplay.NecroSpellbook));
            Definitions.Add(new MacroDefinition("PaladinSpellbook", MacroType.OpenGump, (int)MacroDisplay.PaladinSpellbook));
            Definitions.Add(new MacroDefinition("CombatBook", MacroType.OpenGump, (int)MacroDisplay.CombatBook));
            Definitions.Add(new MacroDefinition("BushidoSpellbook", MacroType.OpenGump, (int)MacroDisplay.BushidoSpellbook));
            Definitions.Add(new MacroDefinition("NinjitsuSpellbook", MacroType.OpenGump, (int)MacroDisplay.NinjitsuSpellbook));
            Definitions.Add(new MacroDefinition("Guild", MacroType.OpenGump, (int)MacroDisplay.Guild));
            Definitions.Add(new MacroDefinition("SpellWeavingSpellbook", MacroType.OpenGump, (int)MacroDisplay.SpellWeavingSpellbook));
            Definitions.Add(new MacroDefinition("QuestLog", MacroType.OpenGump, (int)MacroDisplay.QuestLog));

            // --------------------------------------------------------------------------------------------------------------
            // Use Skill Macros
            // --------------------------------------------------------------------------------------------------------------
            foreach (KeyValuePair<int, SkillEntry> pair in PlayerState.Skills.List)
            {
                SkillEntry skill = pair.Value;
                if (skill.HasUseButton)
                {
                    Definitions.Add(new MacroDefinition(skill.Name, MacroType.UseSkill, skill.Index));
                }
            }

            // --------------------------------------------------------------------------------------------------------------
            // Cast Spell Macros
            // --------------------------------------------------------------------------------------------------------------
            foreach (SpellDefinition spell in SpellsMagery.Spells)
            {
                Definitions.Add(new MacroDefinition(spell.Name, MacroType.CastSpell, spell.ID);
            }

            // --------------------------------------------------------------------------------------------------------------
            // Arm/Disarm Macros
            // --------------------------------------------------------------------------------------------------------------
            Definitions.Add(new MacroDefinition("Main Hand", MacroType.ArmDisarm, (int)MacroArmDisarm.MainHand));
            Definitions.Add(new MacroDefinition("Off Hand", MacroType.ArmDisarm, (int)MacroArmDisarm.OffHand));
        }

        public static string GetMacroName(Macro macro)
        {
            foreach (MacroDefinition def in Definitions)
            {
                if (macro.Type == def.Type && macro.Index == def.Index)
                    return def.Name;
            }
            return "UnknownMacro";
        }

        public static MacroDefinition GetDefinition(MacroType type, int index)
        {
            foreach (MacroDefinition def in Definitions)
            {
                if (type == def.Type && index == def.Index)
                    return def;
            }
            return NullMacro;
        }

        public static void RunMacroAction(MacroAction action)
        {
            WorldModel world = ServiceRegistry.GetService<WorldModel>();
            INetworkClient m_Network = ServiceRegistry.GetService<INetworkClient>();
            for (int i = 0; i < macro.actionList.Count; i++)
            {
                int CurrentActionID = macro.actionList[i].actionID;
                if (i == 0 && CurrentActionID == 0)//first action not selected its important
                {
                    //WARNING
                    break;
                }
                int CurrentValueID = macro.actionList[i].valueID;
                MacroDefinition action = Definitions[CurrentActionID];
                string valueText = macro.actionList[i].valueText;

                if (action.Type == MacroType.Skill)
                {
                    MacroDefinition valueObj = useSkills[CurrentValueID];
                    world.Interaction.UseSkill(CurrentValueID);
                }
                else if (action.Type == MacroType.Spell)
                {
                    MacroDefinition valueObj = castSpell[CurrentValueID];
                    world.Interaction.CastSpell(CurrentValueID);
                }
                else if (action.Type == MacroType.Display)
                {
                    MacroDefinition valueObj = Definitions[CurrentValueID];
                }
                else if (action.Type == MacroType.Move)
                {
                    MacroDefinition valueObj = Moves[CurrentValueID];
                }
                else if (action.Type == MacroType.ArmDisarm)
                {
                    MacroDefinition valueObj = armDisarm[macro.actionList[i].valueID];
                }
                else if ((action.Type == MacroType.None) || action.Name == "Delay")//delay using textentry,so it is EXCEPTION FOR IT
                {
                    //only action list event (DONT USE (valueID or ValueText) variables here!!!!!)
                    //for example: last target, last object, target self
                    //
                    switch (CurrentActionID)
                    {
                        case 17://LAST OBJECT CALLING

                            break;

                        case 22://LAST TARGET CALLING

                            break;

                        case 23://TARGET SELF CALLING
                            Mobile _self = WorldModel.Entities.GetPlayerEntity();
                            m_Network.Send(new TargetObjectPacket(_self, 486));
                            break;

                        case 24://ARM-DISARM CALLING

                            break;

                        case 25://WAITFORTARG CALLING

                            break;

                        case 26://TARGETNEXT CALLING

                            break;

                        case 27://ATTACK LAST CALLING

                            break;

                        case 28:
                            int delay;
                            bool result = int.TryParse(valueText, out delay);
                            if (result)
                            {
                                Thread.Sleep(delay);
                            }
                            break;
                    }
                }
                else if (CurrentActionID == 1)//say
                {
                    world.Interaction.SendSpeech(valueText, ChatMode.Default);
                }
                else if (CurrentActionID == 2)//emote
                {
                    world.Interaction.SendSpeech(valueText, ChatMode.Emote);
                }
                else if (CurrentActionID == 3)//whisper
                {
                    world.Interaction.SendSpeech(valueText, ChatMode.Whisper);
                }
                else if (CurrentActionID == 4)//yell //NOT IMPLEMENTED ???????????
                {
                    //YELL FUNCTION
                }
                //dont use any code here all done........
            }
        }
    }
}
