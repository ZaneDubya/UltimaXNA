using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Core.Diagnostics.Tracing.Listeners
{
    class MsgBoxOnCriticalListener : AEventListener
    {
        public override void OnEventWritten(EventLevel level, string messag)
        {
            if (level == EventLevel.Critical)
                throw new Exception(messag);
        }
    }
}
