using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class GumpPic : Control
    {
        Texture2D _textureGump = null;
        int _gumpID;

        public GumpPic(Control owner, int page)
            : base(owner, page)
        {

        }

        public GumpPic(Control owner, int page, string[] arguements)
            : this(owner, page)
        {
            int x, y, gumpID, hue = 0;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            gumpID = Int32.Parse(arguements[3]);
            if (arguements.Length > 4)
            {
                // has a HUE="XXX" arguement!
                hue = Int32.Parse(arguements[4]);
            }
            buildGumpling(x, y, gumpID, hue);
        }

        public GumpPic(Control owner, int page, int x, int y, int gumpID, int hue)
            : this(owner, page)
        {
            buildGumpling(x, y, gumpID, hue);
        }

        void buildGumpling(int x, int y, int gumpID, int hue)
        {
            Position = new Vector2(x, y);
            _gumpID = gumpID;
        }

        public override void Update(GameTime gameTime)
        {
            if (_textureGump == null)
            {
                _textureGump = Data.Gumps.GetGumpXNA(_gumpID);
                Size = new Vector2(_textureGump.Width, _textureGump.Height);
            }

            base.Update(gameTime);
        }

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_textureGump, Position, Color.White);
            base.Draw(spriteBatch);
        }
    }
}
