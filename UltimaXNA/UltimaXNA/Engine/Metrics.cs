using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA
{
    static class Metrics
    {
        static int _dataRead = 0;
        public static int DataRead
        {
            get { return _dataRead; }
            internal set { _dataRead = value; }
        }

        public static void ReportDataRead(int dataAmount)
        {
            DataRead += dataAmount;
        }
    }
}
