/***************************************************************************
 *   DeferredEntity.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.World.Entities.Effects;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.World.EntityViews;

namespace UltimaXNA.Ultima.World.Entities
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
                Tracer.Critical("Cannot defer this type of object.");
            return null;
        }

        protected override AEntityView CreateView()
        {
            return new DeferredView(m_DrawPosition, m_BaseView);
        }
    }
}
