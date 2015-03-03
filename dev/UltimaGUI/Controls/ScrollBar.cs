/***************************************************************************
 *   ScrollBar.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Rendering;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class ScrollBar : Control
    {
        Texture2D[] m_gumpUpButton = null;
        Texture2D[] m_gumpDownButton = null;
        Texture2D m_gumpSlider = null;
        Texture2D[] m_gumpBackground = null;

        float m_sliderY;
        float m_value = 0;
        public int Value
        {
            get
            {
                return (int)m_value;
            }
            set
            {
                m_value = value;
                if (m_value < MinValue)
                    m_value = MinValue;
                if (m_value > MaxValue)
                    m_value = MaxValue;
                m_sliderY = calculateSliderY();
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
                m_sliderY = calculateSliderY();
            }
        }

        float calculateSliderY()
        {
            if (!IsInitialized)
                return 0f;
            if (MaxValue - MinValue == 0)
                return 0f;
            return scrollableArea() * ((m_value - MinValue) / (MaxValue - MinValue));
        }

        float scrollableArea()
        {
            if (!IsInitialized)
                return 0f;
            return BarHeight - m_gumpUpButton[0].Height - m_gumpDownButton[0].Height - m_gumpSlider.Height - 2;
        }

        public int MinValue;
        private int m_maxValue;
        public int MaxValue
        {
            get
            {
                return m_maxValue;
            }
            set
            {
                m_maxValue = value;
                if (m_value > m_maxValue)
                    m_value = m_maxValue;
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
            if (m_gumpSlider == null)
            {
                m_gumpUpButton = new Texture2D[2];
                m_gumpUpButton[0] = UltimaData.GumpData.GetGumpXNA(251);
                m_gumpUpButton[1] = UltimaData.GumpData.GetGumpXNA(250);
                m_gumpDownButton = new Texture2D[2];
                m_gumpDownButton[0] = UltimaData.GumpData.GetGumpXNA(253);
                m_gumpDownButton[1] = UltimaData.GumpData.GetGumpXNA(252);
                m_gumpBackground = new Texture2D[3];
                m_gumpBackground[0] = UltimaData.GumpData.GetGumpXNA(257);
                m_gumpBackground[1] = UltimaData.GumpData.GetGumpXNA(256);
                m_gumpBackground[2] = UltimaData.GumpData.GetGumpXNA(255);
                m_gumpSlider = UltimaData.GumpData.GetGumpXNA(254);
                Size = new Point2D(m_gumpBackground[0].Width, BarHeight);
            }

            if (m_btnUpClicked || m_btnDownClicked)
            {
                if (m_timeUntilNextClick <= 0f)
                {
                    m_timeUntilNextClick = m_timeBetweenClicks;
                    if (m_btnUpClicked)
                        m_value -= 1;
                    if (m_btnDownClicked)
                        m_value += 1;
                }
                m_timeUntilNextClick -= (float)gameTime.ElapsedGameTime.TotalSeconds / 1000f;
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            // up button
            spriteBatch.Draw2D(m_btnUpClicked ? m_gumpUpButton[1] : m_gumpUpButton[0], new Point2D(X, Y), 0, false, false);
            // scrollbar background
            spriteBatch.Draw2D(m_gumpBackground[0], new Point2D(X, Y + m_gumpUpButton[0].Height), 0, false, false);
            int middlewidth = BarHeight - m_gumpUpButton[0].Height - m_gumpDownButton[0].Height - m_gumpBackground[0].Height - m_gumpBackground[2].Height;
            spriteBatch.Draw2DTiled(m_gumpBackground[1], new Rectangle(X, Y + m_gumpUpButton[0].Height + m_gumpBackground[0].Height, m_gumpBackground[0].Width, middlewidth), 0, false, false);
            spriteBatch.Draw2D(m_gumpBackground[2], new Point2D(X, Y + BarHeight - m_gumpDownButton[0].Height - m_gumpBackground[2].Height), 0, false, false);
            // down button
            spriteBatch.Draw2D(m_btnDownClicked ? m_gumpDownButton[1] : m_gumpDownButton[0], new Point2D(X, Y + Height - m_gumpDownButton[0].Height), 0, false, false);
            // slider
            spriteBatch.Draw2D(m_gumpSlider, new Point2D(X + (m_gumpBackground[0].Width - m_gumpSlider.Width) / 2, Y + m_gumpUpButton[0].Height + 1 + (int)m_sliderY), 0, false, false);
            base.Draw(spriteBatch);
        }

        protected override bool m_hitTest(int x, int y)
        {
            if (new Rectangle(0, 0, Width, Height).Contains(new Point(x, y)))
                return true;
            else
                return false;
        }

        bool m_btnUpClicked = false;
        bool m_btnDownClicked = false;
        bool m_btnSliderClicked = false;
        Point m_clickPosition;

        float m_timeUntilNextClick;
        const float m_timeBetweenClicks = 0.5f;

        protected override void mouseDown(int x, int y, MouseButton button)
        {
            m_timeUntilNextClick = 0f;
            if (new Rectangle(0, 0, m_gumpUpButton[0].Width, m_gumpUpButton[0].Height).Contains(new Point(x, y)))
                m_btnUpClicked = true;
            else if (new Rectangle(0, Height - m_gumpDownButton[0].Height, m_gumpDownButton[0].Width, m_gumpDownButton[0].Height).Contains(new Point(x, y)))
                m_btnDownClicked = true;
            else if (new Rectangle((m_gumpBackground[0].Width - m_gumpSlider.Width) / 2, m_gumpUpButton[0].Height + (int)m_sliderY, m_gumpSlider.Width, m_gumpSlider.Height).Contains(new Point(x, y)))
            {
                m_btnSliderClicked = true;
                m_clickPosition = new Point(x, y);
            }
            else
            {
                // clicked on the bar. This should scroll up a full slider's height worth of entries.
                // not coded yet, obviously.
            }
        }

        protected override void mouseUp(int x, int y, MouseButton button)
        {
            m_btnUpClicked = false;
            m_btnDownClicked = false;
            m_btnSliderClicked = false;
        }

        protected override void mouseOver(int x, int y)
        {
            if (m_btnSliderClicked)
            {
                if (y != m_clickPosition.Y)
                {
                    float sliderY = m_sliderY + (y - m_clickPosition.Y);
                    if (sliderY < 0)
                        sliderY = 0;
                    if (sliderY > scrollableArea())
                        sliderY = scrollableArea();
                    m_clickPosition = new Point(x, y);
                    if (sliderY == 0 && m_clickPosition.Y < m_gumpUpButton[0].Height + m_gumpSlider.Height / 2)
                        m_clickPosition.Y = m_gumpUpButton[0].Height + m_gumpSlider.Height / 2;
                    if (sliderY == (scrollableArea()) && m_clickPosition.Y > BarHeight - m_gumpDownButton[0].Height - m_gumpSlider.Height / 2)
                        m_clickPosition.Y = BarHeight - m_gumpDownButton[0].Height - m_gumpSlider.Height / 2;
                    m_value = ((sliderY / scrollableArea()) * (float)((MaxValue - MinValue))) + MinValue;
                    m_sliderY = sliderY;
                }
            }
        }
    }
}
