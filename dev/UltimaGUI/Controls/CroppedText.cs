/***************************************************************************
 *   croppedText.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Rendering;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class CroppedText : Control
    {
        public int Hue = 0;
        public string Text = string.Empty;
        RenderedText m_Texture;

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
            Position = new Point(x, y);
            Size = new Point(width, height);
            Hue = hue;
            Text = lines[textIndex];
            m_Texture = new RenderedText(Text, true, width);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            m_Texture.Draw(spriteBatch, new Rectangle(X, Y, Size.X, Size.Y), 0, 0);
            base.Draw(spriteBatch);
        }
    }
}
