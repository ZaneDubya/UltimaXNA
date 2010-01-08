using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;

namespace UltimaXNA.UILegacy.Gumplings
{

    class Slider : Control
    {
        Texture2D[] _gumpBar = null;
        Texture2D _gumpSlider = null;

        int _newValue = 0, _value = 0;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = _newValue = value;
                if (IsInitialized)
                    _sliderX = (int)((float)(BarWidth - _gumpSlider.Width) * ((float)(Value - MinValue) / (float)(MaxValue - MinValue)));
            }
        }

        public int MinValue;
        public int MaxValue;
        public int BarWidth;

        int _sliderX;

        public Slider(Control owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
            _pairedSliders = new List<Slider>();
        }

        public Slider(Control owner, int page, int x, int y, int width, int minValue, int maxValue, int value)
            : this(owner, page)
        {
            buildGumpling(x, y, width, minValue, maxValue, value);
        }

        void buildGumpling(int x, int y, int width, int minValue, int maxValue, int value)
        {
            Position = new Vector2(x, y);
            MinValue = minValue;
            MaxValue = maxValue;
            BarWidth = width;
            Value = value; // must set this last
        }

        public override void Update(GameTime gameTime)
        {
            if (_gumpSlider == null)
            {
                _gumpBar = new Texture2D[3];
                _gumpBar[0] = Data.Gumps.GetGumpXNA(213);
                _gumpBar[1] = Data.Gumps.GetGumpXNA(214);
                _gumpBar[2] = Data.Gumps.GetGumpXNA(215);
                _gumpSlider = Data.Gumps.GetGumpXNA(216);
                Size = new Vector2(BarWidth, _gumpSlider.Height);
                _sliderX = (int)((float)(BarWidth - _gumpSlider.Width) * ((float)(Value - MinValue) / (float)(MaxValue - MinValue)));
            }
            
            modifyPairedValues(_newValue - Value);
            _value = _newValue;


            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_gumpBar[0], new Vector2(Area.X, Area.Y), 0, false);
            spriteBatch.DrawTiled(_gumpBar[1], new Rectangle(Area.X + _gumpBar[0].Width, Area.Y, BarWidth - _gumpBar[2].Width - _gumpBar[0].Width, _gumpBar[1].Height), 0, false);
            spriteBatch.Draw(_gumpBar[2], new Vector2(Area.X + BarWidth - _gumpBar[2].Width, Area.Y), 0, false);
            spriteBatch.Draw(_gumpSlider, new Vector2(Area.X + _sliderX, Area.Y), 0, false);
            base.Draw(spriteBatch);
        }

        protected override bool _hitTest(int x, int y)
        {
            if (new Rectangle(_sliderX, 0, _gumpSlider.Width, _gumpSlider.Height).Contains(new Point(x, y)))
                return true;
            else
                return false;
        }

        bool _clicked = false;
        Point _clickPosition;

        protected override void _mouseDown(int x, int y, MouseButtons button)
        {
            _clicked = true;
            _clickPosition = new Point(x, y);
        }

        protected override void _mouseUp(int x, int y, MouseButtons button)
        {
            _clicked = false;
        }

        protected override void _mouseOver(int x, int y)
        {
            if (_clicked)
            {
                _sliderX = _sliderX + (x - _clickPosition.X);
                if (_sliderX < 0)
                    _sliderX = 0;
                if (_sliderX > BarWidth - _gumpSlider.Width)
                    _sliderX = BarWidth - _gumpSlider.Width;
                _clickPosition = new Point(x, y);
                if (_clickPosition.X < _gumpSlider.Width / 2)
                    _clickPosition.X = _gumpSlider.Width / 2;
                if (_clickPosition.X > BarWidth - _gumpSlider.Width / 2)
                    _clickPosition.X = BarWidth - _gumpSlider.Width / 2;
                _newValue = (int)(((float)_sliderX / (float)(BarWidth - _gumpSlider.Width)) * (float)((MaxValue - MinValue))) + MinValue;
            }
        }

        List<Slider> _pairedSliders;
        public void PairSlider(Slider s)
        {
            _pairedSliders.Add(s);
        }

        void modifyPairedValues(int delta)
        {
            List<Slider> sliders = new List<Slider>();
            int d;
            if (delta > 0)
            {
                d = -1;
                foreach (Slider s in _pairedSliders)
                    if (s.Value > s.MinValue)
                        sliders.Add(s);
            }
            else
            {
                d = 1;
                foreach (Slider s in _pairedSliders)
                    if (s.Value < s.MaxValue)
                        sliders.Add(s);
            }

            if (sliders.Count == 0)
                return;

            int j = Value % sliders.Count;
            for (int i = Math.Abs(delta); i > 0; i--)
            {
                sliders[j].Value += d;
                j++;
                if (j == sliders.Count)
                    j = 0;
            }
        }
    }
}
