using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entity;
using UltimaXNA.Entity.EntityViews;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaWorld.View;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Model;
using System.Collections.Generic;


namespace UltimaXNA.UltimaWorld.View
{
    class DeferredEntityCollection
    {
        private List<DeferredView> m_DeferredViews = new List<DeferredView>();
        private List<DeferredView> m_TheseDeferredViews = new List<DeferredView>();

        public void Clear()
        {
            m_DeferredViews.Clear();
        }

        public void AddDeferredView(DeferredView view)
        {
            m_DeferredViews.Add(view);
        }

        public List<DeferredView> GetDeferredViews(Point tile)
        {
            m_TheseDeferredViews.Clear();
            for (int i = 0; i < m_DeferredViews.Count; i++)
            {
                if (m_DeferredViews[i].Tile == tile)
                {
                    m_TheseDeferredViews.Add(m_DeferredViews[i]);
                    m_DeferredViews.RemoveAt(i);
                    i--;
                }
            }
            return m_TheseDeferredViews;
        }
    }
}
