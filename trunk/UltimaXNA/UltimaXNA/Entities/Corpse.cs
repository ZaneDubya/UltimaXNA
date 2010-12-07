/***************************************************************************
 *   Corpse.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    class Corpse : Container
    {
        public Serial MobileSerial = 0;

        private float _corpseFrame = 0.999f;
        private int _corpseBody { get { return Amount; } }

        public Corpse(Serial serial, IIsometricRenderer world)
            : base(serial, world)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HasBeenDrawn = false;
            _corpseFrame += ((float)gameTime.ElapsedGameTime.Milliseconds / 500f);
            if (_corpseFrame >= 1f)
                _corpseFrame = 0.999f;
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            _movement.ClearImmediate();
            tile.Add(new TileEngine.MapObjectCorpse(position, DrawFacing, this, Hue, _corpseBody, _corpseFrame));
            drawOverheads(tile, new Position3D(_movement.Position.Point_V3));
        }

        public void LoadCorpseClothing(List<Client.CorpseClothingItemWithLayer> items)
        {

        }

        public void DeathAnimation()
        {
            _corpseFrame = 0f;
        }
    }
}
