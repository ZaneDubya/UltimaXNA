/***************************************************************************
 *   StatusUpdateEventArgs.cs
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

namespace UltimaXNA.Core.Diagnostics
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
        readonly int m_Current;
        readonly int m_Max;
        readonly int m_ProgressPercentage;

        /// <summary>
        /// Gets the percent completed 0-100
        /// </summary>
        public int ProgressPercentage
        {
            get { return m_ProgressPercentage; }
        }

        /// <summary>
        /// Gets the current value used in the progress percentage calculation
        /// </summary>
        public int Current
        {
            get { return m_Current; }
        }

        /// <summary>
        /// Gets the max value used in the progress percentage calculation
        /// </summary>
        public int Max
        {
            get { return m_Max; }
        }

        public ProgressUpdateEventArgs(int current, int max)
        {
            m_Current = current;
            m_Max = max;
            m_ProgressPercentage = (int)(100 * ((double)current / max));
        }
    }

    /// <summary>
    /// Event argument used when notifying status change from a IStatusNotifier object
    /// </summary>
    public class StatusUpdateEventArgs : EventArgs
    {
        readonly string m_Status;
        readonly int m_StatusLevel;

        /// <summary>
        /// Gets the status message
        /// </summary>
        public string Status
        {
            get { return m_Status; }
        }

        /// <summary>
        /// Gets the status level 
        /// </summary>
        public int StatusLevel
        {
            get { return m_StatusLevel; }
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
            m_StatusLevel = statusLevel;
            status = String.Format(CultureInfo.CurrentCulture, status, objects);
        }

        /// <summary>
        /// Creates an instance of StatusUpdateEventArgs
        /// </summary>
        /// <param name="statusLevel">Gets the status level <see cref="Partners.Core.StatusLevel"/></param>
        /// <param name="status">The status message</param>
        public StatusUpdateEventArgs(int statusLevel, string status)
        {
            m_StatusLevel = statusLevel;
            m_Status = status;
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
