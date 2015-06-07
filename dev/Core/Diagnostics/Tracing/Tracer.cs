using System;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Listeners;

namespace UltimaXNA.Core.Diagnostics.Tracing
{
    public static class Tracer
    {
        public static void RegisterListener(AEventListener listener, EventLevel eventLevel = EventLevel.Verbose)
        {
            m_Listeners.Add(listener);
        }

        public static void UnregisterListener(AEventListener listener)
        {
            if (m_Listeners.Contains(listener))
                m_Listeners.Remove(listener);
        }

        private static List<AEventListener> m_Listeners = new List<AEventListener>();

        public static void Critical(string message)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Critical, message);
        }

        public static void Critical(Exception ex)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Critical, ex);
        }

        public static void Critical(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Critical, message, args);
        }

        public static void Error(string message)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Error, message);
        }

        public static void Error(Exception ex)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Error, ex);
        }

        public static void Error(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Error, message, args);
        }

        public static void Warn(string message)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Warning, message);
        }

        public static void Warn(Exception ex)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Warning, ex);
        }

        public static void Warn(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Warning, message, args);
        }

        public static void Verbose(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Verbose, message, args);
        }

        public static void Debug(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Info, message, args);
        }

        public static void Info(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevel.Info, message, args);
        }
    }
}