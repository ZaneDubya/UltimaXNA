using UltimaXNA.Entity.EntityViews;

namespace UltimaXNA.Entity
{
    public class MobileDeferred : AEntity
    {
        private Mobile m_BaseEntity;
        public Mobile BaseEntity
        {
            get
            {
                return m_BaseEntity;
            }
        }

        private int m_Z;
        public new int Z
        {
            get { return m_Z; }
        }

        public MobileDeferred(Mobile entity, int z)
            : base(entity.Serial)
        {
            m_BaseEntity = entity;
            m_Z = z;
        }

        public override void Update(double frameMS)
        {
            this.Dispose();
        }

        protected override AEntityView CreateView()
        {
            return new MobileView(m_BaseEntity, true);
        }
    }
}
