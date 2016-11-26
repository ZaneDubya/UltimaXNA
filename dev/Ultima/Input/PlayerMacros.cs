using System;
using System.Collections.Generic;
using System.IO;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.IO;
using UltimaXNA.Core.Windows;

namespace UltimaXNA.Ultima.Input
{
    public class PlayerMacros
    {
        public int Count
        {
            get
            {
                return All.Count;
            }
        }

        private static List<Action> EMPTY = new List<Action>();
        private List<Action> m_Macros;

        public List<Action> All
        {
            get
            {
                if (m_Macros == null)
                    return EMPTY;
                return m_Macros;
            }
        }

        public void AddNewMacroAction(Action action)
        {
            m_Macros.Add(action);
        }

        public void AddNewMacroAction(Action action, int index)
        {
            if (index < 0 || index >= m_Macros.Count)
            {
                AddNewMacroAction(action);
            }
            else
            {
                m_Macros.Insert(index, action);
            }
        }

        public void RemoveMacroAction(Action action)
        {
            m_Macros.Remove(action);
        }

        // ============================================================================================================
        // Load / save
        // ============================================================================================================

        private const uint MAGIC = 0xF14934E0;
        private const string c_PathAppend = "_macros2d.txt";
        private string s_Path;

        public void Save()
        {
            bool fileExists = false;

            if (s_Path == null)
                return;

            if (All.Count == 0)
            {
                if (File.Exists(s_Path))
                {
                    try
                    {
                        File.Delete(s_Path);
                    }
                    catch
                    {
                        // ... could not delete?
                    }
                }
                return;
            }

            if (File.Exists(s_Path))
            {
                fileExists = true;
                File.Copy(s_Path, s_Path + ".bak", true);
            }

            try
            {
                Tracer.Debug("Saving macros...");
                BinaryFileWriter writer = new BinaryFileWriter(s_Path, false);
                Serialize(writer);
                writer.Close();
                writer = null;
            }
            catch (Exception e)
            {
                Tracer.Warn(string.Format("Error saving macros: {0}", e.Message));
                Tracer.Debug("Attempting to restore old macros...");
                if (fileExists)
                {
                    File.Copy(s_Path + ".bak", s_Path);
                }
                Tracer.Debug("Old macros restored.");
            }
        }

        public void Load()
        {
            if (s_Path == null || s_Path == string.Empty)
                return;

            if (!File.Exists(s_Path))
            {
                Tracer.Debug("No macros to load. Creating deafult macro set");
                CreateDefaultMacroSet();
                return;
            }

            try
            {
                BinaryFileReader reader = new BinaryFileReader(new BinaryReader(new FileStream(s_Path, FileMode.Open)));
                if (Unserialize(reader))
                {
                    Tracer.Debug("Macros loaded!");
                }
                else
                {
                    Tracer.Debug("Error reading macro file.");
                }
                reader.Close();
                reader = null;
            }
            catch (Exception e)
            {
                Tracer.Warn(string.Format("Error loading macros: {0}", e.Message));
            }
        }

        public void Load(string username)
        {
            s_Path = string.Format("{0}{1}", username, c_PathAppend);

            Load();
        }

        private void Serialize(BinaryFileWriter writer)
        {
            writer.Write(MAGIC);
            writer.Write((int)0); // version
            writer.Write((int)All.Count);

            for (int i = 0; i < All.Count; i++)
            {
                writer.Write((ushort)All[i].Keystroke);
                writer.Write(All[i].Ctrl);
                writer.Write(All[i].Alt);
                writer.Write(All[i].Shift);
                writer.Write(false);

                writer.Write((ushort)All[i].Macros.Count);
                for (int j = 0; j < All[i].Macros.Count; j++)
                {
                    writer.Write((int)All[i].Macros[j].Type);
                    writer.Write((byte)All[i].Macros[j].ValueType);
                    if (All[i].Macros[j].ValueType == Macro.ValueTypes.Integer)
                    {
                        writer.Write((int)All[i].Macros[j].ValueInteger);
                    }
                    else if (All[i].Macros[j].ValueType == Macro.ValueTypes.String)
                    {
                        writer.Write((string)All[i].Macros[j].ValueString);
                    }
                }
            }
        }

