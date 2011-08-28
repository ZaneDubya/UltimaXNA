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

        private GraphicEffectPacket _packet;
        private GraphicEffectType effectType
        {
            get { return _packet.EffectType; }
        }



        public DynamicObject(IIsometricRenderer world)
            : base(Serial.NewDynamicSerial, world)
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
                switch (effectType)
                {
                    case GraphicEffectType.Nothing:
                        this.Dispose();
                        break;
                    case GraphicEffectType.Moving:
                        this.Dispose();
                        break;
                    case GraphicEffectType.Lightning:
                        X = _packet.SourceX;
                        Y = _packet.SourceY;
                        Z = _packet.SourceZ;
                        _useGumpArtInsteadOfTileArt = true;
                        _baseItemID = 20000;
                        _frameLength = 10;

                        _frameSequence = 0;
                        _timeRepeatAnimationSeconds = 1.0f;
                        _timeEndTotalSeconds += _timeRepeatAnimationSeconds;
                        break;
                    case GraphicEffectType.FixedXYZ:
                        data = ParticleData.GetData(_packet.BaseItemID);
                        if (data != null)
                        {
                            X = _packet.SourceX;
                            Y = _packet.SourceY;
                            Z = _packet.SourceZ;
                            _baseItemID = data.ItemID;
                            _frameLength = data.FrameLength;

                            _frameSequence = 0;
                            _timeRepeatAnimationSeconds = _packet.Duration / (float)(10 + _packet.Speed);
                            _timeEndTotalSeconds += _timeRepeatAnimationSeconds;
                        }
                        else
                            this.Dispose();
                        break;
                    case GraphicEffectType.FixedFrom:
                        data = ParticleData.GetData(_packet.BaseItemID);
                        if (data != null)
                        {
                            X = _packet.SourceX;
                            Y = _packet.SourceY;
                            Z = _packet.SourceZ;
                            _baseItemID = data.ItemID;
                            _frameLength = data.FrameLength;

                            _frameSequence = 0;
                            _timeRepeatAnimationSeconds = _packet.Duration / (float)(10 + _packet.Speed);
                            _timeEndTotalSeconds += _timeRepeatAnimationSeconds;
                        }
                        else
                            this.Dispose();
                        break;
                    case GraphicEffectType.ScreenFade:
                        this.Dispose();
                        break;
                }
            }
            else
            {
                _frameSequence = (float)(gameTime.TotalGameTime.TotalSeconds - _timeBeginTotalSeconds) / _timeRepeatAnimationSeconds;
            }

            if (gameTime.TotalGameTime.TotalSeconds >= _timeEndTotalSeconds)
                this.Dispose();

            base.Update(gameTime);
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            tile.FlushObjectsBySerial(Serial);
            int hue = (_isHued) ? ((GraphicEffectHuedPacket)_packet).Hue : 0;
            tile.AddMapObject(new TileEngine.MapObjectDynamic(this, position, _baseItemID, (int)(_frameSequence * _frameLength), hue, _useGumpArtInsteadOfTileArt));
        }

        public void LoadFromPacket(GraphicEffectPacket packet)
        {
            _isHued = false;
            _packet = packet;
        }

        public void LoadFromPacket(GraphicEffectHuedPacket packet)
        {
            _isHued = true;
            _packet = packet;
        }
    }
}
