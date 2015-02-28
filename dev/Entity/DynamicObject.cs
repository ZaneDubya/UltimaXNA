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
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaNetwork.Packets.Server;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaData;
#endregion

namespace UltimaXNA.Entity
{
    public class DynamicObject : BaseEntity
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

        public DynamicObject(Serial serial)
            : base(serial)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!_isInitialized)
            {
                _timeEndTotalSeconds = _timeBeginTotalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
                _frameSequence = 0;
                _isInitialized = true;

                ParticleData itemData;
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
                        itemData = ParticleData.GetData(_baseItemID);
                        if (itemData != null)
                        {
                            // we may need to remap baseItemID from the original value:
                            _baseItemID = itemData.ItemID;
                            _frameLength = itemData.FrameLength;
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
                    DynamicObject dynamic = EntityManager.AddDynamicObject();
                    dynamic.Load_AsExplosion(_targetX, _targetY, _targetZ);
                }
                this.Dispose();
            }

            base.Update(gameTime);
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            tile.FlushObjectsBySerial(Serial);
            int hue = _isHued ? _hue : 0;
            tile.AddMapObject(new MapObjectDynamic(this, position, _baseItemID, (int)(_frameSequence * _frameLength), hue, _useGumpArtInsteadOfTileArt));
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
            ParticleData itemData = ParticleData.RandomExplosion;
            _baseItemID = itemData.ItemID;
            _duration = itemData.FrameLength;
            _speed = 0;
            _doesExplode = false;
        }
    }
}
