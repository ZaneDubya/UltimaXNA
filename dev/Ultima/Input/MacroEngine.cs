using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Input;

namespace UltimaXNA.Ultima.Input
{
    class MacroEngine
    {
        List<RunningMacroAction> m_RunningMacros = new List<RunningMacroAction>();

        public void Run(Action action)
        {
            if (m_RunningMacros.FindIndex(p => p.Action == action) == -1) //ignoring state which prevents multiple calls
                m_RunningMacros.Add(new RunningMacroAction(action));
        }

        public void Update(double frameMS)
        {
            m_RunningMacros.RemoveAll(p => p.IsFinished);
            m_RunningMacros.ForEach(item =>
            {
                item.Update(frameMS);
            });
        }

        class RunningMacroAction
        {
            public Action Action
            {
                get;
                private set;
            }

            public bool IsFinished
            {
                get;
                private set;
            }

            public DateTime? m_MacroDelayer;
            public int m_MacroIndex = 0;

            public RunningMacroAction(Action action)
            {
                Action = action;
                IsFinished = false;
                m_MacroIndex = 0;
            }

            public void Update(double frameMS)
            {
                if (Action.Macros.Count == 0 ||
                    m_MacroIndex == Action.Macros.Count ||
                    Action.Macros[0].Type == MacroType.None)
                {
                    IsFinished = true;
                    return;
                }
                RunMacro(this);
            }
        }

        static void RunMacro(RunningMacroAction action)
        {
            WorldModel world = Services.Get<WorldModel>();

            switch (action.Action.Macros[action.m_MacroIndex].Type)
            {
                case MacroType.Say:
                    MacroSay(action.Action.Macros[action.m_MacroIndex].ValueString);
                    break;
                case MacroType.Delay:
                    if (action.m_MacroDelayer == null) //delay starts
                    {
                        action.m_MacroDelayer = DateTime.Now;
                        return;
                    }

                    int delayMs = 0;
                    bool result = int.TryParse(action.Action.Macros[action.m_MacroIndex].ValueString, out delayMs);
                    if (result)
                    {
                        TimeSpan ts = DateTime.Now - action.m_MacroDelayer.Value;
                        if ((int)ts.TotalMilliseconds >= delayMs) //delay ends
                        {
                            action.m_MacroDelayer = null;
                        }
                        else
                        {
                            return; //prevent iterates
                        }
                    }
                    break;
                case MacroType.UseSkill:
                    world.Interaction.UseSkill(action.Action.Macros[action.m_MacroIndex].ValueInteger);
                    break;
                case MacroType.CastSpell:
                    world.Interaction.CastSpell(action.Action.Macros[action.m_MacroIndex].ValueInteger);
                    break;
                case MacroType.LastTarget:
                    var source = WorldModel.Entities.GetObject<AEntity>(world.Interaction.LastTarget, false);
                    if (source != null)
                    {
                        world.Input.MousePick.PickOnly = PickType.PickStatics | PickType.PickObjects;
                        world.Cursor.mouseTargetingEventObject(source);
                        //need change the cursor
                    }
                    break;
            }
            action.m_MacroIndex++;
        }

        static void MacroSay(string text)
        {
            INetworkClient network = Services.Get<INetworkClient>();
            MessageTypes speechType = MessageTypes.Normal;
            ushort hue = (ushort)Settings.UserInterface.SpeechColor;
            network.Send(new AsciiSpeechPacket(speechType, 0, hue , "ENU", text));
        }

        static void RunMacroAction(Action action)//old method
        {
            /*WorldModel world = ServiceRegistry.GetService<WorldModel>();
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
            }*/
        }
    }
}
