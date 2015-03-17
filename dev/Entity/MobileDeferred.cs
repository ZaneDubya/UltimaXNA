using UltimaXNA.Entity.EntityViews;
using UltimaXNA.UltimaWorld.Model;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Entity
{
    public class MobileDeferred : AEntity
    {
        private int m_Z;
        public new int Z
        {
            get { return m_Z; }
        }

        private Vector3 m_DrawPosition;
        private MobileView m_BaseView;

        public MobileDeferred(Mobile entity, Vector3 drawPosition, int z)
            : base(entity.Serial)
        {
            m_BaseView = (MobileView)entity.GetView();
            m_DrawPosition = drawPosition;
            m_Z = z;
        }

        protected override AEntityView CreateView()
        {
            return new MobileDeferredView(m_DrawPosition, m_BaseView);
        }
    }
}
