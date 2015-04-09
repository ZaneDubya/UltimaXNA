/***************************************************************************
 *   TextLabelAsciiCropped.cs
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
using UltimaXNA.Ultima.IO.FontsOld;

namespace UltimaXNA.Ultima.UI.Controls
{
    class TextLabelAsciiCropped : AControl
    {
        public int Hue = 0;
        public int FontID = 0;
        public string Text = string.Empty;
        Texture2D m_texture = null;

        public TextLabelAsciiCropped(AControl owner, int page)
            : base(owner, page)
        {

        }

        public TextLabelAsciiCropped(AControl owner, int page, int x, int y, int width, int height, int hue, int fontid, string text)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height, hue, fontid, text);
        }

        void buildGumpling(int x, int y, int width, int height, int hue, int fontid, string text)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            Hue = hue;
            FontID = fontid;
            Text = text;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            if (m_texture == null)
                m_texture = ASCIIText.GetTextTexture(Text, FontID, Area.Width);
            spriteBatch.Draw2D(m_texture, Position, Hue, true, false);
            base.Draw(spriteBatch);
        }
    }
}
