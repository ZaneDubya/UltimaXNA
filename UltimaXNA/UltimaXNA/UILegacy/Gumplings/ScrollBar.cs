using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;
using UltimaXNA.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class ScrollBar : Control
    {
        Texture2D[] _gumpUpButton = null;
        Texture2D[] _gumpDownButton = null;
        Texture2D _gumpSlider = null;
        Texture2D[] _gumpBackground = null;

        float _sliderY;
        float _value = 0;
        public int Value
        {
            get
            {
                return (int)_value;
            }
            set
            {
                _value = value;
                if (_value < MinValue)
                    _value = MinValue;
                if (_value > MaxValue)
                    _value = MaxValue;
                _sliderY = calculateSliderY();
            }
        }

        public override int Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = BarHeight = value;
                _sliderY = calculateSliderY();
            }
        }

        float calculateSliderY()
        {
            if (!IsInitialized)
                return 0f;
            if (MaxValue - MinValue == 0)
                return 0f;
            return scrollableArea() * ((_value - MinValue) / (MaxValue - MinValue));
        }

        float scrollableArea()
        {
            if (!IsInitialized)
                return 0f;
            return BarHeight - _gumpUpButton[0].Height - _gumpDownButton[0].Height - _gumpSlider.Height - 2;
        }

        public int MinValue;
        private int _maxValue;
        public int MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
                if (_value > _maxValue)
                    _value = _maxValue;
            }
        }
        public int BarHeight;

        public ScrollBar(Control owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
        }

        public ScrollBar(Control owner, int page, int x, int y, int height, int minValue, int maxValue, int value)
            : this(owner, page)
        {
            buildGumpling(x, y, height, minValue, maxValue, value);
        }

        void buildGumpling(int x, int y, int height, int minValue, int maxValue, int value)
        {
            Position = new Point2D(x, y);
            MinValue = minValue;
            MaxValue = maxValue;
            Height = height;
            Value = value;
        }

        public override void Update(GameTime gameTime)
        {
            if (_gumpSlider == null)
            {
                _gumpUpButton = new Texture2D[2];
                _gumpUpButton[0] = Data.Gumps.GetGumpXNA(251);
                _gumpUpButton[1] = Data.Gumps.GetGumpXNA(250);
                _gumpDownButton = new Texture2D[2];
                _gumpDownButton[0] = Data.Gumps.GetGumpXNA(253);
                _gumpDownButton[1] = Data.Gumps.GetGumpXNA(252);
                _gumpBackground = new Texture2D[3];
                _gumpBackground[0] = Data.Gumps.GetGumpXNA(257);
                _gumpBackground[1] = Data.Gumps.GetGumpXNA(256);
                _gumpBackground[2] = Data.Gumps.GetGumpXNA(255);
                _gumpSlider = Data.Gumps.GetGumpXNA(254);
                Size = new Point2D(_gumpBackground[0].Width, BarHeight);
            }

            if (_btnUpClicked || _btnDownClicked)
            {
                if (_timeUntilNextClick <= 0f)
                {
                    _timeUntilNextClick = _timeBetweenClicks;
                    if (_btnUpClicked)
                        _value -= 1;
                    if (_btnDownClicked)
                        _value += 1;
                }
                _timeUntilNextClick -= (float)gameTime.ElapsedGameTime.TotalSeconds / 1000f;
            }
            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            // up button
            spriteBatch.Draw2D(_btnUpClicked ? _gumpUpButton[1] : _gumpUpButton[0], new Point2D(X, Y), 0, false, false);
            // scrollbar background
            spriteBatch.Draw2D(_gumpBackground[0], new Point2D(X, Y + _gumpUpButton[0].Height), 0, false, false);
            int middlewidth = BarHeight - _gumpUpButton[0].Height - _gumpDownButton[0].Height - _gumpBackground[0].Height - _gumpBackground[2].Height;
            spriteBatch.Draw2DTiled(_gumpBackground[1], new Rectangle(X, Y + _gumpUpButton[0].Height + _gumpBackground[0].Height, _gumpBackground[0].Width, middlewidth), 0, false, false);
            spriteBatch.Draw2D(_gumpBackground[2], new Point2D(X, Y + BarHeight - _gumpDownButton[0].Height - _gumpBackground[2].Height), 0, false, false);
            // down button
            spriteBatch.Draw2D(_btnDownClicked ? _gumpDownButton[1] : _gumpDownButton[0], new Point2D(X, Y + Height - _gumpDownButton[0].Height), 0, false, false);
            // slider
            spriteBatch.Draw2D(_gumpSlider, new Point2D(X + (_gumpBackground[0].Width - _gumpSlider.Width) / 2, Y + _gumpUpButton[0].Height + 1 + (int)_sliderY), 0, false, false);
            base.Draw(spriteBatch);
        }

        protected override bool _hitTest(int x, int y)
        {
            if (new Rectangle(0, 0, Width, Height).Contains(new Point(x, y)))
                return true;
            else
                return false;
        }

        bool _btnUpClicked = false;
        bool _btnDownClicked = false;
        bool _btnSliderClicked = false;
        Point _clickPosition;

        float _timeUntilNextClick;
        const float _timeBetweenClicks = 0.5f;

        protected override void mouseDown(int x, int y, MouseButton button)
        {
            _timeUntilNextClick = 0f;
            if (new Rectangle(0, 0, _gumpUpButton[0].Width, _gumpUpButton[0].Height).Contains(new Point(x, y)))
                _btnUpClicked = true;
            else if (new Rectangle(0, Height - _gumpDownButton[0].Height, _gumpDownButton[0].Width, _gumpDownButton[0].Height).Contains(new Point(x, y)))
                _btnDownClicked = true;
            else if (new Rectangle((_gumpBackground[0].Width - _gumpSlider.Width) / 2, _gumpUpButton[0].Height + (int)_sliderY, _gumpSlider.Width, _gumpSlider.Height).Contains(new Point(x, y)))
            {
                _btnSliderClicked = true;
                _clickPosition = new Point(x, y);
            }
            else
            {
                // clicked on the bar. This should scroll up a full slider's height worth of entries.
                // not coded yet, obviously.
            }
        }

        protected override void mouseUp(int x, int y, MouseButton button)
        {
            _btnUpClicked = false;
            _btnDownClicked = false;
            _btnSliderClicked = false;
        }

        protected override void mouseOver(int x, int y)
        {
            if (_btnSliderClicked)
            {
                if (y != _clickPosition.Y)
                {
                    float sliderY = _sliderY + (y - _clickPosition.Y);
                    if (sliderY < 0)
                        sliderY = 0;
                    if (sliderY > scrollableArea())
                        sliderY = scrollableArea();
                    _clickPosition = new Point(x, y);
                    if (sliderY == 0 && _clickPosition.Y < _gumpUpButton[0].Height + _gumpSlider.Height / 2)
                        _clickPosition.Y = _gumpUpButton[0].Height + _gumpSlider.Height / 2;
                    if (sliderY == (scrollableArea()) && _clickPosition.Y > BarHeight - _gumpDownButton[0].Height - _gumpSlider.Height / 2)
                        _clickPosition.Y = BarHeight - _gumpDownButton[0].Height - _gumpSlider.Height / 2;
                    _value = ((sliderY / scrollableArea()) * (float)((MaxValue - MinValue))) + MinValue;
                    _sliderY = sliderY;
                }
            }
        }
    }
}
