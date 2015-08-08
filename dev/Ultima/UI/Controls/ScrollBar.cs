/***************************************************************************
 *   AScrollBar.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.Resources;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    /// <summary>
    /// A base scrollbar with methods that control min, max, and value
    /// </summary>
    class ScrollBar : AControl, IScrollBar
    {
        // ================================================================================
        // Private variables
        // ================================================================================
        private Texture2D[] m_GumpUpButton = null;
        private Texture2D[] m_GumpDownButton = null;
        private Texture2D[] m_GumpBackground = null;
        private Texture2D m_GumpSlider = null;

        private float m_SliderPosition;
        private float m_Value;
        private int m_Max, m_Min;

        private bool m_BtnUpClicked = false;
        private bool m_BtnDownClicked = false;
        private bool m_BtnSliderClicked = false;
        private Point m_ClickPosition;

        private float m_TimeUntilNextClick;
        private const float c_TimeBetweenClicks = 500f;

        // ================================================================================
        // Public properties
        // ================================================================================
        public int Value
        {
            get
            {
                return (int)m_Value;
            }
            set
            {
                m_Value = value;
                if (m_Value < MinValue)
                    m_Value = MinValue;
                if (m_Value > MaxValue)
                    m_Value = MaxValue;
            }
        }

        public int MinValue
        {
            get
            {
                return m_Min;
            }
            set
            {
                m_Min = value;
                if (m_Value < m_Min)
                    m_Value = m_Min;
            }
        }

        public int MaxValue
        {
            get
            {
                return m_Max;
            }
            set
            {
                if (value < 0)
                    value = 0;
                m_Max = value;
                if (m_Value > m_Max)
                    m_Value = m_Max;
            }
        }

        // ================================================================================
        // Ctors, Initialize, Update, and Draw
        // ================================================================================
        public ScrollBar(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
        }

        public ScrollBar(AControl parent, int x, int y, int height, int minValue, int maxValue, int value)
            : this(parent)
        {
            Position = new Point(x, y);
            MinValue = minValue;
            MaxValue = maxValue;
            Height = height;
            Value = value;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

            m_GumpUpButton = new Texture2D[2];
            m_GumpUpButton[0] = provider.GetUITexture(251);
            m_GumpUpButton[1] = provider.GetUITexture(250);
            m_GumpDownButton = new Texture2D[2];
            m_GumpDownButton[0] = provider.GetUITexture(253);
            m_GumpDownButton[1] = provider.GetUITexture(252);
            m_GumpBackground = new Texture2D[3];
            m_GumpBackground[0] = provider.GetUITexture(257);
            m_GumpBackground[1] = provider.GetUITexture(256);
            m_GumpBackground[2] = provider.GetUITexture(255);
            m_GumpSlider = provider.GetUITexture(254);
            Size = new Point(m_GumpBackground[0].Width, Height);
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);

            if (MaxValue <= MinValue || MinValue >= MaxValue)
            {
                Value = MaxValue = MinValue;
            }

            m_SliderPosition = CalculateSliderYPosition();

            if (m_BtnUpClicked || m_BtnDownClicked)
            {
                if (m_TimeUntilNextClick <= 0f)
                {
                    m_TimeUntilNextClick += c_TimeBetweenClicks;
                    if (m_BtnUpClicked)
                        Value -= 1;
                    if (m_BtnDownClicked)
                        Value += 1;
                }
                m_TimeUntilNextClick -= (float)totalMS;
            }
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            if (Height <= 0)
                return;

            // draw scrollbar background
            int middleHeight = Height - m_GumpUpButton[0].Height - m_GumpDownButton[0].Height - m_GumpBackground[0].Height - m_GumpBackground[2].Height;
            if (middleHeight > 0)
            {
                spriteBatch.Draw2D(m_GumpBackground[0], new Vector3(position.X, position.Y + m_GumpUpButton[0].Height, 0), Vector3.Zero);
                spriteBatch.Draw2DTiled(m_GumpBackground[1], new Rectangle(position.X, position.Y + m_GumpUpButton[0].Height + m_GumpBackground[0].Height, m_GumpBackground[0].Width, middleHeight), Vector3.Zero);
                spriteBatch.Draw2D(m_GumpBackground[2], new Vector3(position.X, position.Y + Height - m_GumpDownButton[0].Height - m_GumpBackground[2].Height, 0), Vector3.Zero);
            }
            else
            {
                middleHeight = Height - m_GumpUpButton[0].Height - m_GumpDownButton[0].Height;
                spriteBatch.Draw2DTiled(m_GumpBackground[1], new Rectangle(position.X, position.Y + m_GumpUpButton[0].Height, m_GumpBackground[0].Width, middleHeight), Vector3.Zero);
            }

            // draw up button
            spriteBatch.Draw2D(m_BtnUpClicked ? m_GumpUpButton[1] : m_GumpUpButton[0], new Vector3(position.X, position.Y, 0), Vector3.Zero);

            // draw down button
            spriteBatch.Draw2D(m_BtnDownClicked ? m_GumpDownButton[1] : m_GumpDownButton[0], new Vector3(position.X, position.Y + Height - m_GumpDownButton[0].Height, 0), Vector3.Zero);

            // draw slider
            if (MaxValue > MinValue && middleHeight > 0)
                spriteBatch.Draw2D(m_GumpSlider, new Vector3(position.X + (m_GumpBackground[0].Width - m_GumpSlider.Width) / 2, position.Y + m_GumpUpButton[0].Height + m_SliderPosition, 0), Vector3.Zero);

            base.Draw(spriteBatch, position);
        }

        private float CalculateSliderYPosition()
        {
            if (!IsInitialized)
                return 0f;
            if (MaxValue - MinValue == 0)
                return 0f;
            return CalculateScrollableArea() * ((m_Value - MinValue) / (MaxValue - MinValue));
        }

        private float CalculateScrollableArea()
        {
            if (!IsInitialized)
                return 0f;
            return Height - m_GumpUpButton[0].Height - m_GumpDownButton[0].Height - m_GumpSlider.Height;
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            m_TimeUntilNextClick = 0f;
            if (new Rectangle(0, Height - m_GumpDownButton[0].Height, m_GumpDownButton[0].Width, m_GumpDownButton[0].Height).Contains(new Point(x, y)))
            {
                // clicked on the down button
                m_BtnDownClicked = true;
            }
            else if (new Rectangle(0, 0, m_GumpUpButton[0].Width, m_GumpUpButton[0].Height).Contains(new Point(x, y)))
            {
                // clicked on the up button
                m_BtnUpClicked = true;
            }
            else if (new Rectangle((m_GumpBackground[0].Width - m_GumpSlider.Width) / 2, m_GumpUpButton[0].Height + (int)m_SliderPosition, m_GumpSlider.Width, m_GumpSlider.Height).Contains(new Point(x, y)))
            {
                // clicked on the slider
                m_BtnSliderClicked = true;
                m_ClickPosition = new Point(x, y);
            }
            else
            {
                // clicked on the bar. This should scroll up a full slider's height worth of entries.
                // not coded yet, obviously.
            }
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            m_BtnUpClicked = false;
            m_BtnDownClicked = false;
            m_BtnSliderClicked = false;
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (m_BtnSliderClicked)
            {
                if (y != m_ClickPosition.Y)
                {
                    float sliderY = m_SliderPosition + (y - m_ClickPosition.Y);

                    if (sliderY < 0)
                        sliderY = 0;

                    float scrollableArea = CalculateScrollableArea();
                    if (sliderY > scrollableArea)
                        sliderY = scrollableArea;

                    m_ClickPosition = new Point(x, y);

                    if (sliderY == 0 && m_ClickPosition.Y < m_GumpUpButton[0].Height + m_GumpSlider.Height / 2)
                        m_ClickPosition.Y = m_GumpUpButton[0].Height + m_GumpSlider.Height / 2;

                    if (sliderY == (scrollableArea) && m_ClickPosition.Y > Height - m_GumpDownButton[0].Height - m_GumpSlider.Height / 2)
                        m_ClickPosition.Y = Height - m_GumpDownButton[0].Height - m_GumpSlider.Height / 2;

                    m_Value = ((sliderY / scrollableArea) * (float)((MaxValue - MinValue))) + MinValue;
                    m_SliderPosition = sliderY;
                }
            }
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            Rectangle bounds = new Rectangle(0, 0, Width, Height);
            if (bounds.Contains(x, y))
                return true;
            return false;
        }

        public bool PointWithinControl(int x, int y)
        {
            return IsPointWithinControl(x, y);
        }
    }
}
