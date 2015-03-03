/***************************************************************************
 *   Corpse.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Server;
#endregion

namespace UltimaXNA.Entity
{
    class Corpse : Container
    {
        public Serial MobileSerial = 0;

        private float m_corpseFrame = 0.999f;
        private int m_corpseBody { get { return Amount; } }

        public Corpse(Serial serial)
            : base(serial)
        {

        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            // HasBeenDrawn = false;
            m_corpseFrame += ((float)frameMS / 500f);
            if (m_corpseFrame >= 1f)
                m_corpseFrame = 0.999f;
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            m_movement.ClearImmediate();
            tile.AddMapObject(new MapObjectCorpse(position, DrawFacing, this, Hue, m_corpseBody, m_corpseFrame));
            drawOverheads(tile, new Position3D(m_movement.Position.Point_V3));
        }

        public void LoadCorpseClothing(List<CorpseClothingPacket.CorpseItem> items)
        {

        }

        public void DeathAnimation()
        {
            m_corpseFrame = 0f;
        }
    }
}
