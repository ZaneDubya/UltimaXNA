/***************************************************************************
 *   Tracer.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Listeners;

namespace UltimaXNA.Core.Diagnostics.Tracing
{
    public static class Tracer
    {
        public static void RegisterListener(AEventListener listener, EventLevels eventLevel = EventLevels.Verbose)
        {
            m_Listeners.Add(listener);
        }

        public static void UnregisterListener(AEventListener listener)
        {
            if (m_Listeners.Contains(listener))
                m_Listeners.Remove(listener);
        }

        private static readonly List<AEventListener> m_Listeners = new List<AEventListener>();

        public static void Critical(string message)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Critical, message);
        }

        public static void Critical(Exception ex)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Critical, ex);
        }

        public static void Critical(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Critical, message, args);
        }

        public static void Error(string message)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Error, message);
        }

        public static void Error(Exception ex)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Error, ex);
        }

        public static void Error(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Error, message, args);
        }

        public static void Warn(string message)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Warning, message);
        }

        public static void Warn(Exception ex)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Warning, ex);
        }

        public static void Warn(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Warning, message, args);
        }

        public static void Verbose(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Verbose, message, args);
        }

        public static void Debug(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Info, message, args);
        }

        public static void Info(string message, params object[] args)
        {
            foreach (AEventListener listener in m_Listeners)
                listener.OnEventWritten(EventLevels.Info, message, args);
        }
    }
}