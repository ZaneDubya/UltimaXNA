using System;

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
}