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

        private int _baseItemID;
        private float _frameSequence;
        private int _frameLength;
        private bool _useGumpArtInsteadOfTileArt = false;

        private float _timeBeginTotalSeconds;
        private float _timeEndTotalSeconds;
        private float _timeRepeatAnimationSeconds;

        private GraphicEffectType _effectType;
        private int _hue;
        private int _duration;
        private int _speed;
        private bool _doesExplode;
        private GraphicEffectBlendMode _bendMode;
        private int _targetX, _targetY, _targetZ;

        public DynamicObject(Serial serial, IIsometricRenderer world)
            : base(serial, world)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!_isInitialized)
            {
                _timeEndTotalSeconds = _timeBeginTotalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
                _frameSequence = 0;
                _isInitialized = true;

                ParticleData data;
                switch (_effectType)
                {
                    case GraphicEffectType.Nothing:
                        this.Dispose();
                        break;
                    case GraphicEffectType.Moving:
                        this.Dispose();
                        break;
                    case GraphicEffectType.Lightning:
                        _useGumpArtInsteadOfTileArt = true;
                        _baseItemID = 20000;
                        _frameLength = 10;
                        _timeRepeatAnimationSeconds = 0.5f;
                        _timeEndTotalSeconds += _timeRepeatAnimationSeconds;
                        break;
                    case GraphicEffectType.FixedXYZ:
                    case GraphicEffectType.FixedFrom:
                        data = ParticleData.GetData(_baseItemID);
                        if (data != null)
                        {
                            // we may need to remap baseItemID from the original value:
                            _baseItemID = data.ItemID;
                            _frameLength = data.FrameLength;
                            _timeRepeatAnimationSeconds = _duration / (float)(10 + _speed);
                            _timeEndTotalSeconds += _timeRepeatAnimationSeconds;
                        }
                        else
                            this.Dispose();
                        break;
                    case GraphicEffectType.ScreenFade:
                        Diagnostics.Logger.Warn("Unhandled ScreenFade effect.");
                        this.Dispose();
                        break;
                }
            }
            else
            {
                _frameSequence = (float)((gameTime.TotalGameTime.TotalSeconds - _timeBeginTotalSeconds) / _timeRepeatAnimationSeconds) % 1.0f;
            }

            if (gameTime.TotalGameTime.TotalSeconds >= _timeEndTotalSeconds)
            {
                if (_doesExplode)
                {
                    DynamicObject dynamic = EntitiesCollection.AddDynamicObject();
                    dynamic.Load_AsExplosion(_targetX, _targetY, _targetZ);
                }
                this.Dispose();
            }

            base.Update(gameTime);
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            tile.FlushObjectsBySerial(Serial);
            tile.AddMapObject(new TileEngine.MapObjectDynamic(this, position, _baseItemID, (int)(_frameSequence * _frameLength), _hue, _useGumpArtInsteadOfTileArt));
        }

        public void Load_FromPacket(GraphicEffectPacket packet)
        {
            _baseItemID = packet.BaseItemID;
            _effectType = packet.EffectType;
            _isHued = false;
            _hue = 0;
            X = packet.SourceX;
            Y = packet.SourceY;
            Z = packet.SourceZ;
            _targetX = packet.TargetX;
            _targetY = packet.TargetY;
            _targetZ = packet.TargetZ;
            _duration = packet.Duration;
            _speed = packet.Speed;
            _doesExplode = packet.DoesExplode;
        }

        public void Load_FromPacket(GraphicEffectHuedPacket packet)
        {
            this.Load_FromPacket((GraphicEffectPacket)packet);
            _isHued = true;
            _hue = packet.Hue;
            _bendMode = packet.BlendMode;
        }

        public void Load_AsExplosion(int x, int y, int z)
        {
            _effectType = GraphicEffectType.FixedXYZ;
            _isHued = false;
            _hue = 0;
            X = x;
            Y = y;
            Z = z;
            ParticleData data = ParticleData.RandomExplosion;
            _baseItemID = data.ItemID;
            _duration = data.FrameLength;
            _speed = 0;
            _doesExplode = false;
        }
    }
}
