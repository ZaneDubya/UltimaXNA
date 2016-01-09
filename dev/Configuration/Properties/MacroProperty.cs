using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Windows;

namespace UltimaXNA.Configuration.Properties
{
    public enum MacroType { Skill, Spell, Display, Text, Move, ArmDisarm, None }

    public interface IMacro//maybe i dont need interface
    {
        int PacketInfo { get; set; } //identification of action for example: send packet
        string Name { get; set; }
        MacroType Type { get; set; }
        bool isActionType { get; set; }
    }

    public struct Macro : IMacro
    {
        public int PacketInfo { get; set; }
        public string Name { get; set; }
        public MacroType Type { get; set; }
        public bool isActionType { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class XKey
    {
        public int Count { get { return xnaMacros.Count; } }

        public XKey(string userName)
        {
            path = Path.Combine(Application.StartupPath, string.Format("{0}_macros2d.txt", userName.Replace(" ", "-")));
            loadKeysFromFile();
        }

        private string path = "";

        private List<XMacro> xnaMacros = new List<XMacro>();

        public XMacro isEqual(WinKeys h_Key, bool h_Shift, bool h_Alt, bool h_Ctrl)
        {
            return this.xnaMacros.Find(p => p.Keystroke == h_Key && p.Shift == h_Shift && p.Alt == h_Alt && p.Ctrl == h_Ctrl);
        }

        public int addKey(XMacro newKey)
        {
            int inx = xnaMacros.FindIndex(p => p.ToString().StartsWith(newKey.ToString()));
            if (inx != -1)
            {
                xnaMacros.RemoveAt(inx);
            }
            xnaMacros.Add(newKey);
            return xnaMacros.Count - 1;
        }

        public void saveMacros()
        {
            try
            {
                if (xnaMacros.Count == 0)
                {
                    File.Delete(path);
                    return;
                }
                this.xnaMacros = this.xnaMacros.OrderBy(p => p.ToString()).ToList();
                StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
                sw.Write(getStringList());
                sw.Close();
            }
            catch (Exception e)
            {
                //error FİLE CANT ACCESSIBLE
                //TRACE.WARN
                Tracer.Warn(string.Format("ERROR in Class this: {0} [ErrorMessage: {1}]]", this.ToString(), e.Message));
            }
        }

        public void loadKeysFromFile()
        {
            if (!File.Exists(path))
            {
                setDefaultMacros();
                return;
            }

            xnaMacros.Clear();
            // i will be change this split system
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            try
            {
                string[] list = sr.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                for (int i = 0; i < list.Length - 1; i++)
                {
                    string[] currValue = list[i].Split('\0');
                    string[] keyInfo = currValue[0].Split('\a');
                    XMacro xmcr = new XMacro();

                    #region baddd

                    bool isNumber = false;
                    if (keyInfo[0].Length == 1)
                    {
                        int IntValeTest = 0;
                        isNumber = int.TryParse(keyInfo[0], out IntValeTest);
                    }
                    if (isNumber)
                    {
                        xmcr.Keystroke = (WinKeys)int.Parse("D" + keyInfo[0]);
                    }
                    else
                    {
                        xmcr.Keystroke = (WinKeys)int.Parse(keyInfo[0]);
                    }

                    #endregion baddd

                    xmcr.Shift = keyInfo[1] == "1" ? true : false;
                    xmcr.Alt = keyInfo[2] == "1" ? true : false;
                    xmcr.Ctrl = keyInfo[3] == "1" ? true : false;
                    for (int i2 = 1; i2 < currValue.Length; i2++)
                    {
                        string[] _action = currValue[i2].Split('\a');
                        XAction action = new XAction();
                        action.actionID = int.Parse(_action[0]);
                        action.valueID = int.Parse(_action[1]);
                        action.valueText = _action[2];
                        xmcr.actionList.Add(action);
                    }
                    addKey(xmcr);
                }
            }
            catch (Exception e)
            {
                //File format wrong maybe opened with notepad ????
                setDefaultMacros();
                Tracer.Warn(string.Format("ERROR in Class this: {0} [ErrorMessage: {1}]]", this.ToString(), e.Message));
            }
        }

        public void setDefaultMacros()//i like it :))
        {
            xnaMacros.Clear();
            //
            XMacro xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.S;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 8, valueID = 2 });//8:open  2:status
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.T;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 8, valueID = 6 });//8:open    6:chat
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.B;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 8, valueID = 5 });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.C;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 6 });//war/peace:6 (in list index)
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.P;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 8, valueID = 1 });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.K;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 8, valueID = 4 });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.J;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 8, valueID = 3 });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.Q;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 8, valueID = 19 });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.W;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 8, valueID = 18 });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.I;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 8, valueID = 7 });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.R;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 8, valueID = 8 });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.O;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 8, valueID = 0 });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.X;
            xmcr.Alt = true;
            xmcr.actionList.Add(new XAction() { actionID = 20 });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.S;
            xmcr.Shift = true;
            xmcr.actionList.Add(new XAction() { actionID = 19 });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.NumPad8;
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Forward" });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.NumPad5;
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Stop" });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.NumPad4;
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Turn Left" });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.NumPad6;
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Turn Right" });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.NumPad7;
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Left" });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.NumPad9;
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Right" });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.NumPad2;
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Backwards" });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.NumPad1;
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Raise Anchor" });
            addKey(xmcr);
            //
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.NumPad3;
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Drop Anchor" });
            addKey(xmcr);
            //NİGHTSİGHT
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.F1;
            xmcr.actionList.Add(new XAction() { actionID = 15, valueID = 5 });//ALL NUMBER USING INDEX NUMBER
            xmcr.actionList.Add(new XAction() { actionID = 23 });//last target
            addKey(xmcr);
            //TEST
            xmcr = new XMacro();
            xmcr.Keystroke = WinKeys.F2;
            xmcr.actionList.Add(new XAction() { actionID = 28, valueText = "1000" });
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Delay 1 second" });
            xmcr.actionList.Add(new XAction() { actionID = 28, valueText = "2000" });
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Delay 2 second" });
            xmcr.actionList.Add(new XAction() { actionID = 28, valueText = "3000" });
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Delay 3 second" });
            xmcr.actionList.Add(new XAction() { actionID = 28, valueText = "4000" });
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Delay 4 second" });
            xmcr.actionList.Add(new XAction() { actionID = 28, valueText = "5000" });
            xmcr.actionList.Add(new XAction() { actionID = 1, valueText = "Delay 5 second" });
            addKey(xmcr);
            ////////
            saveMacros();
        }

        private string getStringList()
        {
            // i will be change this split system
            string value = "";
            foreach (XMacro item in xnaMacros)
            {
                value += item.ToString();
                foreach (XAction item2 in item.actionList)
                    value += item2.ToString();

                value += Environment.NewLine;
            }
            return value;
        }

        public List<XMacro> getMacros()
        {
            return xnaMacros;
        }

        public XMacro getMacro(int index)
        {
            return xnaMacros[index];
        }

        public void removeMacro(int index)
        {
            xnaMacros.RemoveAt(index);
        }

        public void removeMacro(XMacro item)
        {
            xnaMacros.Remove(item);
        }
    }

    public class XMacro : IDisposable //I NEED DISPOSIBLE CLASS
    {
        public WinKeys Keystroke = WinKeys.None;
        public bool Shift = false;
        public bool Alt = false;
        public bool Ctrl = false;
        public List<XAction> actionList = new List<XAction>();

        public override string ToString()
        {
            return string.Format("{0}\a{1}\a{2}\a{3}", (int)Keystroke, Convert.ToInt32(Shift), Convert.ToInt32(Alt), Convert.ToInt32(Ctrl));
        }

        // Flag: Has Dispose already been called?
        private bool disposed = false;

        // Instantiate a SafeHandle instance.
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
    }

    public class XAction //I NEED DISPOSIBLE CLASS
    {
        //referance to= Settings.Macro.actionTypes //vbbb...
        public int actionID = -1;

        public int valueID = -1;
        public string valueText = "N";

        public override string ToString()
        {
            return string.Format("\0{0}\a{1}\a{2}", actionID, valueID, valueText);
        }
    }

    #region NEED I ??

    public enum Spells : int
    {
        Clumsy = 1,

        [EnumMember(Value = "Create Food")]
        Create_Food = 2,

        Feeblemind = 3,
        Heal = 4,

        [EnumMember(Value = "Magic Arrow")]
        Magic_Arrow = 5,

        [EnumMember(Value = "Night Sight")]
        Night_Sight = 6,

        [EnumMember(Value = "Reactive Armor")]
        Reactive_Armor = 7,

        WeakenAgility = 8,
        Cunning = 9,
        Cure = 10,
        Harm = 11,

        [EnumMember(Value = "Magic Trap")]
        Magic_Trap = 12,

        [EnumMember(Value = "Magic Untrap")]
        Magic_Untrap = 13,

        Protection = 14,
        Strength = 15,
        Bless = 16,
        Firball = 17,

        [EnumMember(Value = "Magic Lock")]
        Magic_Lock = 18,

        Poison = 19,
        Telekinesis = 20,
        Teleport = 21,

        [EnumMember(Value = "Magic Unlock")]
        Magic_Unlock = 22,

        [EnumMember(Value = "Wall of Stone")]
        Wall_of_Stone = 23,

        Archcure = 24,

        ArchprotectionCurse = 25,

        [EnumMember(Value = "Fire Field")]
        Fire_Field = 26,

        [EnumMember(Value = "Greater Heal")]
        Greater_Heal = 27,

        Lightning = 28,

        [EnumMember(Value = "Mana Drain")]
        Mana_Drain = 29,

        Recall = 30,

        [EnumMember(Value = "Blade Spirit")]
        Blade_Spirit = 31,

        [EnumMember(Value = "Dispel Field")]
        Dispel_Field = 32,

        Incognito = 33,

        [EnumMember(Value = "Magic Reflection")]
        Magic_Reflection = 34,

        [EnumMember(Value = "Mind Blast")]
        Mind_Blast = 35,

        Paralyze = 36,

        [EnumMember(Value = "Poison Field")]
        Poison_Field = 37,

        [EnumMember(Value = "Summon Creature")]
        Summon_Creature = 38,

        Dispel = 39,

        [EnumMember(Value = "Energy Bolt")]
        Energy_Bolt = 40,

        Explosion = 41,
        Invisibility = 42,
        Mark = 43,

        [EnumMember(Value = "Mass Curse")]
        Mass_Curse = 44,

        [EnumMember(Value = "Paralyze Field")]
        Paralyze_Field = 45,

        Reveal = 46,

        [EnumMember(Value = "Chain Lightning")]
        Chain_Lightning = 47,

        [EnumMember(Value = "Energy Field")]
        Energy_Field = 48,

        Flamestrike = 49,

        [EnumMember(Value = "Gate Travel")]
        Gate_Travel = 50,

        [EnumMember(Value = "Mana Vampire")]
        Mana_Vampire = 51,

        [EnumMember(Value = "Mass Dispell")]
        Mass_Dispell = 52,

        [EnumMember(Value = "Meteor Swarm")]
        Meteor_Swarm = 53,

        Polymorph = 54,
        Earthquake = 55,

        [EnumMember(Value = "Energy Vortex")]
        Energy_Vortex = 56,

        Resurrection = 57,

        [EnumMember(Value = "Summon Air")]
        Summon_Air = 58,

        [EnumMember(Value = "Summon Demon")]
        Summon_Demon = 59,

        [EnumMember(Value = "Summon Earth")]
        Summon_Earth = 60,

        [EnumMember(Value = "Summon Fire")]
        Summon_Fire = 61,

        [EnumMember(Value = "Summon Water")]
        Summon_Water = 62,
    }

    #endregion NEED I ??
}