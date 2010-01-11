using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.TileEngine;

namespace UltimaXNA.Entities
{
    public class Overhead : Entity
    {
        private bool _needsRender;
        private Entity _ownerEntity;

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _needsRender = true;
            }
        }

        private MessageType _msgType;
        public MessageType MsgType
        {
            get { return _msgType; }
            set
            {
                _msgType = value;
            }
        }

        private string _speakerName;
        public string SpeakerName
        {
            get { return _speakerName; }
            set
            {
                _speakerName = value;
                _needsRender = true;
            }
        }

        private int _hue;
        public int Hue
        {
            get { return _hue; }
            set
            {
                _hue = value;
                _needsRender = true;
            }
        }

        private int _font;
        public int Font
        {
            get { return _font; }
            set
            {
                _font = value;
                _needsRender = true;
            }
        }

        private int _msTimePersist = 0;

        public Overhead(Entity ownerEntity, IWorld world, MessageType msgType, string text, int font, int hue)
            : base(ownerEntity.Serial, world)
        {
            _ownerEntity = ownerEntity;
            _text = text;
            _font = font;
            _hue = hue;
            _msgType = msgType;
            _needsRender = true;
        }

        public void RefreshTimer()
        {
            _needsRender = true;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            if (_needsRender)
            {
                _msTimePersist = 5000;
                _needsRender = false;
            }
            else
            {
                _msTimePersist -= gameTime.ElapsedRealTime.Milliseconds;
                if (_msTimePersist <= 0)
                    this.Dispose();
            }
        }

        internal override void Draw(UltimaXNA.TileEngine.MapTile cell, Vector3 position, Vector3 positionOffset)
        {
            string text = Utility.WrapASCIIText(_font, _text, 200);
            cell.Add(new TileEngine.MapObjectText(position, positionOffset, _ownerEntity, text, _hue, _font));
        }
    }
}
