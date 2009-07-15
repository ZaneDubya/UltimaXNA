#region File Description & Usings
//-----------------------------------------------------------------------------
// Corpse.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.GameObjects
{
    class Corpse : Item
    {
        public Serial MobileSerial = 0;

        private float _corpseFrame = 0.999f;
        private int _corpseBody { get { return Item_StackCount; } }

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

        internal override void  Draw(UltimaXNA.TileEngine.MapCell nCell, Microsoft.Xna.Framework.Vector3 nLocation, Microsoft.Xna.Framework.Vector3 nOffset)
        {
            Movement.ClearImmediate();
                nCell.AddGameObjectTile(
                    new TileEngine.GameObjectTile(
                        ObjectTypeID, nLocation, Movement.DrawFacing, this.Serial, Hue, _corpseBody, _corpseFrame));
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
