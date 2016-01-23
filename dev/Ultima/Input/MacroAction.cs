using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Windows;

namespace UltimaXNA.Ultima.Input
{
    /// <summary>
    /// A list of one or more macros that is executed on a given keystroke.
    /// </summary>
    public class MacroAction
    {
        public WinKeys Keystroke = WinKeys.None;
        public bool Shift = false;
        public bool Alt = false;
        public bool Ctrl = false;

        public List<Macro> Macros = new List<Macro>();

        public MacroAction()
        {

        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}{3}", 
                Shift ? "Shift+" : string.Empty,
                Alt ? "Alt+" : string.Empty,
                Ctrl ? "Ctrl+" : string.Empty,
                Keystroke.ToString());
        }
    }
}
