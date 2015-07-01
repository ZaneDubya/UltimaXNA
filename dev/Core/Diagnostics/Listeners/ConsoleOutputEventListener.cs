/***************************************************************************
 *   ConsoleOutputEventListener.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;

namespace UltimaXNA.Core.Diagnostics.Listeners
{
    public class ConsoleOutputEventListener : AEventListener
    {
        public ConsoleOutputEventListener()
        {

        }

        public override void OnEventWritten(EventLevels level, string message)
        {
            ConsoleColor color = ConsoleColor.Gray;

            switch (level)
            {
                case EventLevels.Info:
                    color = ConsoleColor.White;
                    break;
                case EventLevels.Warning:
                    color = ConsoleColor.Yellow;
                    break;
                case EventLevels.Error:
                case EventLevels.Critical:
                    color = ConsoleColor.Red;
                    break;
            }

            ConsoleManager.PushColor(color);
            Console.WriteLine(message);
            ConsoleManager.PopColor();
        }
    }
}