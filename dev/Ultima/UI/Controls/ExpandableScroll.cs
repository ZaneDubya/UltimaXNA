/***************************************************************************
 *   ExpandableScroll.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    class ExpandableScroll : Gump
    {
        private GumpPic m_GumplingTop, m_GumplingBottom;
        private GumpPicTiled m_GumplingMiddle;
        private Button m_GumplingExpander;
        
        private int m_ExpandableScrollHeight;
        private const int c_ExpandableScrollHeight_Min = 274; // this is the min from the client.
        private const int c_ExpandableScrollHeight_Max = 1000; // arbitrary large number.

        private int m_GumplingMidY { get { return m_GumplingTop.Height; } }
        private int m_GumplingMidHeight { get { return m_ExpandableScrollHeight - m_GumplingTop.Height - m_GumplingBottom.Height - (m_GumplingExpander != null ? m_GumplingExpander.Height : 0); } }
        private int m_GumplingBottomY { get { return m_ExpandableScrollHeight - m_GumplingBottom.Height - (m_GumplingExpander != null ? m_GumplingExpander.Height : 0); } }
        private int m_GumplingExpanderX { get { return (Width - (m_GumplingExpander != null ? m_GumplingExpander.Width : 0)) / 2; } }
        private int m_GumplingExpanderY { get { return m_ExpandableScrollHeight - (m_GumplingExpander != null ? m_GumplingExpander.Height : 0) - c_GumplingExpanderY_Offset; } }
        private const int c_GumplingExpanderY_Offset = 2; // this is the gap between the pixels of the btm Control texture and the height of the btm Control texture.
        private const int c_GumplingExpander_ButtonID = 0x7FBEEF;

        private bool m_IsResizable = true;
        private bool m_IsExpanding = false;
        private int m_isExpanding_InitialX, m_isExpanding_InitialY, m_isExpanding_InitialHeight;

        public ExpandableScroll(AControl parent, int x, int y, int height, bool isResizable = true)
            : base(0, 0)
        {
            Parent = parent;
            Position = new Point(x, y);
            m_ExpandableScrollHeight = height;
            m_IsResizable = isResizable;
            MakeThisADragger();
        }

        protected override void OnInitialize()
        {
            m_GumplingTop = (GumpPic)AddControl(new GumpPic(this, 0, 0, 0x0820, 0));
            m_GumplingMiddle = (GumpPicTiled)AddControl(new GumpPicTiled(this, 0, 0, 0, 0, 0x0822));
            m_GumplingBottom = (GumpPic)AddControl(new GumpPic(this, 0, 0, 0x0823, 0));

            if (m_IsResizable)
            {
                m_GumplingExpander = (Button)AddControl(new Button(this, 0, 0, 0x082E, 0x82F, ButtonTypes.Activate, 0, c_GumplingExpander_ButtonID));
                m_GumplingExpander.MouseDownEvent += expander_OnMouseDown;
                m_GumplingExpander.MouseUpEvent += expander_OnMouseUp;
                m_GumplingExpander.MouseOverEvent += expander_OnMouseOver;
            }
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            Point position = new Point(x + ScreenX, y + ScreenY);
            if (m_GumplingTop.HitTest(position, true) != null)
                return true;
            if (m_GumplingMiddle.HitTest(position, true) != null)
                return true;
            if (m_GumplingBottom.HitTest(position, true) != null)
                return true;
            if (m_IsResizable && m_GumplingExpander.HitTest(position, true) != null)
                return true;
            return false;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_ExpandableScrollHeight < c_ExpandableScrollHeight_Min)
                m_ExpandableScrollHeight = c_ExpandableScrollHeight_Min;
            if (m_ExpandableScrollHeight > c_ExpandableScrollHeight_Max)
                m_ExpandableScrollHeight = c_ExpandableScrollHeight_Max;

            if (m_gumplingTitleGumpIDDelta)
            {
                m_gumplingTitleGumpIDDelta = false;
                if (m_gumplingTitle != null)
                    m_gumplingTitle.Dispose();
                m_gumplingTitle = (GumpPic)AddControl(new GumpPic(this, 0, 0, m_gumplingTitleGumpID, 0));
            }

            if (!m_GumplingTop.IsInitialized)
            {
                IsVisible = false;
            }
            else 
            {
                IsVisible = true;
                m_GumplingTop.Position = new Point(0, 0);

                m_GumplingMiddle.Position = new Point(17, m_GumplingMidY);
                m_GumplingMiddle.Width = 263;
                m_GumplingMiddle.Height = m_GumplingMidHeight;

                m_GumplingBottom.Position = new Point(17, m_GumplingBottomY);

                if (m_IsResizable)
                    m_GumplingExpander.Position = new Point(m_GumplingExpanderX, m_GumplingExpanderY);

                if (m_gumplingTitle != null && m_gumplingTitle.IsInitialized)
                {
                    m_gumplingTitle.Position = new Point(
                        (m_GumplingTop.Width - m_gumplingTitle.Width) / 2,
                        (m_GumplingTop.Height - m_gumplingTitle.Height) / 2);
                }
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);
        }

        void expander_OnMouseDown(AControl control, int x, int y, MouseButton button)
        {
            y += m_GumplingExpander.Y + ScreenY - Y;
            if (button == MouseButton.Left)
            {
                m_IsExpanding = true;
                m_isExpanding_InitialHeight = m_ExpandableScrollHeight;
                m_isExpanding_InitialX = x;
                m_isExpanding_InitialY = y;
            }
        }

        void expander_OnMouseUp(AControl control, int x, int y, MouseButton button)
        {
            y += m_GumplingExpander.Y + ScreenY - Y;
            if (m_IsExpanding)
            {
                m_IsExpanding = false;
                m_ExpandableScrollHeight = m_isExpanding_InitialHeight + (y - m_isExpanding_InitialY);
            }
        }

        void expander_OnMouseOver(AControl control, int x, int y)
        {
            y += m_GumplingExpander.Y + ScreenY - Y;
            if (m_IsExpanding && (y != m_isExpanding_InitialY))
            {
                m_ExpandableScrollHeight = m_isExpanding_InitialHeight + (y - m_isExpanding_InitialY);
            }
        }

        bool m_gumplingTitleGumpIDDelta = false;
        int m_gumplingTitleGumpID;
        GumpPic m_gumplingTitle;
        public int TitleGumpID { set { m_gumplingTitleGumpID = value; m_gumplingTitleGumpIDDelta = true; } }
    }
}
