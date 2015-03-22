/***************************************************************************
 *   Slider.cs
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
using UltimaXNA.Rendering;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{

    class Slider : Control
    {
        Texture2D[] m_gumpBar = null;
        Texture2D m_gumpSlider = null;

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
                    m_sliderX = (int)((float)(BarWidth - m_gumpSlider.Width) * ((float)(Value - MinValue) / (float)(MaxValue - MinValue)));
            }
        }

        public int MinValue;
        public int MaxValue;
        public int BarWidth;

        int m_sliderX;

        public Slider(Control owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
            m_pairedSliders = new List<Slider>();
        }

        public Slider(Control owner, int page, int x, int y, int width, int minValue, int maxValue, int value)
            : this(owner, page)
        {
            buildGumpling(x, y, width, minValue, maxValue, value);
        }

        void buildGumpling(int x, int y, int width, int minValue, int maxValue, int value)
        {
            Position = new Point(x, y);
            MinValue = minValue;
            MaxValue = maxValue;
            BarWidth = width;
            Value = value; // must set this last
        }

        public override void Update(GameTime gameTime)
        {
            if (m_gumpSlider == null)
            {
                m_gumpBar = new Texture2D[3];
                m_gumpBar[0] = UltimaData.GumpData.GetGumpXNA(213);
                m_gumpBar[1] = UltimaData.GumpData.GetGumpXNA(214);
                m_gumpBar[2] = UltimaData.GumpData.GetGumpXNA(215);
                m_gumpSlider = UltimaData.GumpData.GetGumpXNA(216);
                Size = new Point(BarWidth, m_gumpSlider.Height);
                m_sliderX = (int)((float)(BarWidth - m_gumpSlider.Width) * ((float)(Value - MinValue) / (float)(MaxValue - MinValue)));
            }
            
            modifyPairedValues(m_newValue - Value);
            m_value = m_newValue;


            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            spriteBatch.Draw2D(m_gumpBar[0], Position, 0, false, false);
            spriteBatch.Draw2DTiled(m_gumpBar[1], new Rectangle(Area.X + m_gumpBar[0].Width, Area.Y, BarWidth - m_gumpBar[2].Width - m_gumpBar[0].Width, m_gumpBar[1].Height), 0, false, false);
            spriteBatch.Draw2D(m_gumpBar[2], new Point(Area.X + BarWidth - m_gumpBar[2].Width, Area.Y), 0, false, false);
            spriteBatch.Draw2D(m_gumpSlider, new Point(Area.X + m_sliderX, Area.Y), 0, false, false);
            base.Draw(spriteBatch);
        }

        protected override bool InternalHitTest(int x, int y)
        {
            if (new Rectangle(m_sliderX, 0, m_gumpSlider.Width, m_gumpSlider.Height).Contains(new Point(x, y)))
                return true;
            else
                return false;
        }

        bool m_clicked = false;
        Point m_clickPosition;

        protected override void mouseDown(int x, int y, MouseButton button)
        {
            m_clicked = true;
            m_clickPosition = new Point(x, y);
        }

        protected override void mouseUp(int x, int y, MouseButton button)
        {
            m_clicked = false;
        }

        protected override void mouseOver(int x, int y)
        {
            if (m_clicked)
            {
                m_sliderX = m_sliderX + (x - m_clickPosition.X);
                if (m_sliderX < 0)
                    m_sliderX = 0;
                if (m_sliderX > BarWidth - m_gumpSlider.Width)
                    m_sliderX = BarWidth - m_gumpSlider.Width;
                m_clickPosition = new Point(x, y);
                if (m_clickPosition.X < m_gumpSlider.Width / 2)
                    m_clickPosition.X = m_gumpSlider.Width / 2;
                if (m_clickPosition.X > BarWidth - m_gumpSlider.Width / 2)
                    m_clickPosition.X = BarWidth - m_gumpSlider.Width / 2;
                m_newValue = (int)(((float)m_sliderX / (float)(BarWidth - m_gumpSlider.Width)) * (float)((MaxValue - MinValue))) + MinValue;
            }
        }

        List<Slider> m_pairedSliders;
        public void PairSlider(Slider s)
        {
            m_pairedSliders.Add(s);
        }

        void modifyPairedValues(int delta)
        {
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
