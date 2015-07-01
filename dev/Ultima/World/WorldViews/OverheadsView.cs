using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.Input;

namespace UltimaXNA.Ultima.World.WorldViews
{
    static class OverheadRenderer
    {
        private static List<ViewWithDrawInfo> m_Views = new List<ViewWithDrawInfo>();

        public static void AddView(AEntityView view, Vector3 drawPosition)
        {
            m_Views.Add(new ViewWithDrawInfo(view, drawPosition));
        }

        public static void Render(SpriteBatch3D spriteBatch, MouseOverList mouseOverList, Map map)
        {
            if (m_Views.Count > 0)
            {
                for (int i = 0; i < m_Views.Count; i++)
                {
                    m_Views[i].View.Draw(spriteBatch, m_Views[i].DrawPosition, mouseOverList, map);
                }

                m_Views.Clear();
            }
        }

        private struct ViewWithDrawInfo
        {
            public readonly AEntityView View;
            public readonly Vector3 DrawPosition;

            public ViewWithDrawInfo(AEntityView view, Vector3 drawPosition)
            {
                View = view;
                DrawPosition = drawPosition;
            }
        }
    }
}
