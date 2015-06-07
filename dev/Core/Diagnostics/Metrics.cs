/***************************************************************************
 *   Metrics.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UltimaXNA.Core.Diagnostics
{
    static class Metrics
    {
        static List<NameValuePair> m_dataReadList = new List<NameValuePair>();
        public static int TotalDataRead
        {
            get
            {
                int total = 0;
                foreach (NameValuePair p in m_dataReadList)
                    total += p.Value;
                return total;
            }
        }

        static string m_dataReadBreakdown;
        static bool m_dataReadBreakdown_MustUpdate = true;
        public static string DataReadBreakdown
        {
            get
            {
                if (m_dataReadBreakdown_MustUpdate)
                {
                    m_dataReadBreakdown_MustUpdate = false;
                    m_dataReadBreakdown = "Data Read from HDD:";
                    foreach (NameValuePair p in m_dataReadList)
                        m_dataReadBreakdown += '\n' + p.Name + ": " + p.Value;
                }
                return m_dataReadBreakdown;
            }
        }

        public static void ReportDataRead(int dataAmount)
        {
            string name;
#if DEBUG
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            name = methodBase.DeclaringType.FullName;
#else
            name = "Total";
#endif
            bool mustAddPair = true;
            foreach (NameValuePair p in m_dataReadList)
            {
                if (p.Name == name)
                {
                    mustAddPair = false;
                    p.Value += dataAmount;
                }
            }
            if (mustAddPair)
            {
                m_dataReadList.Add(new NameValuePair(name, dataAmount));
            }
            m_dataReadBreakdown_MustUpdate = true;
        }
    }

    internal class NameValuePair
    {
        public string Name = string.Empty;
        public int Value = 0;

        public NameValuePair(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}
