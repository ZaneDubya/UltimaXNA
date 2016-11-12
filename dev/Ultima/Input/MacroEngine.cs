using System.Collections.Generic;

namespace UltimaXNA.Ultima.Input
{
    class MacroEngine
    {
        private List<RunningMacroAction> m_RunningMacros = new List<RunningMacroAction>();

        public void Run(Action action)
        {
            for (int i = 0; i < m_RunningMacros.Count; i++)
            {
                if (m_RunningMacros[i].Action == action)
                {
                    m_RunningMacros.RemoveAt(i);
                    i--;
                }
            }
            m_RunningMacros.Add(new RunningMacroAction(action));
        }

        public void Update(double frameMS)
        {
            for (int i = 0; i < m_RunningMacros.Count; i++)
            {
                if (m_RunningMacros[i].IsFinished)
                {
                    m_RunningMacros.RemoveAt(i);
                    i--;
                }
                else
                {
                    m_RunningMacros[i].Update(frameMS);
                }
            }
        }

        private class RunningMacroAction
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

            private double m_LastMacroTimestamp = 0;
            private int m_MacroIndex = 0;

            public RunningMacroAction(Action action)
            {
                Action = action;
                IsFinished = false;

                m_MacroIndex = 0;
                m_LastMacroTimestamp = 0;
            }

            public void Update(double frameMS)
            {
                IsFinished = true;
            }
        }

        private static void RunMacroAction(Action action)
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
