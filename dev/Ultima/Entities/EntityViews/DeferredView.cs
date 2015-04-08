using Microsoft.Xna.Framework;
using UltimaXNA.Core.Rendering;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Maps;
using UltimaXNA.UltimaWorld.Controllers;

namespace UltimaXNA.UltimaEntities.EntityViews
{
    public class DeferredView : AEntityView
    {
        Vector3 m_DrawPosition;
        AEntityView m_BaseView;

        public DeferredView(Vector3 drawPosition, AEntityView baseView)
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
