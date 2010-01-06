using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Diagnostics;

namespace UltimaXNA.Scripting
{
    public class LuaGlobals
    {
        static Logger _log = new Logger("LuaGlobals");

        public static void Debug(string message)
        {
            _log.Debug(message);
        }
    }
}
