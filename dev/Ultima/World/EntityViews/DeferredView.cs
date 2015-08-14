using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;

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
            if (m_BaseView.Entity is Ultima.World.Entities.Mobiles.Mobile && !(m_BaseView.Entity as Ultima.World.Entities.Mobiles.Mobile).IsAlive)
                return false;
            m_BaseView.SetAllowDefer();
            return m_BaseView.Draw(spriteBatch, m_DrawPosition, mouseOverList, map);
        }
    }
}
