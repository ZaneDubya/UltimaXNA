/***************************************************************************
 *   ServiceRegistry.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima.World.Entities.Items.Containers
{
    class Corpse : ContainerItem
    {
        public Serial MobileSerial = 0;

        public float Frame = 0.999f;
        public Body Body { get { return Amount; } }

        private Direction m_Facing = Direction.Nothing;
        public Direction Facing
        {
            get { return m_Facing & Direction.FacingMask; }
            set { m_Facing = value; }
        }

        public readonly bool DieForwards;

        public Corpse(Serial serial, Map map)
            : base(serial, map)
        {
            Equipment = new MobileEquipment(this);
            DieForwards = Utility.RandomValue(0, 1) == 0;
        }

        protected override AEntityView CreateView()
        {
            return new MobileView(this);
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            Frame += ((float)frameMS / 500f);
            if (Frame >= 1f)
                Frame = 0.999f;
        }

        public void PlayDeathAnimation()
        {
            Frame = 0f;
        }

        public MobileEquipment Equipment;

        public override void RemoveItem(Serial serial)
        {
            base.RemoveItem(serial);
            Equipment.RemoveBySerial(serial);
            m_OnUpdated?.Invoke(this);
        }
    }
}
