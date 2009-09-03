using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.EventSystem
{
    public interface IEventReceiver
    {
        void SendEvent(object sender, int eventId, params object[] args);
    }
}
