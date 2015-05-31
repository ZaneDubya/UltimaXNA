/***************************************************************************
 *   Corpse.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Network.Server;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.Entities.Items.Containers
{
    class Corpse : Container
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

        public Corpse(Serial serial, Map map)
            : base(serial, map)
        {

        }

        protected override EntityViews.AEntityView CreateView()
        {
            return new EntityViews.CorpseView(this);
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            Frame += ((float)frameMS / 500f);
            if (Frame >= 1f)
                Frame = 0.999f;
        }

        public void LoadCorpseClothing(List<CorpseClothingPacket.CorpseItem> items)
        {

        }

        public void PlayDeathAnimation()
        {
            Frame = 0f;
        }
    }
}
