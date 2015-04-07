/***************************************************************************
 *   ColorPicker.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Rendering;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class ColorPicker : AControl
    {
        protected Texture2D m_huesTexture;
        protected Texture2D m_selectedIndicator;
        protected Rectangle m_openArea;

        protected Point m_hueSize;
        protected int[] m_hues;

        protected ColorPicker m_openColorPicker;

        bool m_getNewSelectedTexture;
        int m_index = 0;
        public int Index
        {
            get { return m_index; }
            set { m_index = value; m_getNewSelectedTexture = true; }
        }

        public int HueValue
        {
            get { return m_hues[Index]; }
            set
            {
                for (int i = 0; i < m_hues.Length; i++)
                {
                    if (value == m_hues[i])
                    {
                        Index = i;
                        break;
                    }
                }
            }
        }

        public ColorPicker(AControl owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
        }

        public ColorPicker(AControl owner, int page, Rectangle area, int swatchWidth, int swatchHeight, int[] hues)
            : this(owner, page)
        {
            m_isAnOpenSwatch = true;
            buildGumpling(area, swatchWidth, swatchHeight, hues);
        }

        public ColorPicker(AControl owner, int page, Rectangle closedArea, Rectangle openArea, int swatchWidth, int swatchHeight, int[] hues)
            : this(owner, page)
        {
            m_isAnOpenSwatch = false;
            m_openArea = openArea;
            buildGumpling(closedArea, swatchWidth, swatchHeight, hues);
        }

        void buildGumpling(Rectangle area, int swatchWidth, int swatchHeight, int[] hues)
        {
            m_hueSize = new Point(swatchWidth, swatchHeight);
            Position = new Point(area.X, area.Y);
            Size = new Point(area.Width, area.Height);
            m_hues = hues;
            Index = 0;
            closeSwatch();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_isAnOpenSwatch)
            {
                if (m_huesTexture == null)
                {
                m_huesTexture = UltimaData.HuesXNA.HueSwatch(m_hueSize.X, m_hueSize.Y, m_hues);
                m_selectedIndicator = UltimaData.GumpData.GetGumpXNA(6000);
                }
            }
            else
            {
                if (m_huesTexture == null || m_getNewSelectedTexture)
                {
                    m_getNewSelectedTexture = false;
                    m_huesTexture = null;
                    m_huesTexture = UltimaData.HuesXNA.HueSwatch(1, 1, new int[1] { m_hues[Index] });
                }
            }

            if (!m_isAnOpenSwatch)
            {
                if (m_isSwatchOpen && m_openColorPicker.IsInitialized)
                {
                    if (Engine.UserInterface.MouseOverControl != m_openColorPicker)
                        closeSwatch();
                }
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            if (m_isAnOpenSwatch)
            {
                spriteBatch.Draw2D(m_huesTexture, Area, 0, false, false);
                spriteBatch.Draw2D(m_selectedIndicator, new Point(
                    (int)(X + (float)(Width / m_hueSize.X) * ((Index % m_hueSize.X) + 0.5f) - m_selectedIndicator.Width / 2),
                    (int)(Y + (float)(Height / m_hueSize.Y) * ((Index / m_hueSize.X) + 0.5f) - m_selectedIndicator.Height / 2)
                    ), 0, false, false);
            }
            else
            {
                if (!m_isSwatchOpen)
                    spriteBatch.Draw2D(m_huesTexture, Area, 0, false, false);
            }
            base.Draw(spriteBatch);
        }

        bool m_isAnOpenSwatch = false;
        bool m_isSwatchOpen = false;

        void openSwatch()
        {
            m_isSwatchOpen = true;
            if (m_openColorPicker != null)
            {
                m_openColorPicker.Dispose();
                m_openColorPicker = null;
            }
            m_openColorPicker = new ColorPicker(m_owner, Page, m_openArea, m_hueSize.X, m_hueSize.Y, m_hues);
            m_openColorPicker.OnMouseClick = onOpenSwatchClick;
            ((Gump)m_owner).AddControl(m_openColorPicker);
        }

        void closeSwatch()
        {
            m_isSwatchOpen = false;
            if (m_openColorPicker != null)
            {
                m_openColorPicker.Dispose();
                m_openColorPicker = null;
            }
        }

        protected override void mouseClick(int x, int y, MouseButton button)
        {
            if (!m_isAnOpenSwatch)
            {
                openSwatch();
            }
        }

        protected override void mouseOver(int x, int y)
        {
            if (m_isAnOpenSwatch)
            {
                int clickRow = x / (Width / m_hueSize.X);
                int clickColumn = y / (Height / m_hueSize.Y);
                Index = clickRow + clickColumn * m_hueSize.X;
            }
        }

        void onOpenSwatchClick(int x, int y, MouseButton button)
        {
            Index = m_openColorPicker.Index;
            closeSwatch();
        }
    }
}
