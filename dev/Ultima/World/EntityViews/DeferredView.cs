using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.Input;

namespace UltimaXNA.Ultima.World.EntityViews
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
