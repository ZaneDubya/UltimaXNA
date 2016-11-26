/***************************************************************************
 *   PropertyList.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using System.Text;
#endregion

namespace UltimaXNA.Ultima.World.Entities
{
    public class PropertyList
    {
        public int Hash;
        private readonly List<string> m_PropertyList = new List<string>();

        public bool HasProperties
        {
            get
            {
                if (m_PropertyList.Count == 0)
                    return false;
                else
                    return true;
            }
        }

        public string Properties
        {
            get
            {
                StringBuilder concat = new StringBuilder();
                for (int i = 0; i < m_PropertyList.Count; i++)
                {
                    concat.Append(m_PropertyList[i]);
                    if (i < m_PropertyList.Count - 1)
                    {
                        concat.Append('\n');
                    }
                }
                return concat.ToString();
            }
        }

        public void Clear()
        {
            m_PropertyList.Clear();
        }

        public void AddProperty(string nProperty)
        {
            m_PropertyList.Add(nProperty);
        }
    }
}
