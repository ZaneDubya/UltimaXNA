/***************************************************************************
 *   StatusLevel.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace UltimaXNA.Diagnostics
{
    /// <summary>
    /// Status level used to filter out events from IStatusNotifier
    /// </summary>
    public enum StatusLevel : int
    {
        /// <summary>
        /// Indicates Debug level of status
        /// </summary>
        Debug = 0,
        /// <summary>
        /// Indicates Informational level of status
        /// </summary>
        Info = 1,
        /// <summary>
        /// Indicates Warning level of status
        /// </summary>
        Warn = 2,
        /// <summary>
        /// Indicates Error level of status
        /// </summary>
        Error = 3,
        /// <summary>
        /// Indicates Fatal level of status
        /// </summary>
        Fatal = 4
    }
}
