using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;
using UltimaXNA.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    enum ButtonTypes
    {
        Default = 0,
        SwitchPage = 0,
        Activate = 1
    }

    class Button : Control
    {
        Texture2D _gumpUp = null;
        Texture2D _gumpDown = null;
        Texture2D _gumpOver = null;
        Texture2D _texture = null;
        int _gumpID1 = 0, _gumpID2 = 0, _gumpID3 = 0; // 1 == up, 2 == down, 3 == additional over state, not sent by the server but can be added for clientside gumps.
        public int GumpOverID { set { _gumpID3 = value; } }

        public ButtonTypes ButtonType = ButtonTypes.Default;
        public int ButtonParameter = 0;
        public int ButtonID = 0;
        public string Caption = string.Empty;

        internal bool MouseDownOnThis { get { return (_clicked); } }

        TextRenderer _textRenderer;

        public Button(Control owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
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
            buildGumpling(x, y, gumpID1, gumpID2, (ButtonTypes)buttonType, param, buttonID);
        }

        public Button(Control owner, int page, int x, int y, int gumpID1, int gumpID2, ButtonTypes buttonType, int param, int buttonID)
            : this(owner, page)
        {
            buildGumpling(x, y, gumpID1, gumpID2, buttonType, param, buttonID);
        }

        void buildGumpling(int x, int y, int gumpID1, int gumpID2, ButtonTypes buttonType, int param, int buttonID)
        {
            Position = new Point2D(x, y);
            _gumpID1 = gumpID1;
            _gumpID2 = gumpID2;
            ButtonType = buttonType;
            ButtonParameter = param;
            ButtonID = buttonID;
            _textRenderer = new TextRenderer("", 100, true);
        }

        public override void Update(GameTime gameTime)
        {
            if (_gumpUp == null)
            {
                _gumpUp = Data.Gumps.GetGumpXNA(_gumpID1);
                _gumpDown = Data.Gumps.GetGumpXNA(_gumpID2);
                Size = new Point2D(_gumpUp.Width, _gumpUp.Height);
            }

            if (_gumpID3 != 0 && _gumpOver == null)
            {
                _gumpOver = Data.Gumps.GetGumpXNA(_gumpID3);
            }

            if (MouseDownOnThis)
                _texture = _gumpDown;
            else if (_manager.MouseOverControl == this && _gumpOver != null)
                _texture = _gumpOver;
            else
                _texture = _gumpUp;

            if (Caption != "")
                _textRenderer.Text = Caption;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            spriteBatch.Draw2D(_texture, Position, 0, false, false);
            if (Caption != string.Empty)
            {
                int yoffset = MouseDownOnThis ? 1 : 0;
                _textRenderer.Draw(spriteBatch, 
                    new Point2D(X + (Width - _textRenderer.Width) / 2,
                        Area.Y + yoffset + (_texture.Height - _textRenderer.Height) / 2));
            }
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

        bool _clicked = false;

        protected override void mouseDown(int x, int y, MouseButton button)
        {
            _clicked = true;
        }

        protected override void mouseUp(int x, int y, MouseButton button)
        {
            _clicked = false;
        }

        protected override void mouseClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                switch (this.ButtonType)
                {
                    case ButtonTypes.SwitchPage:
                        // switch page
                        ChangePage(this.ButtonParameter);
                        break;
                    case ButtonTypes.Activate:
                        // send response
                        ActivateByButton(this.ButtonID);
                        break;
                }
            }
        }
    }
}
