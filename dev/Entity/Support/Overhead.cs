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
using UltimaXNA.UltimaWorld;

namespace UltimaXNA.Entity
{
    public class Overhead : BaseEntity
    {
        private bool m_needsRender;
        private BaseEntity m_ownerEntity;

        private string m_text;
        public string Text
        {
            get { return m_text; }
            set
            {
                m_text = value;
                m_needsRender = true;
            }
        }

        private MessageType m_msgType;
        public MessageType MsgType
        {
            get { return m_msgType; }
            set
            {
                m_msgType = value;
            }
        }

        private string m_speakerName;
        public string SpeakerName
        {
            get { return m_speakerName; }
            set
            {
                m_speakerName = value;
                m_needsRender = true;
            }
        }

        private int m_hue;
        public int Hue
        {
            get { return m_hue; }
            set
            {
                m_hue = value;
                m_needsRender = true;
            }
        }

        private int m_font;
        public int Font
        {
            get { return m_font; }
            set
            {
                m_font = value;
                m_needsRender = true;
            }
        }

        private int m_msTimePersist = 0;

        public Overhead(BaseEntity ownerEntity, MessageType msgType, string text, int font, int hue)
            : base(ownerEntity.Serial)
        {
            m_ownerEntity = ownerEntity;
            m_text = text;
            m_font = font;
            m_hue = hue;
            m_msgType = msgType;
            m_needsRender = true;
        }

        public void RefreshTimer()
        {
            m_needsRender = true;
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            if (m_needsRender)
            {
                m_msTimePersist = 5000;
                m_needsRender = false;
            }
            else
            {
                m_msTimePersist -= (int)frameMS;
                if (m_msTimePersist <= 0)
                    this.Dispose();
            }
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            // string text = Utility.WrapASCIIText(m_font, m_text, 200);
            // tile.Add(new TileEngine.MapObjectText(position, m_ownerEntity, text, m_hue, m_font));
        }
    }
}
