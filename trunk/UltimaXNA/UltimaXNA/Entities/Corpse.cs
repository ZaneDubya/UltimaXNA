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
#endregion

namespace UltimaXNA.Entities
{
    class Corpse : ContainerItem
    {
        public Serial MobileSerial = 0;

        private float _corpseFrame = 0.999f;
        private int _corpseBody { get { return Amount; } }

        public Corpse(Serial serial)
            : base(serial)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_corpseFrame != 0.999f)
            {
                _hasBeenDrawn = false;
                _corpseFrame += ((float)gameTime.ElapsedGameTime.Milliseconds / 500f);
                if (_corpseFrame >= 1f)
                    _corpseFrame = 0.999f;
            }
        }

        internal override void Draw(UltimaXNA.TileEngine.MapCell cell, Microsoft.Xna.Framework.Vector3 nLocation, Microsoft.Xna.Framework.Vector3 nOffset)
        {
            Movement.ClearImmediate();
            cell.Add(new TileEngine.MapObjectCorpse(nLocation, Movement.DrawFacing, this, Hue, _corpseBody, _corpseFrame));
        }

        public void LoadCorpseClothing(List<Network.Packets.Server.CorpseClothingItemWithLayer> items)
        {

        }

        public void DeathAnimation()
        {
            _corpseFrame = 0f;
        }
    }
}
