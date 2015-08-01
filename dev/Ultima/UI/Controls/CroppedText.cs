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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    class CroppedText : AControl
    {
        public int Hue = 0;
        public string Text = string.Empty;
        RenderedText m_Texture;

        public CroppedText(AControl parent)
            : base(parent)
        {

        }

        public CroppedText(AControl parent, string[] arguements, string[] lines)
            : this(parent)
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

        public CroppedText(AControl parent, int x, int y, int width, int height, int hue, int textIndex, string[] lines)
            : this(parent)
        {
            buildGumpling(x, y, width, height, hue, textIndex, lines);
        }

        void buildGumpling(int x, int y, int width, int height, int hue, int textIndex, string[] lines)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            Hue = hue;
            Text = lines[textIndex];
            m_Texture = new RenderedText(Text, width);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            m_Texture.Draw(spriteBatch, new Rectangle(position.X, position.Y, Width, Height), 0, 0);
            base.Draw(spriteBatch, position);
        }
    }
}
