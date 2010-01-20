using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.EventSystem
{
    public interface IEventService
    {
        void Register(IEventReceiver registerable, short eventId);
        void Unregister(IEventReceiver registerable, short eventId);
        void RegisterAll(IEventReceiver registerable);
        void UnregisterAll(IEventReceiver registerable);
        void SendEvent(object sender, short eventId, params object[] args);
        bool IsRegistered(IEventReceiver sender, short eventId);
    }
}
