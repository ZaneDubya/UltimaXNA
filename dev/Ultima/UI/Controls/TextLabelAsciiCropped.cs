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
        public int Hue = 0;
        public int FontID = 0;

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

        public TextLabelAsciiCropped(AControl parent)
            : base(parent)
        {

        }

        public TextLabelAsciiCropped(AControl parent, int x, int y, int width, int height, int hue, int fontid, string text)
            : this(parent)
        {
            buildGumpling(x, y, width, height, hue, fontid, text);
        }

        void buildGumpling(int x, int y, int width, int height, int hue, int fontid, string text)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            m_Rendered = new RenderedText(string.Empty, width);
            Hue = hue;
            FontID = fontid;
            Text = text;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            m_Rendered.Draw(spriteBatch, position, Utility.GetHueVector(Hue, true, false));
            base.Draw(spriteBatch, position);
        }
    }
}
