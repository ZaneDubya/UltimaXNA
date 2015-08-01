using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Diagnostics.Tracing;
using System.Threading;

namespace UltimaXNA.Core
{
    public class DelayedAction
    {
        private volatile Action m_Action;

        private DelayedAction(Action action, int msDelay)
        {
            m_Action = action;
            dynamic timer = new Timer(TimerProc);
            timer.Change(msDelay, Timeout.Infinite);
        }

        private void TimerProc(object state)
        {
            try
            {
                // The state object is the Timer object. 
                ((Timer)state).Dispose();
                m_Action.Invoke();
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
            }
        }

        public static DelayedAction Start(Action callback, int msDelay)
        {
            return new DelayedAction(callback, msDelay);
        }
    }
}
