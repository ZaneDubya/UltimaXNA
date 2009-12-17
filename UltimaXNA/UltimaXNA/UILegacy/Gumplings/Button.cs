using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class Button : Control
    {
        Texture2D _gumpUp = null;
        Texture2D _gumpDown = null;

        public int ButtonType = 0;
        public int ButtonParameter = 0;
        public int ButtonID = 0;

        public Button(Serial serial, Control owner)
            : base(serial, owner)
        {
            HandlesInput = true;
        }

        public Button(Serial serial, Control owner, string[] arguements)
            : this(serial, owner)
        {
            int x, y, gumpID1, gumpID2, buttonType, param, buttonID;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            gumpID1 = Int32.Parse(arguements[3]);
            gumpID2 = Int32.Parse(arguements[4]);
            buttonType = Int32.Parse(arguements[5]);
            param = Int32.Parse(arguements[6]);
            buttonID = Int32.Parse(arguements[7]);
            buildGumpling(x, y, gumpID1, gumpID2, buttonType, param, buttonID);
        }

        public Button(Serial serial, Control owner, int x, int y, int gumpID1, int gumpID2, int buttonType, int param, int buttonID)
            : this(serial, owner)
        {
            buildGumpling(x, y, gumpID1, gumpID2, buttonType, param, buttonID);
        }

        void buildGumpling(int x, int y, int gumpID1, int gumpID2, int buttonType, int param, int buttonID)
        {
            Position = new Vector2(x, y);
            _gumpUp = Data.Gumps.GetGumpXNA(gumpID1);
            _gumpDown = Data.Gumps.GetGumpXNA(gumpID2);
            Size = new Vector2(_gumpUp.Width, _gumpUp.Height);
            ButtonType = buttonType;
            if (ButtonType == 0)
                throw new Exception("What do we do with ButtonType == 0 buttons?");
            ButtonParameter = param;
            ButtonID = buttonID;
        }

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            if (_clicked && _manager.MouseOverControl == this)
            {
                spriteBatch.Draw(_gumpDown, new Vector2(Area.X, Area.Y), Color.White);
            }
            else
            {
                spriteBatch.Draw(_gumpUp, new Vector2(Area.X, Area.Y), Color.White);
            }
            base.Draw(spriteBatch);
        }

        bool _clicked = false;

        public override void _mouseDown(int x, int y, int button)
        {
            _clicked = true;
        }

        public override void _mouseUp(int x, int y, int button)
        {
            _clicked = false;
        }

        public override void _mouseClick(int x, int y, int button)
        {
            if (button == 0)
            {
                _owner.Activate(this);
            }
        }
    }
}
