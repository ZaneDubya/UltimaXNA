/***************************************************************************
 *   Slider.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Graphics;
using UltimaXNA.Input;
using UltimaXNA.GUI;

namespace UltimaXNA.UltimaGUI.Controls
{

    class Slider : Control
    {
        Texture2D[] _gumpBar = null;
        Texture2D _gumpSlider = null;

        // we use _newValue to (a) get delta, (b) so Value only changes once per frame.
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
            Position = new Point2D(x, y);
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
                _gumpBar[0] = UltimaData.Gumps.GetGumpXNA(213);
                _gumpBar[1] = UltimaData.Gumps.GetGumpXNA(214);
                _gumpBar[2] = UltimaData.Gumps.GetGumpXNA(215);
                _gumpSlider = UltimaData.Gumps.GetGumpXNA(216);
                Size = new Point2D(BarWidth, _gumpSlider.Height);
                _sliderX = (int)((float)(BarWidth - _gumpSlider.Width) * ((float)(Value - MinValue) / (float)(MaxValue - MinValue)));
            }
            
            modifyPairedValues(_newValue - Value);
            _value = _newValue;


            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            spriteBatch.Draw2D(_gumpBar[0], Position, 0, false, false);
            spriteBatch.Draw2DTiled(_gumpBar[1], new Rectangle(Area.X + _gumpBar[0].Width, Area.Y, BarWidth - _gumpBar[2].Width - _gumpBar[0].Width, _gumpBar[1].Height), 0, false, false);
            spriteBatch.Draw2D(_gumpBar[2], new Point2D(Area.X + BarWidth - _gumpBar[2].Width, Area.Y), 0, false, false);
            spriteBatch.Draw2D(_gumpSlider, new Point2D(Area.X + _sliderX, Area.Y), 0, false, false);
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

        protected override void mouseDown(int x, int y, MouseButton button)
        {
            _clicked = true;
            _clickPosition = new Point(x, y);
        }

        protected override void mouseUp(int x, int y, MouseButton button)
        {
            _clicked = false;
        }

        protected override void mouseOver(int x, int y)
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
            bool updateSinceLastCycle = true;
            int d = (delta > 0) ? -1 : 1;
            int points = Math.Abs(delta);
            int sliderIndex = Value % _pairedSliders.Count;
            while (points > 0)
            {
                if (d > 0)
                {
                    if (_pairedSliders[sliderIndex].Value < _pairedSliders[sliderIndex].MaxValue)
                    {
                        updateSinceLastCycle = true;
                        _pairedSliders[sliderIndex].Value += d;
                        points--;
                    }
                }
                else
                {
                    if (_pairedSliders[sliderIndex].Value > _pairedSliders[sliderIndex].MinValue)
                    {
                        updateSinceLastCycle = true;
                        _pairedSliders[sliderIndex].Value += d;
                        points--;
                    }
                }
                sliderIndex++;
                if (sliderIndex == _pairedSliders.Count)
                {
                    if (!updateSinceLastCycle)
                        return;
                    updateSinceLastCycle = false;
                    sliderIndex = 0;
                }
            }
        }
    }
}
