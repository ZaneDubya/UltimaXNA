/***************************************************************************
 *   TextLabelAscii.cs
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
using UltimaXNA.Rendering;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaData.Fonts;

namespace UltimaXNA.UltimaGUI.Controls
{
    class TextLabelAscii : Control
    {
        public int Hue = 0;
        public int FontID = 0;
        public string Text = string.Empty;
        Texture2D m_texture = null;

        public TextLabelAscii(Control owner, int page)
            : base(owner, page)
        {

        }

        public TextLabelAscii(Control owner, int page, int x, int y, int hue, int fontid, string text)
            : this(owner, page)
        {
            buildGumpling(x, y, hue, fontid, text);
        }

        void buildGumpling(int x, int y, int hue, int fontid, string text)
        {
            Position = new Point(x, y);
            Hue = hue;
            FontID = fontid;
            Text = text;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            m_texture = ASCIIText.GetTextTexture(Text, FontID);
            spriteBatch.Draw2D(m_texture, Position, Hue, true, false);
            base.Draw(spriteBatch);
        }
    }
}
