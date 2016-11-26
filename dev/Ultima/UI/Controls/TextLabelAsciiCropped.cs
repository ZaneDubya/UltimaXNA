/***************************************************************************
 *   TextLabelAsciiCropped.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    class TextLabelAsciiCropped : AControl
    {
        public int Hue;
        public int FontID;

        private RenderedText m_Rendered;
        private string m_Text;

        public string Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value;
                m_Rendered.Text = string.Format("<span style=\"font-family=ascii{0}\">{1}", FontID, m_Text);
            }
        }

        TextLabelAsciiCropped(AControl parent)
            : base(parent)
        {

        }

        public TextLabelAsciiCropped(AControl parent, int x, int y, int width, int height, int font, int hue, string text)
            : this(parent)
        {
            BuildGumpling(x, y, width, height, font, hue, text);
        }

        void BuildGumpling(int x, int y, int width, int height, int font, int hue, string text)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            m_Rendered = new RenderedText(string.Empty, width);
            Hue = hue;
            FontID = font;
            Text = text;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            m_Rendered.Draw(spriteBatch, position, Utility.GetHueVector(Hue, true, false, true));
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
