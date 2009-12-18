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

        public Button(Control owner, int page)
            : base(owner, page)
        {
            HandlesInput = true;
        }

        public Button(Control owner, int page, string[] arguements)
            : this(owner, page)
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

        public Button(Control owner, int page, int x, int y, int gumpID1, int gumpID2, int buttonType, int param, int buttonID)
            : this(owner, page)
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
                switch (ButtonType)
                {
                    case 0:
                        // switch page
                        _owner.ChangePage(this);
                        break;
                    case 1:
                        // send response
                        _owner.Activate(this);
                        break;
                }
            }
        }
    }
}
