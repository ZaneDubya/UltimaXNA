using System.Diagnostics.Tracing;

namespace UltimaXNA.Diagnostics.Tracing
{
    internal sealed class TracerEventSource : EventSource
    {
        public static readonly TracerEventSource Instance = new TracerEventSource();

        [Event(TraceEventId.Critical, Level = EventLevel.Critical)]
        public void Critical(string message)
        {
            WriteEvent(TraceEventId.Critical, message);
        }

        [Event(TraceEventId.Error, Level = EventLevel.Error)]
        public void Error(string message)
        {
            WriteEvent(TraceEventId.Error, message);
        }

        [Event(TraceEventId.Info, Level = EventLevel.Informational)]
        public void Info(string message)
        {
            WriteEvent(TraceEventId.Info, message);
        }

        [Event(TraceEventId.Verbose, Level = EventLevel.Verbose)]
        public void Verbose(string message)
        {
            WriteEvent(TraceEventId.Verbose, message);
        }

        [Event(TraceEventId.Warning, Level = EventLevel.Warning)]
        public void Warn(string message)
        {
            WriteEvent(TraceEventId.Warning, message);
        }
    }
}