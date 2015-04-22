using System;

namespace UltimaXNA.Core.Diagnostics.Tracing.Listeners
{
    public abstract class AEventListener
    {
        public abstract void OnEventWritten(EventLevel level, string message);

        public void OnEventWritten(EventLevel level, string message, params object[] args)
        {
            OnEventWritten(level, string.Format(message, args));
        }

        public void OnEventWritten(EventLevel level, Exception ex)
        {
            OnEventWritten(level, ex.Message);
        }
    }
}
