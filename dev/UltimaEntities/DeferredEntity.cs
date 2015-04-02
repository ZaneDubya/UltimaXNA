using UltimaXNA.UltimaEntities.EntityViews;
using UltimaXNA.UltimaWorld.Model;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaEntities.Effects;

namespace UltimaXNA.UltimaEntities
{
    public class DeferredEntity : AEntity
    {
        private int m_Z;
        public new int Z
        {
            get { return m_Z; }
        }

        private Vector3 m_DrawPosition;
        private AEntityView m_BaseView;

        public DeferredEntity(AEntity entity, Vector3 drawPosition, int z)
            : base(entity.Serial, entity.Map)
        {

            m_BaseView = GetBaseView(entity);
            m_DrawPosition = drawPosition;
            m_Z = z;
        }

        private AEntityView GetBaseView(AEntity entity)
        {
            if (entity is Mobile)
                return (MobileView)entity.GetView();
            else if (entity is LightningEffect)
                return (LightningEffectView)entity.GetView();
            else if (entity is AnimatedItemEffect)
                return (AnimatedItemEffectView)entity.GetView();
            else if (entity is MovingEffect)
                return (MovingEffectView)entity.GetView();
            else
                Core.Diagnostics.Logger.Fatal("Cannot defer this type of object.");
            return null;
        }

        protected override AEntityView CreateView()
        {
            return new DeferredView(m_DrawPosition, m_BaseView);
        }
    }
}
