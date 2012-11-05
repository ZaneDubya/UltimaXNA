/***************************************************************************
 *   croppedText.cs
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
using UltimaXNA.Interface.Graphics;
using UltimaXNA.Interface.GUI;

namespace UltimaXNA.UltimaGUI.Gumplings
{
    class CroppedText : Control
    {
        public int Hue = 0;
        public string Text = string.Empty;
        Interface.GUI.TextRenderer _textRenderer;

        public CroppedText(Control owner, int page)
            : base(owner, page)
        {

        }

        public CroppedText(Control owner, int page, string[] arguements, string[] lines)
            : this(owner, page)
        {
            int x, y, width, height, hue, textIndex;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            hue = Int32.Parse(arguements[5]);
            textIndex = Int32.Parse(arguements[6]);
            buildGumpling(x, y, width, height, hue, textIndex, lines);
        }

        public CroppedText(Control owner, int page, int x, int y, int width, int height, int hue, int textIndex, string[] lines)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height, hue, textIndex, lines);
        }

        void buildGumpling(int x, int y, int width, int height, int hue, int textIndex, string[] lines)
        {
            Position = new Point2D(x, y);
            Size = new Point2D(width, height);
            Hue = hue;
            Text = lines[textIndex];
            _textRenderer = new Interface.GUI.TextRenderer(Text, width, true);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            _textRenderer.Draw(spriteBatch, new Rectangle(X, Y, Size.X, Size.Y), 0, 0);
            base.Draw(spriteBatch);
        }
    }
}
