using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;
using UltimaXNA.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class ExpandableScroll : Gump
    {
        GumpPic _gumplingTop, _gumplingBottom;
        GumpPicTiled _gumplingMiddle;
        Button _gumplingExpander;
        
        int _expandableScrollHeight;
        const int _expandableScrollHeight_Min = 274; // this is the min from the client.
        const int _expandableScrollHeight_Max = 1000; // arbitrary large number.

        int gumplingMidY { get { return _gumplingTop.Height; } }
        int gumplingMidHeight { get { return _expandableScrollHeight - _gumplingTop.Height - _gumplingBottom.Height - _gumplingExpander.Height; } }
        int gumplingBottomY { get { return _expandableScrollHeight - _gumplingBottom.Height - _gumplingExpander.Height; } }
        int gumplingExpanderX { get { return (Width - _gumplingExpander.Width) / 2; } }
        int gumplingExpanderY { get { return _expandableScrollHeight - _gumplingExpander.Height - gumplingExpanderY_Offset; } }
        const int gumplingExpanderY_Offset = 2; // this is the gap between the pixels of the btm gumpling texture and the height of the btm gumpling texture.
        const int gumplingExpander_ButtonID = 0x7FBEEF;

        bool _isExpanding = false;
        int _isExpanding_InitialX, _isExpanding_InitialY, _isExpanding_InitialHeight;

        public ExpandableScroll(Control owner, int page, int x, int y, int height)
            : base(0, 0)
        {
            _owner = owner;
            Position = new Point2D(x, y);
            _expandableScrollHeight = height;
        }

        public override void Initialize(UIManager manager)
        {
            base.Initialize(manager);
            _gumplingTop = (GumpPic)AddGumpling(new GumpPic(this, 0, 0, 0, 0x0820, 0));
            _gumplingMiddle = (GumpPicTiled)AddGumpling(new GumpPicTiled(this, 0, 0, 0, 0, 0, 0x0822));
            _gumplingBottom = (GumpPic)AddGumpling(new GumpPic(this, 0, 0, 0, 0x0823, 0));
            _gumplingExpander = (Button)AddGumpling(new Button(this, 0, 0, 0, 0x082E, 0x82F, ButtonTypes.Activate, 0, gumplingExpander_ButtonID));
            
            _gumplingExpander.OnMouseDown = expander_OnMouseDown;
            _gumplingExpander.OnMouseUp = expander_OnMouseUp;
            _gumplingExpander.OnMouseOver = expander_OnMouseOver;
        }

        protected override bool _hitTest(int x, int y)
        {
            Point2D position = new Point2D(x + OwnerX + X, y + OwnerY + Y);
            if (_gumplingTop.HitTest(position, true) != null)
                return true;
            if (_gumplingMiddle.HitTest(position, true) != null)
                return true;
            if (_gumplingBottom.HitTest(position, true) != null)
                return true;
            if (_gumplingExpander.HitTest(position, true) != null)
                return true;
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            if (_expandableScrollHeight < _expandableScrollHeight_Min)
                _expandableScrollHeight = _expandableScrollHeight_Min;
            if (_expandableScrollHeight > _expandableScrollHeight_Max)
                _expandableScrollHeight = _expandableScrollHeight_Max;

            if (_gumplingTitleGumpIDDelta)
            {
                _gumplingTitleGumpIDDelta = false;
                if (_gumplingTitle != null)
                    _gumplingTitle.Dispose();
                _gumplingTitle = (GumpPic)AddGumpling(new GumpPic(this, 0, 0, 0, _gumplingTitleGumpID, 0));
            }

            if (!_gumplingTop.IsInitialized)
            {
                Visible = false;
            }
            else 
            {
                Visible = true;
                _gumplingTop.X = 0;
                _gumplingTop.Y = 0;

                _gumplingMiddle.X = 17;
                _gumplingMiddle.Y = gumplingMidY;
                _gumplingMiddle.Width = 263;
                _gumplingMiddle.Height = gumplingMidHeight;

                _gumplingBottom.X = 17;
                _gumplingBottom.Y = gumplingBottomY;

                _gumplingExpander.X = gumplingExpanderX;
                _gumplingExpander.Y = gumplingExpanderY;

                if (_gumplingTitle != null && _gumplingTitle.IsInitialized)
                {
                    _gumplingTitle.X = (_gumplingTop.Width - _gumplingTitle.Width) / 2;
                    _gumplingTitle.Y = (_gumplingTop.Height - _gumplingTitle.Height) / 2;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void ActivateByButton(int buttonID)
        {
            // this is necessary to override the default behavior for buttons, which is to send a msg to the server.
        }

        void expander_OnMouseDown(int x, int y, MouseButton button)
        {
            y += _gumplingExpander.Y + OwnerY;
            if (button == MouseButton.Left)
            {
                _isExpanding = true;
                _isExpanding_InitialHeight = _expandableScrollHeight;
                _isExpanding_InitialX = x;
                _isExpanding_InitialY = y;
            }
        }

        void expander_OnMouseUp(int x, int y, MouseButton button)
        {
            y += _gumplingExpander.Y + OwnerY;
            if (_isExpanding)
            {
                _isExpanding = false;
                _expandableScrollHeight = _isExpanding_InitialHeight + (y - _isExpanding_InitialY);
            }
        }

        void expander_OnMouseOver(int x, int y)
        {
            y += _gumplingExpander.Y + OwnerY;
            if (_isExpanding && (y != _isExpanding_InitialY))
            {
                _expandableScrollHeight = _isExpanding_InitialHeight + (y - _isExpanding_InitialY);
            }
        }

        bool _gumplingTitleGumpIDDelta = false;
        int _gumplingTitleGumpID;
        GumpPic _gumplingTitle;
        public int TitleGumpID { set { _gumplingTitleGumpID = value; _gumplingTitleGumpIDDelta = true; } }
    }
}
