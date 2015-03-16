using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib
{
    public static class Logging
    {
        // TRACE < DEBUG < INFO < WARN < ERROR < FATAL
        public static void Fatal(string error)
        {
            throw new Exception(error);
        }

        public static void Fatal(string error, params object[] args)
        {
            throw new Exception(string.Format(error, args));
        }
    }
}
