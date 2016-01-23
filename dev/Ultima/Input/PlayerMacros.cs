using System;
using System.Collections.Generic;
using System.IO;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Input;
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

        private static List<MacroAction> EMPTY = new List<MacroAction>();
        private List<MacroAction> m_Macros = null;

        public List<MacroAction> All
        {
            get
            {
                if (m_Macros == null)
                    return EMPTY;
                return m_Macros;
            }
        }

        public MacroAction AddNewMacroAction()
        {
            MacroAction ma = new MacroAction();
            if (m_Macros == null)
                m_Macros = new List<MacroAction>();
            m_Macros.Add(ma);
            return ma;
        }

        public void ReceiveKeyboardInput(List<InputEventKeyboard> events)
        {
            foreach (InputEventKeyboard e in events)
            {
                foreach (MacroAction action in All)
                {
                    if (action.Keystroke == e.KeyCode &&
                        action.Alt == e.Alt &&
                        action.Ctrl == e.Control &&
                        action.Shift == e.Shift)
                    {
                        Macros.RunMacroAction(action);
                        e.Handled = true;
                    }
                }
            }
        }

        // ==============================================================================================================
        // Load / save
        // ==============================================================================================================

        private const uint MAGIC = 0xAB4603E0;
        private const string c_PathAppend = "_macros2d.txt";
        private string s_Path = null;

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
                File.Copy(s_Path, s_Path + ".bak");
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

        public void Load(string username)
        {
            s_Path = string.Format("{0}{1}", username, c_PathAppend);

            if (!File.Exists(s_Path))
            {
                Tracer.Debug("No macros to load.");
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
                    writer.Write((int)All[i].Macros[j].Index);
                    writer.Write((bool)All[i].Macros[j].IsInteger);
                    if (All[i].Macros[j].IsInteger)
                    {
                        writer.Write((int)All[i].Macros[j].ValueInteger);
                    }
                    else
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
                m_Macros = new List<MacroAction>();
            m_Macros.Clear();

            int version = reader.ReadInt();
            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                MacroAction action = new MacroAction();
                action.Keystroke = (WinKeys)reader.ReadUShort();
                action.Ctrl = reader.ReadBool();
                action.Alt = reader.ReadBool();
                action.Shift = reader.ReadBool();
                reader.ReadBool(); // unused filler byte

                int macroCount = reader.ReadUShort();
                for (int j = 0; j < macroCount; j++)
                {
                    int type = reader.ReadInt();
                    int index = reader.ReadInt();
                    MacroDefinition def = Macros.GetDefinition((MacroType)type, index);
                    bool isInteger = reader.ReadBool();
                    if (isInteger)
                        action.Macros.Add(new Macro(def, reader.ReadInt()));
                    else
                        action.Macros.Add(new Macro(def, reader.ReadString()));
                }

                m_Macros.Add(action);

            }

            return true;
        }
    }
}
