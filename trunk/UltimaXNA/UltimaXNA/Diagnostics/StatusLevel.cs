/***************************************************************************
 *   StatusLevel.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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
    public class StatusLevel
    {
        /// <summary>
        /// Indicates Debug level of status
        /// </summary>
        public const int Debug = 0;
        /// <summary>
        /// Indicates Informational level of status
        /// </summary>
        public const int Info = 1;
        /// <summary>
        /// Indicates Warning level of status
        /// </summary>
        public const int Warn = 2;
        /// <summary>
        /// Indicates Error level of status
        /// </summary>
        public const int Error = 3;
        /// <summary>
        /// Indicates Fatal level of status
        /// </summary>
        public const int Fatal = 4;
    }
}
