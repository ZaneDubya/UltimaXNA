/***************************************************************************
 *   DynamicObject.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.TileEngine;
using UltimaXNA.Client.Packets.Server;
using Microsoft.Xna.Framework;
using UltimaXNA.Data;
#endregion

namespace UltimaXNA.Entities
{
    public class DynamicObject : Entity
    {
        private bool _isHued = false;
        private bool _isInitialized = false;
        private GraphicEffectPacket _data;
        private ParticleData _particle;
        private GameTime _beginTime;
        private int _frameCount;

        public DynamicObject(IIsometricRenderer world)
            : base(Serial.NewDynamicSerial, world)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!_isInitialized)
            {
                _particle = ParticleData.GetData(_data.BaseItemID);
                _beginTime = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                this.X = _data.SourceX;
                this.Y = _data.SourceY;
                this.Z = _data.SourceZ;
                _frameCount = 0;
                _isInitialized = true;
            }
            else
            {
                _frameCount++;
            }

            if (_frameCount >= _data.Duration)
            {
                this.Dispose();
            }

            base.Update(gameTime);
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            tile.FlushObjectsBySerial(Serial);
            int hue = (_isHued) ? ((GraphicEffectHuedPacket)_data).Hue : 0;
            tile.AddMapObject(new TileEngine.MapObjectDynamic(this, position, _particle.ItemID, (_frameCount % _particle.FrameLength), hue));
        }

        public void LoadFromPacket(GraphicEffectPacket packet)
        {
            _isHued = false;
            _data = packet;
        }

        public void LoadFromPacket(GraphicEffectHuedPacket packet)
        {
            _isHued = true;
            _data = packet;
        }
    }
}
