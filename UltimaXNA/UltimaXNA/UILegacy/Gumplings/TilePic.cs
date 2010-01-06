using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class TilePic : Control
    {
        Texture2D _texture = null;
        int Hue;
        int _tileID;

        public TilePic(Control owner, int page)
            : base(owner, page)
        {

        }

        public TilePic(Control owner, int page, string[] arguements)
            : this(owner, page)
        {
            int x, y, tileID, hue = 0;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            tileID = Int32.Parse(arguements[3]);
            if (arguements.Length > 4)
            {
                // has a HUE="XXX" arguement!
                hue = Int32.Parse(arguements[4]);
            }
            buildGumpling(x, y, tileID, hue);
        }

        public TilePic(Control owner, int page, int x, int y, int tileID, int hue)
            : this(owner, page)
        {
            buildGumpling(x, y, tileID, hue);
        }

        void buildGumpling(int x, int y, int tileID, int hue)
        {
            Position = new Vector2(x, y);
            Hue = hue;
            _tileID = tileID;
        }

        public override void Update(GameTime gameTime)
        {
            if (_texture == null)
            {
                _texture = Data.Art.GetStaticTexture(_tileID);
                Size = new Vector2(_texture.Width, _texture.Height);
            }
            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
            base.Draw(spriteBatch);
        }
    }
}
