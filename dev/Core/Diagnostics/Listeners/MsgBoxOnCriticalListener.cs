/***************************************************************************
 *   MsgBoxOnCriticalListener.cs
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
    class MsgBoxOnCriticalListener : AEventListener
    {
        public override void OnEventWritten(EventLevels level, string message)
        {
            if (level == EventLevels.Critical)
                throw new Exception(message);
        }
    }
}
