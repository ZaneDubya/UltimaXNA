/***************************************************************************
 *   TextLabel.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    public class TextLabel : AControl
    {
        public int Hue = 0;

        private string m_Text;
        public string Text
        {
            get { return m_Text; }
            set
            {
                if (m_textRenderer == null)
                {
                    m_textRenderer = new RenderedText(m_Text, true, 300);
                }
                m_textRenderer.Text = m_Text = value;
            }
        }

        RenderedText m_textRenderer;

        public TextLabel(AControl owner, int page)
            : base(owner, page)
        {

        }

        public TextLabel(AControl owner, int page, string[] arguements, string[] lines)
            : this(owner, page)
        {
            int x, y, hue, textIndex;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            hue = Int32.Parse(arguements[3]);
            textIndex = Int32.Parse(arguements[4]);
            buildGumpling(x, y, hue, lines[textIndex]);
        }

        public TextLabel(AControl owner, int page, int x, int y, int hue, string text)
            : this(owner, page)
        {
            buildGumpling(x, y, hue, text);
        }

        void buildGumpling(int x, int y, int hue, string text)
        {
            Position = new Point(x, y);
            Hue = hue;
            Text = text;
            m_textRenderer.Hue = Hue;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            m_textRenderer.Draw(spriteBatch, Position);
            base.Draw(spriteBatch);
        }
    }
}
