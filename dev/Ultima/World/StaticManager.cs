using System.Collections.Generic;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Items;

namespace UltimaXNA.Ultima.World
{
    public static class StaticManager
    {
        private static List<StaticItem> m_ActiveStatics = new List<StaticItem>();

        public static void AddStaticThatNeedsUpdating(StaticItem item)
        {
            if (item.IsDisposed || item.Overheads.Count == 0)
                return;

            m_ActiveStatics.Add(item);
        }

        public static void Update(double frameMS)
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
