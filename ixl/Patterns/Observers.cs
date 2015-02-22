using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.Patterns
{
    public class BaseObservers<T> where T : struct
    {
        public delegate void EventObserver(object entity = null);
        private static Dictionary<T, List<EventObserver>> m_EventObservers;

        static BaseObservers()
        {
            m_EventObservers = new Dictionary<T, List<EventObserver>>();
        }

        public static void RegisterEventObserver(T event_id, EventObserver observer)
        {
            if (!m_EventObservers.ContainsKey(event_id))
                m_EventObservers.Add(event_id, new List<EventObserver>());
            List<EventObserver> list = m_EventObservers[event_id];
            if (!list.Contains(observer))
                list.Add(observer);
        }

        public static void UnregisterEventObserver(T event_id, EventObserver observer)
        {
            if (!m_EventObservers.ContainsKey(event_id))
                return;
            List<EventObserver> list = m_EventObservers[event_id];
            if (list.Contains(observer))
                list.Remove(observer);
        }

        public static void AnnounceEvent(T event_id, object announcing_entity)
        {
            if (!m_EventObservers.ContainsKey(event_id))
                return;
            List<EventObserver> list = m_EventObservers[event_id];
            foreach (EventObserver observer in list)
                observer.Invoke(announcing_entity);
        }
    }
}
