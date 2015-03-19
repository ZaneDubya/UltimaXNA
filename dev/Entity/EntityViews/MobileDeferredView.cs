using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Model;

namespace UltimaXNA.Entity.EntityViews
{
    public class MobileDeferredView : AEntityView
    {
        Vector3 m_DrawPosition;
        MobileView m_BaseView;

        public MobileDeferredView(Vector3 drawPosition, MobileView baseView)
            : base(baseView.Entity)
        {
            m_DrawPosition = drawPosition;
            m_BaseView = baseView;
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map)
        {
            m_BaseView.SetAllowDefer();
            return m_BaseView.Draw(spriteBatch, m_DrawPosition, mouseOverList, map);
        }
    }
}
