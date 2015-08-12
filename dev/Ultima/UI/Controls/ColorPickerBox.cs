/***************************************************************************
 *   ColorPickerButton.cs
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
    class ColorPickerBox : AControl
    {
        protected Texture2D m_huesTexture;
        protected Texture2D m_selectedIndicator;
        protected Rectangle m_openArea;

        protected int m_hueWidth, m_hueHeight;
        protected int[] m_hues;
        protected int m_Index;

        protected ColorPickerBox m_ChildColorPicker;
        private Texture2D m_Inactive;

        public int Index
        {
            get;
            set;
        }

        public bool IsChild = false;
        public ColorPickerBox ParentColorPicker = null;

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

        UserInterfaceService m_UserInterface;

        public ColorPickerBox(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
        }

        public ColorPickerBox(AControl parent, Rectangle area, int swatchWidth, int swatchHeight, int[] hues)
            : this(parent)
        {
            buildGumpling(area, swatchWidth, swatchHeight, hues);
        }

        public ColorPickerBox(AControl parent, Rectangle closedArea, Rectangle openArea, int swatchWidth, int swatchHeight, int[] hues, int index)
            : this(parent)
        {
            m_openArea = openArea;
            m_Index = index;
            buildGumpling(closedArea, swatchWidth, swatchHeight, hues);
        }

        void buildGumpling(Rectangle area, int swatchWidth, int swatchHeight, int[] hues)
        {
            m_hueWidth = swatchWidth;
            m_hueHeight = swatchHeight;
            Position = new Point(area.X, area.Y);
            Size = new Point(area.Width, area.Height);
            m_hues = hues;
            Index = m_Index;
        }

        protected override void OnInitialize()
        {
            if (m_huesTexture == null)
            {
                if (IsChild) // is a child
                {
                    m_huesTexture = HueData.CreateHueSwatch(m_hueWidth, m_hueHeight, m_hues);
                    IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                    m_selectedIndicator = provider.GetUITexture(6000);
                }
                else
                {
                    m_huesTexture = HueData.CreateHueSwatch(1, 1, new int[1] { m_hues[Index] });
                }
            }
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            m_Inactive = provider.GetUITexture(210);

            spriteBatch.Draw2D(m_Inactive, new Vector3(position.X, position.Y, 0), Vector3.Zero);
            spriteBatch.Draw2D(m_huesTexture, new Rectangle(position.X+3, position.Y+3, Width-2, Height-1), Vector3.Zero);

            if (IsChild && IsMouseOver)
            {
                spriteBatch.Draw2D(m_selectedIndicator, new Vector3(
                    (int)(position.X + (float)(Width / m_hueWidth) * ((Index % m_hueWidth) + 0.5f) - m_selectedIndicator.Width / 2),
                    (int)(position.Y + (float)(Height / m_hueHeight) * ((Index / m_hueWidth) + 0.5f) - m_selectedIndicator.Height / 2),
                    0), Vector3.Zero);
            }
            base.Draw(spriteBatch, position);
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (IsChild) // is a child
            {
                ParentColorPicker.Index = this.Index;
                ParentColorPicker.CloseChildPicker();
            }
            else
            {
                if (m_ChildColorPicker == null)
                {
                    m_ChildColorPicker = new ColorPickerBox(this.Parent, m_openArea, m_hueWidth, m_hueHeight, m_hues);
                    m_ChildColorPicker.IsChild = true;
                    m_ChildColorPicker.ParentColorPicker = this;
                    Parent.AddControl(m_ChildColorPicker, this.Page);
                }
                else
                {
                    m_ChildColorPicker.Dispose();
                    m_ChildColorPicker = null;
                }

            }
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (IsChild)
            {
                int clickRow = x / (Width / m_hueWidth);
                int clickColumn = y / (Height / m_hueHeight);
                ParentColorPicker.Index = Index = clickRow + clickColumn * m_hueWidth;
            }
        }

        protected override void OnMouseOut(int x, int y)
        {
        }

        protected void CloseChildPicker()
        {
            if (m_ChildColorPicker != null)
            {
                m_ChildColorPicker.Dispose();
                m_ChildColorPicker = null;
                m_huesTexture = HueData.CreateHueSwatch(1, 1, new int[1] { m_hues[Index] });
            }
        }
    }
}
