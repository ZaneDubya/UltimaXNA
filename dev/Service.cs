/***************************************************************************
 *   Services.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Tracing;
#endregion

namespace UltimaXNA
{
    public static class Service
    {
        static readonly Dictionary<Type, object> m_Services = new Dictionary<Type, object>();

        public static T Add<T>(T service)
        {
            Type type = typeof(T);
            if (m_Services.ContainsKey(type))
            {
                Tracer.Critical(string.Format("Attempted to register service of type {0} twice.", type));
                m_Services.Remove(type);
            }
            m_Services.Add(type, service);
            return service;
        }

        public static void Remove<T>()
        {
            Type type = typeof(T);
            if (m_Services.ContainsKey(type))
            {
                m_Services.Remove(type);
            }
            else
            {
                Tracer.Critical(string.Format("Attempted to unregister service of type {0}, but no service of this type (or type and equality) is registered.", type));
            }
        }

        public static bool Has<T>()
        {
            Type type = typeof(T);
            return m_Services.ContainsKey(type);
        }

        public static T Get<T>(bool failIfNotRegistered = true)
        {
            Type type = typeof(T);
            if (m_Services.ContainsKey(type))
            {
                return (T)m_Services[type];
            }
            if (failIfNotRegistered)
            {
                Tracer.Critical(string.Format("Attempted to get service service of type {0}, but no service of this type is registered.", type));
            }
            return default(T);
        }
    }
}
