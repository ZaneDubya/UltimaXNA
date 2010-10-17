using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace UltimaXNA.InputOld
{
    public class MacroKey
    {
        DateTime _lastExecution = DateTime.MinValue;
        bool _shift = false;
        bool _alt = false;
        bool _ctrl = false;
        Keys _key = Keys.A;

        public Keys Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public bool Shift
        {
            get { return _shift; }
            set { _shift = value; }
        }

        public bool Alt
        {
            get { return _alt; }
            set { _alt = value; }
        }

        public bool Ctrl
        {
            get { return _ctrl; }
            set { _ctrl = value; }
        }

        public DateTime LastExecution
        {
            get { return _lastExecution; }
            set { _lastExecution = value; }
        }

        public override bool Equals(object obj)
        {
            MacroKey key = obj as MacroKey;

            if (key == null)
                return false;

            return key.Key == Key && key.Shift == Shift && key.Alt == Alt && key.Ctrl == Ctrl;
        }

        public static bool operator ==(MacroKey l, MacroKey r)
        {
            return l.Equals(r);
        }

        public static bool operator !=(MacroKey l, MacroKey r)
        {
            return !l.Equals(r);
        }

        public override int GetHashCode()
        {
            return (int)Key | ((Shift ? 1 : 0) << 9) | ((Alt ? 1 : 0) << 10) | ((Ctrl ? 1 : 0) << 11);
        }
    }

    public interface IMacroAction
    {
        string Name { get; }
        string Description { get; }

        void Execute();
    }

    public abstract class MacroAction : IMacroAction
    {
        // static ILoggingService Log = new UltimaXNA.Diagnostics.Logger("MacroAction");

        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual void Execute()
        {
            // Log.Debug("Action {0} was executed at {1}", Name, DateTime.Now);
        }
    }
}
