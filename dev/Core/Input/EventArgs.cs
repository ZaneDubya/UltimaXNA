/***************************************************************************
 *   EVentArgs.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
namespace UltimaXNA.Input
{
    public class EventArgs
    {
        private readonly WinKeys _modifiers;

        public virtual bool Alt
        {
            get { return ((_modifiers & WinKeys.Alt) == WinKeys.Alt); }
        }

        public bool Control
        {
            get { return ((_modifiers & WinKeys.Control) == WinKeys.Control); }
        }

        public virtual bool Shift
        {
            get { return ((_modifiers & WinKeys.Shift) == WinKeys.Shift); }
        }

        public EventArgs(WinKeys modifiers)
        {
            _modifiers = modifiers;
        }
    }
}
