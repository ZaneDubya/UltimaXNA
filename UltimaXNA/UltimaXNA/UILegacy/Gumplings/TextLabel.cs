/***************************************************************************
 *   TextLabel.cs
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

namespace UltimaXNA.UILegacy.Gumplings
{
    public class TextLabel : Control
    {
        public int Hue = 0;
        public string Text = string.Empty;
        TextRenderer _textRenderer;

        public TextLabel(Control owner, int page)
            : base(owner, page)
        {

        }

        public TextLabel(Control owner, int page, string[] arguements, string[] lines)
            : this(owner, page)
        {
            int x, y, hue, textIndex;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            hue = Int32.Parse(arguements[3]);
            textIndex = Int32.Parse(arguements[4]);
            buildGumpling(x, y, hue, lines[textIndex]);
        }

        public TextLabel(Control owner, int page, int x, int y, int hue, string text)
            : this(owner, page)
        {
            buildGumpling(x, y, hue, text);
        }

        void buildGumpling(int x, int y, int hue, string text)
        {
            Position = new Point2D(x, y);
            Hue = hue;
            Text = text;
            _textRenderer = new TextRenderer(Text, 0, true);
            _textRenderer.Hue = Hue;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            _textRenderer.Draw(spriteBatch, Position);
            base.Draw(spriteBatch);
        }
    }
}
