/***************************************************************************
 *   Overhead.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
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

        public Overhead(Entity ownerEntity, IIsometricRenderer world, MessageType msgType, string text, int font, int hue)
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
                _msTimePersist -= gameTime.ElapsedGameTime.Milliseconds;
                if (_msTimePersist <= 0)
                    this.Dispose();
            }
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            // string text = Utility.WrapASCIIText(_font, _text, 200);
            // tile.Add(new TileEngine.MapObjectText(position, _ownerEntity, text, _hue, _font));
        }
    }
}
