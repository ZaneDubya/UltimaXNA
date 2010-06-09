using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
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

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_bgGump, new Rectangle(X, Y, Area.Width, Area.Height), new Rectangle(0, 0, _bgGump.Width, _bgGump.Height), 0, false);
            base.Draw(spriteBatch);
        }
    }
}
