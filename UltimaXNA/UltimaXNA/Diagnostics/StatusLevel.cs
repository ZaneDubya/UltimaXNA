using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
