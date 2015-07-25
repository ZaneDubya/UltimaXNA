/***************************************************************************
 *   WorldClient.cs
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
using UltimaXNA.Ultima.World.Entities.Items;
#endregion

namespace UltimaXNA.Ultima.World.Managers
{
    public class StaticManager
    {
        private List<StaticItem> m_ActiveStatics = new List<StaticItem>();

        public void AddStaticThatNeedsUpdating(StaticItem item)
        {
            if (item.IsDisposed || item.Overheads.Count == 0)
                return;

            m_ActiveStatics.Add(item);
        }

        public void Update(double frameMS)
        {
            for (int i = 0; i < m_ActiveStatics.Count; i++)
            {
                m_ActiveStatics[i].Update(frameMS);
                if (m_ActiveStatics[i].IsDisposed || m_ActiveStatics[i].Overheads.Count == 0)
                    m_ActiveStatics.RemoveAt(i);
            }
        }
    }
}
