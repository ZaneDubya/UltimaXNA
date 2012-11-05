/***************************************************************************
 *   TextLabelAsciiCropped.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.Graphics;
using UltimaXNA.GUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class TextLabelAsciiCropped : Control
    {
        public int Hue = 0;
        public int FontID = 0;
        public string Text = string.Empty;
        Texture2D _texture = null;

        public TextLabelAsciiCropped(Control owner, int page)
            : base(owner, page)
        {

        }

        public TextLabelAsciiCropped(Control owner, int page, int x, int y, int width, int height, int hue, int fontid, string text)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height, hue, fontid, text);
        }

        void buildGumpling(int x, int y, int width, int height, int hue, int fontid, string text)
        {
            Position = new Point2D(x, y);
            Size = new Point2D(width, height);
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
            if (_texture == null)
                _texture = UltimaData.ASCIIText.GetTextTexture(Text, FontID, Area.Width);
            spriteBatch.Draw2D(_texture, Position, Hue, true, false);
            base.Draw(spriteBatch);
        }
    }
}
