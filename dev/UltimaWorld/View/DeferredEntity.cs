using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.View;
using UltimaXNA.Entity;
using UltimaXNA.Entity.View;

namespace UltimaXNA.UltimaWorld.View
{
    public class DeferredEntity : AEntity
    {
        private AEntity m_BaseEntity;
        public AEntity BaseEntity
        {
            get
            {
                return m_BaseEntity;
            }
        }

        public DeferredEntity(AEntity entity)
            : base(entity.Serial)
        {
            m_BaseEntity = entity;
        }
    }
}
