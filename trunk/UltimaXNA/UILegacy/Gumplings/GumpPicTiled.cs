﻿/***************************************************************************
 *   GumpPicTiled.cs
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
using UltimaXNA.Interface.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class GumpPicTiled : Control
    {
        Texture2D _bgGump = null;
        int _gumpID;

        public GumpPicTiled(Control owner, int page)
            : base(owner, page)
        {

        }

        public GumpPicTiled(Control owner, int page, string[] arguements)
            : this(owner, page)
        {
            int x, y, gumpID, width, height;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            gumpID = Int32.Parse(arguements[5]);
            buildGumpling(x, y, width, height, gumpID);
        }

        public GumpPicTiled(Control owner, int page, int x, int y, int width, int height, int gumpID)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height, gumpID);
        }

        void buildGumpling(int x, int y, int width, int height, int gumpID)
        {
            Position = new Point2D(x, y);
            Size = new Point2D(width, height);
            _gumpID = gumpID;
        }

        public override void Update(GameTime gameTime)
        {
            if (_bgGump == null)
            {
                _bgGump = Data.Gumps.GetGumpXNA(_gumpID);
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            spriteBatch.Draw2DTiled(_bgGump, new Rectangle(X, Y, Area.Width, Area.Height), 0, false, false);
            base.Draw(spriteBatch);
        }
    }
}
