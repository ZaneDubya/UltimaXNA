/***************************************************************************
 *   ContextMenu.cs
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
using System.Linq;
using UltimaXNA.Ultima.Resources;
#endregion

namespace UltimaXNA.Ultima.Data
{
    public class ContextMenuData
    {
        private readonly List<ContextMenuItem> m_Entries = new List<ContextMenuItem>();
        private readonly Serial m_Serial;

        public ContextMenuData(Serial serial)
        {
            m_Serial = serial;
        }

        public Serial Serial
        {
            get { return m_Serial; }
        }

        public int Count
        {
            get { return m_Entries.Count; }
        }

        public ContextMenuItem this[int index]
        {
            get
            {
                if (index < 0 || index >= m_Entries.Count)
                    return null;
                return m_Entries[index];
            }
        }

        // Add a new context menu entry.
        internal void AddItem(int nResponseCode, int nStringID, int nFlags, int nHue)
        {
            m_Entries.Add(new ContextMenuItem(nResponseCode, nStringID, nFlags, nHue));
        }
    }
}