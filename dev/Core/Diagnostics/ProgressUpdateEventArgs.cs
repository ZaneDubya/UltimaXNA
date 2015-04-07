using System;

namespace UltimaXNA.Core.Diagnostics
{
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
}