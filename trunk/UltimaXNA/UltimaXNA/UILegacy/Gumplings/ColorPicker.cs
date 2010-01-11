﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class ColorPicker : Control
    {
        protected Texture2D _huesTexture;
        protected Texture2D _selectedIndicator;
        protected Rectangle _openArea;

        protected Point _hueSize;
        protected int[] _hues;

        protected ColorPicker _openColorPicker;

        bool _getNewSelectedTexture;
        int _index = 0;
        public int Index
        {
            get { return _index; }
            set { _index = value; _getNewSelectedTexture = true; }
        }

        public ColorPicker(Control owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
        }

        public ColorPicker(Control owner, int page, Rectangle area, int swatchWidth, int swatchHeight, int[] hues)
            : this(owner, page)
        {
            _isAnOpenSwatch = true;
            buildGumpling(area, swatchWidth, swatchHeight, hues);
        }

        public ColorPicker(Control owner, int page, Rectangle closedArea, Rectangle openArea, int swatchWidth, int swatchHeight, int[] hues)
            : this(owner, page)
        {
            _isAnOpenSwatch = false;
            _openArea = openArea;
            buildGumpling(closedArea, swatchWidth, swatchHeight, hues);
        }

        void buildGumpling(Rectangle area, int swatchWidth, int swatchHeight, int[] hues)
        {
            _hueSize = new Point(swatchWidth, swatchHeight);
            Position = new Vector2(area.X, area.Y);
            Size = new Vector2(area.Width, area.Height);
            _hues = hues;
            Index = 0;
            closeSwatch();
        }

        public override void Update(GameTime gameTime)
        {
            if (_isAnOpenSwatch)
            {
                if (_huesTexture == null)
                {
                _huesTexture = Data.HuesXNA.HueSwatch(_hueSize.X, _hueSize.Y, _hues);
                _selectedIndicator = Data.Gumps.GetGumpXNA(6000);
                }
            }
            else
            {
                if (_huesTexture == null || _getNewSelectedTexture)
                {
                    _getNewSelectedTexture = false;
                    _huesTexture = null;
                    _huesTexture = Data.HuesXNA.HueSwatch(1, 1, new int[1] { _hues[Index] });
                }
            }

            if (!_isAnOpenSwatch)
            {
                if (_isSwatchOpen && _openColorPicker.IsInitialized)
                {
                    if (_manager.MouseOverControl != _openColorPicker)
                        closeSwatch();
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            if (_isAnOpenSwatch)
            {
                spriteBatch.Draw(_huesTexture, Area, 0, false);
                spriteBatch.Draw(_selectedIndicator, new Vector2(
                    X + (float)(Width / _hueSize.X) * ((Index % _hueSize.X) + 0.5f) - _selectedIndicator.Width / 2,
                    Y + (float)(Height / _hueSize.Y) * ((Index / _hueSize.X) + 0.5f) - _selectedIndicator.Height / 2
                    ), 0, false);
            }
            else
            {
                if (!_isSwatchOpen)
                    spriteBatch.Draw(_huesTexture, Area, 0, false);
            }
            base.Draw(spriteBatch);
        }

        bool _isAnOpenSwatch = false;
        bool _isSwatchOpen = false;

        void openSwatch()
        {
            _isSwatchOpen = true;
            if (_openColorPicker != null)
            {
                _openColorPicker.Dispose();
                _openColorPicker = null;
            }
            _openColorPicker = new ColorPicker(_owner, Page, _openArea, _hueSize.X, _hueSize.Y, _hues);
            _openColorPicker.OnMouseClick = onOpenSwatchClick;
            ((Gump)_owner).AddGumpling(_openColorPicker);
        }

        void closeSwatch()
        {
            _isSwatchOpen = false;
            if (_openColorPicker != null)
            {
                _openColorPicker.Dispose();
                _openColorPicker = null;
            }
        }

        protected override void _mouseClick(int x, int y, UltimaXNA.Input.MouseButtons button)
        {
            if (!_isAnOpenSwatch)
            {
                openSwatch();
            }
        }

        protected override void _mouseOver(int x, int y)
        {
            if (_isAnOpenSwatch)
            {
                int clickRow = x / (Width / _hueSize.X);
                int clickColumn = y / (Height / _hueSize.Y);
                Index = clickRow + clickColumn * _hueSize.X;
            }
        }

        void onOpenSwatchClick(int x, int y, UltimaXNA.Input.MouseButtons button)
        {
            Index = _openColorPicker.Index;
            closeSwatch();
        }
    }
}
