using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class GumpPic : Control
    {
        Texture2D _textureGump = null;

        public GumpPic(Serial serial, Control owner)
            : base(serial, owner)
        {

        }

        public GumpPic(Serial serial, Control owner, string[] arguements)
            : this(serial, owner)
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

        public GumpPic(Serial serial, Control owner, int x, int y, int gumpID, int hue)
            : this(serial, owner)
        {
            buildGumpling(x, y, gumpID, hue);
        }

        void buildGumpling(int x, int y, int gumpID, int hue)
        {
            Position = new Vector2(x, y);
            _textureGump = Data.Gumps.GetGumpXNA(gumpID);
            Size = new Vector2(_textureGump.Width, _textureGump.Height);
            
        }

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_textureGump, Position, Color.White);
            base.Draw(spriteBatch);
        }
    }
}
