/***************************************************************************
 *   DebugOutputEventListener.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Diagnostics;

namespace UltimaXNA.Core.Diagnostics.Listeners
{
    public class DebugOutputEventListener : AEventListener
    {
        private const string Format = "{0} {1:yyyy-MM-dd HH\\:mm\\:ss\\:ffff} {2}";

        public override void OnEventWritten(EventLevels level, string message)
        {
            string output = string.Format(Format, level, DateTime.Now, message);
            Debug.WriteLine(output);
        }
    }
}