using System.Collections.Generic;
using UltimaXNA.Core.Windows;

namespace UltimaXNA.Ultima.Input
{
    /// <summary>
    /// A list of one or more macros that is executed on a given keystroke.
    /// </summary>
    public class Action
    {
        public WinKeys Keystroke = WinKeys.None;
        public bool Shift;
        public bool Alt;
        public bool Ctrl;

        public List<Macro> Macros = new List<Macro>();

        public Action()
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
