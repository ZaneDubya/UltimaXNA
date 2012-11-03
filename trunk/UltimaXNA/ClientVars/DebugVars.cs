﻿/***************************************************************************
 *   DebugVars.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using UltimaXNA.Entity;

namespace UltimaXNA.ClientVars
{
    class DebugVars
    {
        public static bool Flag_ShowDataRead = false;
        public static bool Flag_BreakdownDataRead = false;
        public static bool Flag_DisplayFPS = true;
        public static bool Flag_LogKeyboardChars = false;

        // Debug message - I put a lot of crap in here to test values. Feel free to add or remove variables.
        public static string DebugMessage { get { return generateDebugMessage(); } }
        static string generateDebugMessage()
        {
            String debugMessage = string.Empty;

            debugMessage += string.Format("#Objects: {0}", IsometricRenderer.ObjectsRendered);

            if (Flag_DisplayFPS)
                debugMessage += string.Format("\nFPS: {0}", (int)EngineVars.FPS);

            if (Flag_ShowDataRead)
            {
                if (Flag_BreakdownDataRead)
                    debugMessage += ClientVars.Metrics.DataReadBreakdown;
                else
                    debugMessage += string.Format("\nData Read: {0}", ClientVars.Metrics.TotalDataRead.ToString());
            }

            BaseEntity e = Entities.GetPlayerObject();
            if (e != null)
                debugMessage += "\nMyPos:" + e.Position;


            if (IsometricRenderer.MouseOverObject != null)
                debugMessage += "\nOVER:" + IsometricRenderer.MouseOverObject.ToString();
            else
                debugMessage += "\nOVER: " + "null";

            if (IsometricRenderer.MouseOverGround != null)
                debugMessage += "\nGROUND: " + IsometricRenderer.MouseOverGround.Position.ToString();
            else
                debugMessage += "\nGROUND: null";

            return debugMessage;
        }
    }
}
