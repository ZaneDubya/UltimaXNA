/***************************************************************************
 *   MacroSettings.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

#region usings
using System.Collections.Generic;
using System.Threading;
using UltimaXNA.Configuration.Macros;
using UltimaXNA.Configuration.Properties;
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;
#endregion

namespace UltimaXNA.Core.Configuration
{
    public class MacroSettings : ASettingsSection
    {
        public XKey UserMacros;

        public static string SectionName = "macro";

        public List<MacroDefinition> actionTypes = new List<MacroDefinition>();
        public List<MacroDefinition> Moves = new List<MacroDefinition>();
        public List<MacroDefinition> Displays = new List<MacroDefinition>();
        public List<MacroDefinition> useSkills = new List<MacroDefinition>();
        public List<MacroDefinition> castSpell = new List<MacroDefinition>();
        public List<MacroDefinition> armDisarm = new List<MacroDefinition>();

        public MacroSettings()
        {
            // ==============================================================================================================
            // Action Macros
            // ==============================================================================================================
            actionTypes.Add(new MacroDefinition("NONE", MacroType.None));
            actionTypes.Add(new MacroDefinition() { Name = "Say", Type = MacroType.Say});
            actionTypes.Add(new MacroDefinition() { Name = "Emote", Type = MacroType.Emote});
            actionTypes.Add(new MacroDefinition() { Name = "Whisper", Type = MacroType.Whisper });
            actionTypes.Add(new MacroDefinition() { Name = "Yell", Type = MacroType.Yell});
            actionTypes.Add(new MacroDefinition() { Name = "Move", Type = MacroType.Move});
            actionTypes.Add(new MacroDefinition() { Name = "Toggle War/Peace", Type = MacroType.ToggleWarPeace});
            actionTypes.Add(new MacroDefinition() { Name = "Paste", Type = MacroType.Paste});
            actionTypes.Add(new MacroDefinition() { Name = "Open Gump", Type = MacroType.OpenGump});
            actionTypes.Add(new MacroDefinition() { Name = "Close Gump", Type = MacroType.CloseGump});
            actionTypes.Add(new MacroDefinition() { Name = "Minimize", Type = MacroType.MinimizeWindow });
            actionTypes.Add(new MacroDefinition() { Name = "Maximize", Type = MacroType.MaximizeWindow});
            // actionTypes.Add(new MacroDefinition() { Name = "OpenDoor", Type = MacroType.None}); - not yet implmented in client
            actionTypes.Add(new MacroDefinition() { Name = "UseSkill", Type = MacroType.UseSkill});
            // actionTypes.Add(new MacroDefinition() { Name = "LastSkill", Type = MacroType.None}); - not yet implmented in client
            actionTypes.Add(new MacroDefinition() { Name = "CastSpell", Type = MacroType.CastSpell});
            // actionTypes.Add(new MacroDefinition() { Name = "LastSpell", Type = MacroType.None}); - not yet added 
            // actionTypes.Add(new MacroDefinition() { Name = "LastObject", Type = MacroType.None}); - what does this do?
            actionTypes.Add(new MacroDefinition() { Name = "Bow", Type = MacroType.EmoteBow});
            actionTypes.Add(new MacroDefinition() { Name = "Salute", Type = MacroType.EmoteSalute});
            actionTypes.Add(new MacroDefinition() { Name = "QuitGame", Type = MacroType.QuitGame});//20
            actionTypes.Add(new MacroDefinition() { Name = "AllNames", Type = MacroType.ShowAllNames});
            actionTypes.Add(new MacroDefinition() { Name = "LastTarget", Type = MacroType.LastTarget });//22.
            actionTypes.Add(new MacroDefinition() { Name = "TargetSelf", Type = MacroType.TargetSelf});//23
            actionTypes.Add(new MacroDefinition() { Name = "Arm/Disarm", Type = MacroType.ArmDisarm});
            actionTypes.Add(new MacroDefinition() { Name = "Wait For Target", Type = MacroType.WaitForTarget });//25
            actionTypes.Add(new MacroDefinition() { Name = "Target Next", Type = MacroType.NextTarget });//26
            // actionTypes.Add(new MacroDefinition() { Name = "AttackLast", Type = MacroType.None}); - what does this do?
            // actionTypes.Add(new MacroDefinition() { Name = "Delay", Type = MacroType.Delay}); - not yet implmented in client
            ///I don't know about continue
            // actionTypes.Add(new MacroDefinition() { Name = "CircleTrans", Type = MacroType.None}); - not yet implmented in client
            actionTypes.Add(new MacroDefinition() { Name = "Close All Gumps", Type = MacroType.CloseAllGumps });
            actionTypes.Add(new MacroDefinition() { Name = "AlwaysRun", Type = MacroType.SetAlwaysRun });
            actionTypes.Add(new MacroDefinition() { Name = "Save Desktop", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "KillGumpOpen", Type = MacroType.None}); - what does this do?

            // These haven't been implemented in the client yet:
            // actionTypes.Add(new MacroDefinition() { Name = "PrimaryAbility", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "SecondaryAbility", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "EquipLastWeapon", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "SetUpdateRange", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "ModifyUpdateRange", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "IncreaseUpdateRange", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "DecreaseUpdateRange", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "MaxUpdateRange", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "MinUpdateRange", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "DefaultUpdateRange", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "UpdateRangeInfo", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "EnableRangeColor", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "DisableRangeColor", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "ToggleRangeColor", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "InvokeVirtue", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "SelectNext", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "SelectPrevious", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "SelectNearest", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "AttackSelectedTarget", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "UseSelectedTarget", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "CurrentTarget", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "Target System On/Off", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "ToggleBuffIconWindow", Type = MacroType.None});
            // actionTypes.Add(new MacroDefinition() { Name = "BandageSelfBandageTarget", Type = MacroType.None});

            // ==============================================================================================================
            // Move Macros
            // ==============================================================================================================
            Moves.Add(new MacroDefinition() { Name = "NW (top)", Type = MacroType.Move, Index = (int)Direction.Up });
            Moves.Add(new MacroDefinition() { Name = "N (top-right)", Type = MacroType.Move, Index = (int)Direction.North });
            Moves.Add(new MacroDefinition() { Name = "NE (right)", Type = MacroType.Move, Index = (int)Direction.Right });
            Moves.Add(new MacroDefinition() { Name = "E (bottom-right)", Type = MacroType.Move, Index = (int)Direction.East });
            Moves.Add(new MacroDefinition() { Name = "SE (bottom)", Type = MacroType.Move, Index = (int)Direction.Down });
            Moves.Add(new MacroDefinition() { Name = "S (bottom-left)", Type = MacroType.Move, Index = (int)Direction.South });
            Moves.Add(new MacroDefinition() { Name = "SW (left)", Type = MacroType.Move, Index = (int)Direction.Left });
            Moves.Add(new MacroDefinition() { Name = "W (top-left)", Type = MacroType.Move, Index = (int)Direction.West });

            // ==============================================================================================================
            // Display Macros
            // ==============================================================================================================
            Displays.Add(new MacroDefinition() { Name = "Configuration", Type = MacroType.OpenGump, Index = (int)MacroDisplay.Configuration });//0
            Displays.Add(new MacroDefinition() { Name = "Paperdoll", Type = MacroType.OpenGump, Index = (int)MacroDisplay.Paperdoll });//1
            Displays.Add(new MacroDefinition() { Name = "Status", Type = MacroType.OpenGump, Index = (int)MacroDisplay.Status });//2
            Displays.Add(new MacroDefinition() { Name = "Journal", Type = MacroType.OpenGump, Index = (int)MacroDisplay.Journal });//3
            Displays.Add(new MacroDefinition() { Name = "Skills", Type = MacroType.OpenGump, Index = (int)MacroDisplay.Skills });//4
            Displays.Add(new MacroDefinition() { Name = "MageSpellbook", Type = MacroType.OpenGump, Index = (int)MacroDisplay.MageSpellbook });
            Displays.Add(new MacroDefinition() { Name = "Chat", Type = MacroType.OpenGump, Index = (int)MacroDisplay.Chat });//6
            Displays.Add(new MacroDefinition() { Name = "Backpack", Type = MacroType.OpenGump, Index = (int)MacroDisplay.Backpack });//7
            Displays.Add(new MacroDefinition() { Name = "Overview", Type = MacroType.OpenGump, Index = (int)MacroDisplay.Overview });//8
            Displays.Add(new MacroDefinition() { Name = "Mail", Type = MacroType.OpenGump, Index = (int)MacroDisplay.Mail });
            Displays.Add(new MacroDefinition() { Name = "PartyManifest", Type = MacroType.OpenGump, Index = (int)MacroDisplay.PartyManifest });
            Displays.Add(new MacroDefinition() { Name = "PartyChat", Type = MacroType.OpenGump, Index = (int)MacroDisplay.PartyChat });
            Displays.Add(new MacroDefinition() { Name = "NecroSpellbook", Type = MacroType.OpenGump, Index = (int)MacroDisplay.NecroSpellbook });
            Displays.Add(new MacroDefinition() { Name = "PaladinSpellbook", Type = MacroType.OpenGump, Index = (int)MacroDisplay.PaladinSpellbook });//13
            Displays.Add(new MacroDefinition() { Name = "CombatBook", Type = MacroType.OpenGump, Index = (int)MacroDisplay.CombatBook });
            Displays.Add(new MacroDefinition() { Name = "BushidoSpellbook", Type = MacroType.OpenGump, Index = (int)MacroDisplay.BushidoSpellbook });
            Displays.Add(new MacroDefinition() { Name = "NinjitsuSpellbook", Type = MacroType.OpenGump, Index = (int)MacroDisplay.NinjitsuSpellbook });
            Displays.Add(new MacroDefinition() { Name = "Guild", Type = MacroType.OpenGump, Index = (int)MacroDisplay.Guild });
            Displays.Add(new MacroDefinition() { Name = "SpellWeavingSpellbook", Type = MacroType.OpenGump, Index = (int)MacroDisplay.SpellWeavingSpellbook });//18
            Displays.Add(new MacroDefinition() { Name = "QuestLog", Type = MacroType.OpenGump, Index = (int)MacroDisplay.QuestLog });//19

            // ==============================================================================================================
            // Use Skill Macros
            // ==============================================================================================================
            foreach (KeyValuePair<int, SkillEntry> pair in PlayerState.Skills.List)
            {
                SkillEntry skill = pair.Value;
                if (skill.HasUseButton)
                {
                    useSkills.Add(new MacroDefinition(skill.Name, MacroType.UseSkill, skill.Index));
                }
            }

            // ==============================================================================================================
            // Cast Spell Macros
            // ==============================================================================================================
            foreach (SpellDefinition spell in SpellsMagery.Spells)
            {
                castSpell.Add(new MacroDefinition(spell.Name, MacroType.CastSpell, spell.ID);
            }

            // ==============================================================================================================
            // Arm/Disarm Macros
            // ==============================================================================================================
            armDisarm.Add(new MacroDefinition("Main Hand", MacroType.ArmDisarm, (int)MacroArmDisarm.MainHand));
            armDisarm.Add(new MacroDefinition("Off Hand", MacroType.ArmDisarm, (int)MacroArmDisarm.OffHand));
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
                MacroDefinition action = actionTypes[CurrentActionID];
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
                    MacroDefinition valueObj = Displays[CurrentValueID];
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