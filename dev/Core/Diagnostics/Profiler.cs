/***************************************************************************
 *   Profiler.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Tracing;

namespace UltimaXNA.Core.Diagnostics
{
    public static class Profiler
    {
        private static List<ContextAndTick> m_Context;
        private static HighPerformanceTimer m_Timer;
        private static List<ProfileData> m_ThisFrameData;
        private static List<ProfileData> m_AllFrameData;

        static Profiler()
        {
            m_Context = new List<ContextAndTick>();
            m_ThisFrameData = new List<ProfileData>();
            m_AllFrameData = new List<ProfileData>();

            m_Timer = new HighPerformanceTimer();
            m_Timer.Start();
        }

        private static long m_BeginFrameTicks;
        public static double LastFrameTimeMS;
        public static double TotalTimeMS;

        public static void BeginFrame()
        {
            if (m_ThisFrameData.Count > 0)
            {
                for (int i = 0; i < m_ThisFrameData.Count; i++)
                {
                    bool added = false;
                    for (int j = 0; j < m_AllFrameData.Count; j++)
                    {
                        if (m_AllFrameData[j].MatchesContext(m_ThisFrameData[i].Context))
                        {
                            m_AllFrameData[j].AddNewHitLength(m_ThisFrameData[i].TimeSpent);
                            added = true;
                            break;
                        }
                    }
                    if (!added)
                        m_AllFrameData.Add(new ProfileData(m_ThisFrameData[i].Context, m_ThisFrameData[i].TimeSpent));
                }
                m_ThisFrameData.Clear();
            }

            m_BeginFrameTicks = m_Timer.ElapsedTicks;
        }

        public static void EndFrame()
        {
            LastFrameTimeMS = HighPerformanceTimer.SecondsFromTicks(m_Timer.ElapsedTicks - m_BeginFrameTicks) * 1000d;
            TotalTimeMS += LastFrameTimeMS;
        }

        public static void EnterContext(string context_name)
        {
            m_Context.Add(new ContextAndTick(context_name, m_Timer.ElapsedTicks));
        }

        public static void ExitContext(string context_name)
        {
            if (m_Context[m_Context.Count - 1].Name != context_name)
                Tracer.Error("Profiler.ExitProfiledContext: context_name does not match current context.");
            string[] context = new string[m_Context.Count];
            for (int i = 0; i < m_Context.Count; i++)
                context[i] = m_Context[i].Name;

            m_ThisFrameData.Add(new ProfileData(context, HighPerformanceTimer.SecondsFromTicks(m_Timer.ElapsedTicks - m_Context[m_Context.Count - 1].Tick)));
            m_Context.RemoveAt(m_Context.Count - 1);
        }

        public static bool InContext(string context_name)
        {
            if (m_Context.Count == 0)
                return false;
            return (m_Context[m_Context.Count - 1].Name == context_name);
        }

        public static ProfileData GetContext(string context_name)
        {
            for (int i = 0; i < m_AllFrameData.Count; i++)
                if (m_AllFrameData[i].Context[m_AllFrameData[i].Context.Length - 1] == context_name)
                    return m_AllFrameData[i];
            return ProfileData.Empty;
        }

        public class ProfileData
        {
            public string[] Context;
            public int HitCount;
            public double TimeSpent;

            private double[] m_Last60Times = new double[60];

            public double LastTime
            {
                get { return m_Last60Times[HitCount % 60]; }
            }

            public double AverageOfLast60Times
            {
                get
                {
                    double value = 0;
                    for (int i = 0; i < 60; i++)
                        value += m_Last60Times[i];
                    return value / 60d;
                }
            }

            public double AverageTime
            {
                get
                {
                    return (TimeSpent / HitCount);
                }
            }

            public ProfileData(string[] context, double time_spent)
            {
                Context = context;
                HitCount = 1;
                TimeSpent = time_spent;
            }

            public bool MatchesContext(string[] context)
            {
                if (Context.Length != context.Length)
                    return false;
                for (int i = 0; i < Context.Length; i++)
                    if (Context[i] != context[i])
                        return false;
                return true;
            }

            public void AddNewHitLength(double time_spent)
            {
                TimeSpent = TimeSpent + time_spent;
                m_Last60Times[HitCount % 60] = time_spent;
                HitCount = HitCount + 1;
            }

            public override string ToString()
            {
                string name = string.Empty;
                for (int i = 0; i < Context.Length; i++)
                {
                    if (name != string.Empty)
                        name += ":";
                    name += Context[i];
                }
                return string.Format("{0} [{1} hits, {2:0.0000} seconds]", name, HitCount, TimeSpent);
            }

            public static ProfileData Empty = new ProfileData(null, 0d);
        }

        private struct ContextAndTick
        {
            public readonly string Name;
            public readonly long Tick;

            public ContextAndTick(string name, long tick)
            {
                Name = name;
                Tick = tick;
            }

            public override string ToString()
            {
                return string.Format("{0} [{1}]", Name, Tick);
            }
        }
    }
}