        private bool Unserialize(BinaryFileReader reader)
        {
            uint magic = reader.ReadUInt();
            if (magic != MAGIC)
                return false;

            if (m_Macros == null)
                m_Macros = new List<Action>();
            m_Macros.Clear();

            int version = reader.ReadInt();
            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Action action = new Action();
                action.Keystroke = (WinKeys)reader.ReadUShort();
                action.Ctrl = reader.ReadBool();
                action.Alt = reader.ReadBool();
                action.Shift = reader.ReadBool();
                reader.ReadBool(); // unused filler byte

                int macroCount = reader.ReadUShort();
                for (int j = 0; j < macroCount; j++)
                {
                    int type = reader.ReadInt();
                    Macro.ValueTypes valueType = (Macro.ValueTypes)reader.ReadByte();
                    if (valueType == Macro.ValueTypes.Integer)
                        action.Macros.Add(new Macro((MacroType)type, reader.ReadInt()));
                    else if (valueType == Macro.ValueTypes.String)
                        action.Macros.Add(new Macro((MacroType)type, reader.ReadString()));
                    else
                        action.Macros.Add(new Macro((MacroType)type));
                }

                m_Macros.Add(action);

            }

            return true;
        }

        // ============================================================================================================
        // Default macro set
        // ============================================================================================================

        public void CreateDefaultMacroSet()
        {
            if (m_Macros == null)
                m_Macros = new List<Action>();
            m_Macros.Clear();

            Action action;
            
            action = new Action();
            action.Keystroke = WinKeys.S;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Status));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.T;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Chat));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.B;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.MageSpellbook));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.C;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.ToggleWarPeace));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.P;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Paperdoll));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.K;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Skills));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.J;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Journal));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.Q;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.QuestLog));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.W;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.SpellWeavingSpellbook));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.I;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Backpack));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.R;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Overview));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.O;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Configuration));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.X;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.TargetSelf));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.S;
            action.Shift = true;
            action.Macros.Add(new Macro(MacroType.LastTarget));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad8;
            action.Macros.Add(new Macro(MacroType.Say, "Forward"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad5;
            action.Macros.Add(new Macro(MacroType.Say, "Stop"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad4;
            action.Macros.Add(new Macro(MacroType.Say, "Turn Left"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad6;
            action.Macros.Add(new Macro(MacroType.Say, "Turn Right"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad7;
            action.Macros.Add(new Macro(MacroType.Say, "Left"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad9;
            action.Macros.Add(new Macro(MacroType.Say, "Right"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad2;
            action.Macros.Add(new Macro(MacroType.Say, "Backwards"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad1;
            action.Macros.Add(new Macro(MacroType.Say, "Raise Anchor"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad3;
            action.Macros.Add(new Macro(MacroType.Say, "Drop Anchor"));
            AddNewMacroAction(action);

            // NİGHTSİGHT
            action = new Action();
            action.Keystroke = WinKeys.F1;
            action.Macros.Add(new Macro(MacroType.CastSpell, 5)); 
            action.Macros.Add(new Macro(MacroType.LastTarget));
            AddNewMacroAction(action);
            
            // TEST
            action = new Action();
            action.Keystroke = WinKeys.F2;
            action.Macros.Add(new Macro(MacroType.Say, "Delaying 1 second."));
            action.Macros.Add(new Macro(MacroType.Delay, "10"));
            action.Macros.Add(new Macro(MacroType.Say, "Delay 2 seconds."));
            action.Macros.Add(new Macro(MacroType.Delay, "20"));
            action.Macros.Add(new Macro(MacroType.Say, "Delay complete!"));
            AddNewMacroAction(action);
            
            Save();
        }
    }
}
