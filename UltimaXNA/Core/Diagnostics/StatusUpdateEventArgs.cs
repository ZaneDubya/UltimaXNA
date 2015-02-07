/***************************************************************************
 *   StatusUpdateEventArgs.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using System.Globalization;
#endregion

namespace UltimaXNA.Diagnostics
{
    /// <summary>
    /// Defines events used to notify progress 
    /// </summary>
    public interface IProgressNotifier
    {
        /// <summary>
        /// Called when the progress of the notifier has changed
        /// </summary>
        event EventHandler<ProgressUpdateEventArgs> ProgressUpdated;

        /// <summary>
        /// Called when the notifiers operation is complete
        /// </summary>
        event EventHandler<ProgressCompletedEventArgs> ProgressCompleted;
    }

    /// <summary>
    /// Event argument used when progress changes withing an IProgressNotifier object
    /// </summary>
    public class ProgressUpdateEventArgs : EventArgs
    {
        readonly int current;
        readonly int max;
        readonly int progressPercentage;

        /// <summary>
        /// Gets the percent completed 0-100
        /// </summary>
        public int ProgressPercentage
        {
            get { return progressPercentage; }
        }

        /// <summary>
        /// Gets the current value used in the progress percentage calculation
        /// </summary>
        public int Current
        {
            get { return current; }
        }

        /// <summary>
        /// Gets the max value used in the progress percentage calculation
        /// </summary>
        public int Max
        {
            get { return max; }
        }

        public ProgressUpdateEventArgs(int current, int max)
        {
            this.current = current;
            this.max = max;
            this.progressPercentage = (int)(100 * ((double)current / max));
        }
    }

    /// <summary>
    /// Event argument used when notifying status change from a IStatusNotifier object
    /// </summary>
    public class StatusUpdateEventArgs : EventArgs
    {
        readonly string status;
        readonly int statusLevel;

        /// <summary>
        /// Gets the status message
        /// </summary>
        public string Status
        {
            get { return status; }
        }

        /// <summary>
        /// Gets the status level 
        /// </summary>
        public int StatusLevel
        {
            get { return statusLevel; }
        }

        /// <summary>
        /// Creates an instance of StatusUpdateEventArgs
        /// </summary>
        /// <param name="status">A composite format string</param>
        /// <param name="objects">An object array containing zero or more objects to format</param>
        public StatusUpdateEventArgs(string status, params object[] objects)
            : this((int)Diagnostics.StatusLevel.Debug, status, objects)
        {
        }

        /// <summary>
        /// Creates an instance of StatusUpdateEventArgs
        /// </summary>
        /// <param name="status">The status message</param>
        public StatusUpdateEventArgs(string status)
            : this((int)Diagnostics.StatusLevel.Debug, status)
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
            this.statusLevel = statusLevel;
            this.status = String.Format(CultureInfo.CurrentCulture, status, objects);
        }

        /// <summary>
        /// Creates an instance of StatusUpdateEventArgs
        /// </summary>
        /// <param name="statusLevel">Gets the status level <see cref="Partners.Core.StatusLevel"/></param>
        /// <param name="status">The status message</param>
        public StatusUpdateEventArgs(int statusLevel, string status)
        {
            this.statusLevel = statusLevel;
            this.status = status;
        }
    }

    /// <summary>
    /// Event argument used when notifying process started from a IStatusNotifier object
    /// </summary>
    public class StartedEventArgs : EventArgs
    {

    }

    /// <summary>
    /// Event argument used when notifying process complete from a IStatusNotifier object
    /// </summary>
    public class ProgressCompletedEventArgs : EventArgs
    {

    }
}
