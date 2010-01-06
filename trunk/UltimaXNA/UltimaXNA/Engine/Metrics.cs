using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UltimaXNA
{
    static class Metrics
    {
        static List<NameValuePair> _dataReadList = new List<NameValuePair>();
        public static int TotalDataRead
        {
            get
            {
                int total = 0;
                foreach (NameValuePair p in _dataReadList)
                    total += p.Value;
                return total;
            }
        }

        static string _dataReadBreakdown;
        static bool _dataReadBreakdown_MustUpdate = true;
        public static string DataReadBreakdown
        {
            get
            {
                if (_dataReadBreakdown_MustUpdate)
                {
                    _dataReadBreakdown_MustUpdate = false;
                    _dataReadBreakdown = string.Empty;
                    foreach (NameValuePair p in _dataReadList)
                        _dataReadBreakdown += p.Name + ": " + p.Value + '\n';
                }
                return _dataReadBreakdown;
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
            foreach (NameValuePair p in _dataReadList)
            {
                if (p.Name == name)
                {
                    mustAddPair = false;
                    p.Value += dataAmount;
                }
            }
            if (mustAddPair)
            {
                _dataReadList.Add(new NameValuePair(name, dataAmount));
            }
            _dataReadBreakdown_MustUpdate = true;
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
