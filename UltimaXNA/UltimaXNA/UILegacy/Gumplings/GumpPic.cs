using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class GumpPic : Control
    {
        protected Texture2D _texture = null;
        int _gumpID;
        int _hue;

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
            Position = new Point2D(x, y);
            _gumpID = gumpID;
            _hue = hue;
        }

        public override void Update(GameTime gameTime)
        {
            if (_texture == null)
            {
                _texture = Data.Gumps.GetGumpXNA(_gumpID);
                Size = new Point2D(_texture.Width, _texture.Height);
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            bool hueOnlyGreyPixels = (_hue & 0x8000) == 0x8000;
            spriteBatch.Draw2D(_texture, Position, _hue & 0x7FFF, hueOnlyGreyPixels, false);
            base.Draw(spriteBatch);
        }

        protected override bool _hitTest(int x, int y)
        {
            Color[] pixelData;
            pixelData = new Color[1];
            _texture.GetData<Color>(0, new Rectangle(x, y, 1, 1), pixelData, 0, 1);
            if (pixelData[0].A > 0)
                return true;
            else
                return false;
        }
    }
}
