using System;
using System.Diagnostics;

namespace UltimaXNA.Core.Diagnostics.Tracing.Listeners
{
    public class DebugOutputEventListener : AEventListener
    {
        private const string Format = "{0} {1:yyyy-MM-dd HH\\:mm\\:ss\\:ffff} {2}";

        public override void OnEventWritten(EventLevel level, string message)
        {
            string output = string.Format(Format, level, DateTime.Now, message);
            Debug.WriteLine(output);
        }
    }
}