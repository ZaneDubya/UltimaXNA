/***************************************************************************
 *   AEventListener.cs
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
    public abstract class AEventListener
    {
        public abstract void OnEventWritten(EventLevels level, string message);

        public void OnEventWritten(EventLevels level, string message, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                OnEventWritten(level, message);
            }
            else
            {
                OnEventWritten(level, string.Format(message, args));
            }
        }

        public void OnEventWritten(EventLevels level, Exception ex)
        {
            OnEventWritten(level, ex.Message);
        }
    }
}
