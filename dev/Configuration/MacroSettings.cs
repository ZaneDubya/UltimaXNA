using System.Collections.Generic;
using System.Threading;
using UltimaXNA.Configuration.Properties;
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Core.Configuration
{
    public class MacroSettings : ASettingsSection
    {
        public XKey UserMacros;

        public static string SectionName = "macro";

        public List<Macro> actionTypes = new List<Macro>();
        public List<Macro> Moves = new List<Macro>();
        public List<Macro> Displays = new List<Macro>();
        public List<Macro> useSkills = new List<Macro>();
        public List<Macro> castSpell = new List<Macro>();
        public List<Macro> armDisarm = new List<Macro>();

        public MacroSettings()
        {
            //MacroType enum for dropdownlist controll
            //ACTION TYPES ADDED
            actionTypes.Add(new Macro() { Name = "NONE", Type = MacroType.None, isActionType = true });//0.
            actionTypes.Add(new Macro() { Name = "Say", Type = MacroType.Text, isActionType = true });//1
            actionTypes.Add(new Macro() { Name = "Emote", Type = MacroType.Text, isActionType = true });//2
            actionTypes.Add(new Macro() { Name = "Whisper", Type = MacroType.Text, isActionType = true });//3
            actionTypes.Add(new Macro() { Name = "Yell", Type = MacroType.Text, isActionType = true });
            actionTypes.Add(new Macro() { Name = "Walk", Type = MacroType.Move, isActionType = true });
            actionTypes.Add(new Macro() { Name = "War/Peace", Type = MacroType.None, isActionType = true });//6.
            actionTypes.Add(new Macro() { Name = "Paste", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "Open", Type = MacroType.Display, isActionType = true });
            actionTypes.Add(new Macro() { Name = "Close", Type = MacroType.Display, isActionType = true });
            actionTypes.Add(new Macro() { Name = "Minimize", Type = MacroType.Display, isActionType = true });
            actionTypes.Add(new Macro() { Name = "Maximize", Type = MacroType.Display, isActionType = true });
            actionTypes.Add(new Macro() { Name = "OpenDoor", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "UseSkill", Type = MacroType.Skill, isActionType = true });
            actionTypes.Add(new Macro() { Name = "LastSkill", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "CastSpell", Type = MacroType.Spell, isActionType = true });//15.
            actionTypes.Add(new Macro() { Name = "LastSpell", Type = MacroType.None, isActionType = true });//16
            actionTypes.Add(new Macro() { Name = "LastObject", Type = MacroType.None, isActionType = true });//17
            actionTypes.Add(new Macro() { Name = "Bow", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "Salute", Type = MacroType.None, isActionType = true });//19
            actionTypes.Add(new Macro() { Name = "QuitGame", Type = MacroType.None, isActionType = true });//20
            actionTypes.Add(new Macro() { Name = "AllNames", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "LastTarget", Type = MacroType.None, isActionType = true });//22.
            actionTypes.Add(new Macro() { Name = "TargetSelf", Type = MacroType.None, isActionType = true });//23
            actionTypes.Add(new Macro() { Name = "Arm/Disarm", Type = MacroType.ArmDisarm, isActionType = true });
            actionTypes.Add(new Macro() { Name = "WaitForTarg", Type = MacroType.None, isActionType = true });//25
            actionTypes.Add(new Macro() { Name = "TargetNext", Type = MacroType.None, isActionType = true });//26
            actionTypes.Add(new Macro() { Name = "AttackLast", Type = MacroType.None, isActionType = true });//27
            actionTypes.Add(new Macro() { Name = "Delay", Type = MacroType.Text, isActionType = true });//28
            ///I don't know about continue
            actionTypes.Add(new Macro() { Name = "CircleTrans", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "CloseGumps", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "AlwaysRun", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "SaveDesktop", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "KillGumpOpen", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "PrimaryAbility", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "SecondaryAbility", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "EquipLastWeapon", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "SetUpdateRange", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "ModifyUpdateRange", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "IncreaseUpdateRange", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "DecreaseUpdateRange", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "MaxUpdateRange", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "MinUpdateRange", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "DefaultUpdateRange", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "UpdateRangeInfo", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "EnableRangeColor", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "DisableRangeColor", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "ToggleRangeColor", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "InvokeVirtue", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "SelectNext", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "SelectPrevious", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "SelectNearest", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "AttackSelectedTarget", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "UseSelectedTarget", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "CurrentTarget", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "TargetSystemOn/Off", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "ToggleBuffIconWindow", Type = MacroType.None, isActionType = true });
            actionTypes.Add(new Macro() { Name = "BandageSelfBandageTarget", Type = MacroType.None, isActionType = true });
            //ACTION TYPES FINISHED

            //MOVE POSITIONS ADDED
            //Positions.Add(new Macro() { Name = "NONE", Type = MacroType.Move });
            Moves.Add(new Macro() { Name = "NW (top)", Type = MacroType.Move });
            Moves.Add(new Macro() { Name = "N (top-right)", Type = MacroType.Move });
            Moves.Add(new Macro() { Name = "NE (right)", Type = MacroType.Move });
            Moves.Add(new Macro() { Name = "E (bottom-right)", Type = MacroType.Move });
            Moves.Add(new Macro() { Name = "SE (bottom)", Type = MacroType.Move });
            Moves.Add(new Macro() { Name = "S (bottom-left)", Type = MacroType.Move });
            Moves.Add(new Macro() { Name = "SW (left)", Type = MacroType.Move });
            Moves.Add(new Macro() { Name = "W (top-left)", Type = MacroType.Move });
            //MOVE POSITIONS FINISHED

            //DISPLAYS ADDED
            Displays.Add(new Macro() { Name = "Configuration", Type = MacroType.Display });//0
            Displays.Add(new Macro() { Name = "Paperdoll", Type = MacroType.Display });//1
            Displays.Add(new Macro() { Name = "Status", Type = MacroType.Display });//2
            Displays.Add(new Macro() { Name = "Journal", Type = MacroType.Display });//3
            Displays.Add(new Macro() { Name = "Skills", Type = MacroType.Display });//4
            Displays.Add(new Macro() { Name = "MageSpellbook", Type = MacroType.Display });
            Displays.Add(new Macro() { Name = "Chat", Type = MacroType.Display });//6
            Displays.Add(new Macro() { Name = "Backpack", Type = MacroType.Display });//7
            Displays.Add(new Macro() { Name = "Overview", Type = MacroType.Display });//8
            Displays.Add(new Macro() { Name = "Mail", Type = MacroType.Display });
            Displays.Add(new Macro() { Name = "PartyManifest", Type = MacroType.Display });
            Displays.Add(new Macro() { Name = "PartyChat", Type = MacroType.Display });
            Displays.Add(new Macro() { Name = "NecroSpellbook", Type = MacroType.Display });
            Displays.Add(new Macro() { Name = "PaladinSpellbook", Type = MacroType.Display });//13
            Displays.Add(new Macro() { Name = "CombatBook", Type = MacroType.Display });
            Displays.Add(new Macro() { Name = "BushidoSpellbook", Type = MacroType.Display });
            Displays.Add(new Macro() { Name = "NinjitsuSpellbook", Type = MacroType.Display });
            Displays.Add(new Macro() { Name = "Guild", Type = MacroType.Display });
            Displays.Add(new Macro() { Name = "SpellWeavingSpellbook", Type = MacroType.Display });//18
            Displays.Add(new Macro() { Name = "QuestLog", Type = MacroType.Display });//19
            //DISPLAYS FINISHED

            //USESKILLS ADDED
            useSkills.Add(new Macro() { Name = "Anatomy", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Animal Lore", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Animal Taming", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Arms Lore", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Begging", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Cartography", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Detecting Hidden", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Discordance", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Evaluating Intelligence", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Forensic Evaluation", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Hiding", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Inscription", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Item Identification", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Meditation", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Peacemaking", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Poisoning", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Provocation", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Remove Trap", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Spirit Speak", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Stealing", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Stealth", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Taste Identification", Type = MacroType.Skill });
            useSkills.Add(new Macro() { Name = "Tracking", Type = MacroType.Skill });
            //USESKILLS FINISHED

            //CASTSPELL ADDED
            castSpell.Add(new Macro() { Name = "Clumsy", Type = MacroType.Spell });//0.
            castSpell.Add(new Macro() { Name = "Create Food", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Feeblemind", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Heal", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Magic Arrow", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Night Sight", Type = MacroType.Spell });//5
            castSpell.Add(new Macro() { Name = "Reactive Armor", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "WeakenAgility", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Cunning", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Cure", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Harm", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Magic Trap", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Magic Untrap", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Protection", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Strength", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Bless", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Firball", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Magic Lock", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Poison", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Telekinesis", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Teleport", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Magic Unlock", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Wall of Stone", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Archcure", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "ArchprotectionCurse", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Fire Field", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Greater Heal", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Lightning", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Mana Drain", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Recall", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Blade Spirit", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Dispel Field", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Incognito", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Magic Reflection", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Mind Blast", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Paralyze", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Poison Field", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Summon Creature", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Dispel", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Energy Bolt", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Explosion", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Invisibility", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Mark", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Mass Curse", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Paralyze Field", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Reveal", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Chain Lightning", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Energy Field", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Flamestrike", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Gate Travel", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Mana Vampire", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Mass Dispell", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Meteor Swarm", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Polymorph", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Earthquake", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Energy Vortex", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Resurrection", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Summon Air", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Summon Demon", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Summon Earth", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Summon Fire", Type = MacroType.Spell });
            castSpell.Add(new Macro() { Name = "Summon Water", Type = MacroType.Spell });
            //CASTSPELL FINISHED

            //ARMDISARM ADDED
            armDisarm.Add(new Macro() { Name = "Left Hand", Type = MacroType.ArmDisarm });
            armDisarm.Add(new Macro() { Name = "Right Hand", Type = MacroType.ArmDisarm });
            //ARMDISARM FINISHED
        }

        public void UseMacro(XMacro macro)
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
                Macro action = actionTypes[CurrentActionID];
                string valueText = macro.actionList[i].valueText;

                if (action.Type == MacroType.Skill)
                {
                    Macro valueObj = useSkills[CurrentValueID];
                    world.Interaction.UseSkill(CurrentValueID + 1);//i can't do it (+1 for index starting 0)
                }
                else if (action.Type == MacroType.Spell)
                {
                    Macro valueObj = castSpell[CurrentValueID];
                    world.Interaction.CastSpell(CurrentValueID + 1);//i can't do it (+1 for index starting 0)
                }
                else if (action.Type == MacroType.Display)
                {
                    Macro valueObj = Displays[CurrentValueID];
                }
                else if (action.Type == MacroType.Move)
                {
                    Macro valueObj = Moves[CurrentValueID];
                }
                else if (action.Type == MacroType.ArmDisarm)
                {
                    Macro valueObj = armDisarm[macro.actionList[i].valueID];
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

                        case 22://TARGET SELF CALLING

                            break;

                        case 23://ARM-DISARM CALLING

                            break;

                        case 24://WAITFORTARG CALLING

                            break;

                        case 25://TARGETNEXT CALLING

                            break;

                        case 26://ATTACK LAST CALLING

                            break;

                        case 27:

                            break;

                        case 28:
                            int delay;
                            bool result = int.TryParse(valueText, out delay);
                            if (result)
                            {
                                Thread.Sleep(delay);//maybe can't calculating the correct time
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