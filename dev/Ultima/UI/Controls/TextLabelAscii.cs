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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.IO.FontsOld;

namespace UltimaXNA.Ultima.UI.Controls
{
    class TextLabelAscii : AControl
    {
        public int Hue = 0;
        public int FontID = 0;
        public string Text = string.Empty;
        Texture2D m_texture = null;

        public TextLabelAscii(AControl owner, int page)
            : base(owner, page)
        {

        }

        public TextLabelAscii(AControl owner, int page, int x, int y, int hue, int fontid, string text)
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

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            m_texture = ASCIIText.GetTextTexture(Text, FontID);
            spriteBatch.Draw2D(m_texture, new Vector3(position.X, position.Y, 0), Utility.GetHueVector(Hue, true, false));
            base.Draw(spriteBatch, position);
        }
    }
}
