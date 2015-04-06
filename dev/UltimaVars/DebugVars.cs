/***************************************************************************
 *   DebugVars.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using UltimaXNA.UltimaWorld.Views;
using UltimaXNA.Core.Diagnostics;

namespace UltimaXNA.UltimaVars
{
    class DebugVars
    {
        public static bool DrawUIOutlines = false;

        public static bool Flag_ShowDataRead = false;
        public static bool Flag_BreakdownDataRead = false;
        public static bool Flag_DisplayFPS = true;
        public static bool Flag_LogKeyboardChars = false;

        // Debug message - I put a lot of crap in here to test values. Feel free to add or remove variables.
        public static string DebugMessage { get { return generateDebugMessage(); } }
        static string generateDebugMessage()
        {
            String debugMessage = string.Empty;

            if (Flag_ShowDataRead)
            {
                if (Flag_BreakdownDataRead)
                    debugMessage += Metrics.DataReadBreakdown;
                else
                    debugMessage += string.Format("\nData Read: {0}", Metrics.TotalDataRead.ToString());
            }

            return debugMessage;
        }
    }
}
