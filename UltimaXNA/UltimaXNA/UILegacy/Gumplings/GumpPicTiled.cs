using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class GumpPicTiled : Control
    {
        Texture2D _bgGump = null;

        public GumpPicTiled(Serial serial, Control owner)
            : base(serial, owner)
        {

        }

        public GumpPicTiled(Serial serial, Control owner, string[] arguements)
            : this(serial, owner)
        {
            int x, y, gumpID, width, height;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            gumpID = Int32.Parse(arguements[5]);
            buildGumpling(x, y, width, height, gumpID);
        }

        public GumpPicTiled(Serial serial, Control owner, int x, int y, int width, int height, int gumpID)
            : this(serial, owner)
        {
            buildGumpling(x, y, width, height, gumpID);
        }

        void buildGumpling(int x, int y, int width, int height, int gumpID)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
            _bgGump = Data.Gumps.GetGumpXNA(gumpID);
        }

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_bgGump, new Rectangle(Area.X, Area.Y, Area.Width, Area.Height), new Rectangle(0, 0, Area.Width, Area.Height), Color.White);
            base.Draw(spriteBatch);
        }
    }
}
