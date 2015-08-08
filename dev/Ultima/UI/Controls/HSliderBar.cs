/***************************************************************************
 *   Slider.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    enum HSliderBarStyle
    {
        MetalWidgetRecessedBar,
        BlueWidgetNoBar
    }

    class HSliderBar : AControl
    {
        Texture2D[] m_GumpSliderBackground = null;
        Texture2D m_GumpWidget = null;

        // we use m_newValue to (a) get delta, (b) so Value only changes once per frame.
        int m_newValue = 0, m_value = 0;
        public int Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = m_newValue = value;
                if (IsInitialized)
                    RecalculateSliderX();
            }
        }

        private void RecalculateSliderX()
        {
            m_sliderX = (int)((float)(BarWidth - m_GumpWidget.Width) * ((float)(Value - MinValue) / (float)(MaxValue - MinValue)));
        }

        public int MinValue;
        public int MaxValue;
        public int BarWidth;

        private int m_sliderX;
        private HSliderBarStyle Style;

        public HSliderBar(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
            m_pairedSliders = new List<HSliderBar>();
        }

        public HSliderBar(AControl parent, int x, int y, int width, int minValue, int maxValue, int value, HSliderBarStyle style)
            : this(parent)
        {
            buildGumpling(x, y, width, minValue, maxValue, value, style);
        }

        void buildGumpling(int x, int y, int width, int minValue, int maxValue, int value, HSliderBarStyle style)
        {
            Position = new Point(x, y);
            MinValue = minValue;
            MaxValue = maxValue;
            BarWidth = width;
            Value = value;
            Style = style;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_GumpWidget == null)
            {
                IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                switch (Style)
                {
                    default:
                    case HSliderBarStyle.MetalWidgetRecessedBar:
                        m_GumpSliderBackground = new Texture2D[3];
                        m_GumpSliderBackground[0] = provider.GetUITexture(213);
                        m_GumpSliderBackground[1] = provider.GetUITexture(214);
                        m_GumpSliderBackground[2] = provider.GetUITexture(215);
                        m_GumpWidget = provider.GetUITexture(216);
                        break;
                    case HSliderBarStyle.BlueWidgetNoBar:
                        m_GumpWidget = provider.GetUITexture(0x845);
                        break;
                }
                Size = new Point(BarWidth, m_GumpWidget.Height);
                RecalculateSliderX();
            }
            
            modifyPairedValues(m_newValue - Value);
            m_value = m_newValue;


            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            if (m_GumpSliderBackground != null)
            {
                spriteBatch.Draw2D(m_GumpSliderBackground[0], new Vector3(position.X, position.Y, 0), Vector3.Zero);
                spriteBatch.Draw2DTiled(m_GumpSliderBackground[1], new Rectangle(position.X + m_GumpSliderBackground[0].Width, position.Y, BarWidth - m_GumpSliderBackground[2].Width - m_GumpSliderBackground[0].Width, m_GumpSliderBackground[1].Height), Vector3.Zero);
                spriteBatch.Draw2D(m_GumpSliderBackground[2], new Vector3(position.X + BarWidth - m_GumpSliderBackground[2].Width, position.Y, 0), Vector3.Zero);
            }
            spriteBatch.Draw2D(m_GumpWidget, new Vector3(position.X + m_sliderX, position.Y, 0), Vector3.Zero);
            base.Draw(spriteBatch, position);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            if (new Rectangle(m_sliderX, 0, m_GumpWidget.Width, m_GumpWidget.Height).Contains(new Point(x, y)))
                return true;
            else
                return false;
        }

        bool m_clicked = false;
        Point m_clickPosition;

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            m_clicked = true;
            m_clickPosition = new Point(x, y);
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            m_clicked = false;
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (m_clicked)
            {
                m_sliderX = m_sliderX + (x - m_clickPosition.X);
                if (m_sliderX < 0)
                    m_sliderX = 0;
                if (m_sliderX > BarWidth - m_GumpWidget.Width)
                    m_sliderX = BarWidth - m_GumpWidget.Width;
                m_clickPosition = new Point(x, y);
                if (m_clickPosition.X < m_GumpWidget.Width / 2)
                    m_clickPosition.X = m_GumpWidget.Width / 2;
                if (m_clickPosition.X > BarWidth - m_GumpWidget.Width / 2)
                    m_clickPosition.X = BarWidth - m_GumpWidget.Width / 2;
                m_newValue = (int)(((float)m_sliderX / (float)(BarWidth - m_GumpWidget.Width)) * (float)((MaxValue - MinValue))) + MinValue;
            }
        }

        List<HSliderBar> m_pairedSliders;
        public void PairSlider(HSliderBar s)
        {
            m_pairedSliders.Add(s);
        }

        void modifyPairedValues(int delta)
        {
            if (m_pairedSliders.Count == 0)
                return;

            bool updateSinceLastCycle = true;
            int d = (delta > 0) ? -1 : 1;
            int points = Math.Abs(delta);
            int sliderIndex = Value % m_pairedSliders.Count;
            while (points > 0)
            {
                if (d > 0)
                {
                    if (m_pairedSliders[sliderIndex].Value < m_pairedSliders[sliderIndex].MaxValue)
                    {
                        updateSinceLastCycle = true;
                        m_pairedSliders[sliderIndex].Value += d;
                        points--;
                    }
                }
                else
                {
                    if (m_pairedSliders[sliderIndex].Value > m_pairedSliders[sliderIndex].MinValue)
                    {
                        updateSinceLastCycle = true;
                        m_pairedSliders[sliderIndex].Value += d;
                        points--;
                    }
                }
                sliderIndex++;
                if (sliderIndex == m_pairedSliders.Count)
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
