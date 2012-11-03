/***************************************************************************
 *   CallPriorityComparer.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UltimaXNA.Core.Extensions
{
    public class CallPriorityComparer : IComparer<MethodInfo>
    {
        public int Compare(MethodInfo x, MethodInfo y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null)
                return 1;

            if (y == null)
                return -1;

            return GetPriority(x) - GetPriority(y);
        }

        private int GetPriority(MethodInfo mi)
        {
            object[] objs = mi.GetCustomAttributes(typeof(CallPriorityAttribute), true);

            if (objs == null)
                return 0;

            if (objs.Length == 0)
                return 0;

            CallPriorityAttribute attr = objs[0] as CallPriorityAttribute;

            if (attr == null)
                return 0;

            return attr.Priority;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CallPriorityAttribute : Attribute
    {
        private int _Priority;

        public int Priority
        {
            get { return _Priority; }
            set { _Priority = value; }
        }

        public CallPriorityAttribute(int priority)
        {
            _Priority = priority;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TypeAliasAttribute : Attribute
    {
        private string[] m_Aliases;

        public string[] Aliases
        {
            get
            {
                return m_Aliases;
            }
        }

        public TypeAliasAttribute(params string[] aliases)
        {
            m_Aliases = aliases;
        }
    }
}
