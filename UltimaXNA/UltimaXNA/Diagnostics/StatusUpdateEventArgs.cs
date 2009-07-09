using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace UltimaXNA.Diagnostics
{
    /// <summary>
    /// Event argument used when notifying status change from a IStatusNotifier object
    /// </summary>
    public class StatusUpdateEventArgs : EventArgs
    {
        readonly string _status;
        readonly int _statusLevel;

        /// <summary>
        /// Gets the status message
        /// </summary>
        public string Status
        {
            get { return _status; }
        }

        /// <summary>
        /// Gets the status level 
        /// <see cref="Partners.Core.StatusLevel"/>
        /// </summary>
        public int StatusLevel
        {
            get { return _statusLevel; }
        }

        /// <summary>
        /// Creates an instance of StatusUpdateEventArgs
        /// </summary>
        /// <param name="status">A composite format string</param>
        /// <param name="objects">An object array containing zero or more objects to format</param>
        public StatusUpdateEventArgs(string status, params object[] objects)
            : this(UltimaXNA.Diagnostics.StatusLevel.Debug, status, objects)
        {
        }

        /// <summary>
        /// Creates an instance of StatusUpdateEventArgs
        /// </summary>
        /// <param name="status">The status message</param>
        public StatusUpdateEventArgs(string status)
            : this(UltimaXNA.Diagnostics.StatusLevel.Debug, status)
        {
        }

        /// <summary>
        /// Creates an instance of StatusUpdateEventArgs
        /// </summary>
        /// <param name="statusLevel">Gets the status level <see cref="Partners.Core.StatusLevel"/></param>
        /// <param name="status">A composite format string</param>
        /// <param name="objects">An object array containing zero or more objects to format</param>
        public StatusUpdateEventArgs(int statusLevel, string status, params object[] objects)
        {
            this._statusLevel = statusLevel;
            this._status = String.Format(CultureInfo.CurrentCulture, status, objects);
        }

        /// <summary>
        /// Creates an instance of StatusUpdateEventArgs
        /// </summary>
        /// <param name="statusLevel">Gets the status level <see cref="Partners.Core.StatusLevel"/></param>
        /// <param name="status">The status message</param>
        public StatusUpdateEventArgs(int statusLevel, string status)
        {
            this._statusLevel = statusLevel;
            this._status = status;
        }
    }
}
