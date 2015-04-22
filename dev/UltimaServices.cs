using System;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Tracing;

namespace UltimaXNA
{
    public static class UltimaServices
    {
        private static Dictionary<Type, object> m_Services = new Dictionary<Type, object>();

        public static T Register<T>(T service)
        {
            Type type = typeof(T);

            if (m_Services.ContainsKey(type))
            {
                Tracer.Critical(string.Format("Attempted to register service of type {0} twice.", type.ToString()));
                m_Services.Remove(type);
            }

            m_Services.Add(type, service);
            return service;
        }

        public static void Unregister<T>(T service)
        {
            Type type = typeof(T);

            if (m_Services.ContainsKey(type) && (object)m_Services[type] == (object)service)
            {
                m_Services.Remove(type);
            }
            else
            {
                Tracer.Critical(string.Format("Attempted to unregister service of type {0}, but no service of this type (or type and equality) is registered.", type.ToString()));
            }
        }

        public static T GetService<T>()
        {
            Type type = typeof(T);

            if (m_Services.ContainsKey(type))
            {
                return (T)m_Services[type];
            }
            else
            {
                Tracer.Critical(string.Format("Attempted to get service service of type {0}, but no service of this type is registered.", type.ToString()));
                return default(T);
            }
        }
    }
}
