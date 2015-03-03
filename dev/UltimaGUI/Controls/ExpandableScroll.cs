/***************************************************************************
 *   ExpandableScroll.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using UltimaXNA.Rendering;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class ExpandableScroll : Gump
    {
        GumpPic m_gumplingTop, m_gumplingBottom;
        GumpPicTiled m_gumplingMiddle;
        Button m_gumplingExpander;
        
        int m_expandableScrollHeight;
        const int m_expandableScrollHeight_Min = 274; // this is the min from the client.
        const int m_expandableScrollHeight_Max = 1000; // arbitrary large number.

        int gumplingMidY { get { return m_gumplingTop.Height; } }
        int gumplingMidHeight { get { return m_expandableScrollHeight - m_gumplingTop.Height - m_gumplingBottom.Height - m_gumplingExpander.Height; } }
        int gumplingBottomY { get { return m_expandableScrollHeight - m_gumplingBottom.Height - m_gumplingExpander.Height; } }
        int gumplingExpanderX { get { return (Width - m_gumplingExpander.Width) / 2; } }
        int gumplingExpanderY { get { return m_expandableScrollHeight - m_gumplingExpander.Height - gumplingExpanderY_Offset; } }
        const int gumplingExpanderY_Offset = 2; // this is the gap between the pixels of the btm Control texture and the height of the btm Control texture.
        const int gumplingExpander_ButtonID = 0x7FBEEF;

        bool m_isExpanding = false;
        int m_isExpanding_InitialX, m_isExpanding_InitialY, m_isExpanding_InitialHeight;

        public ExpandableScroll(Control owner, int page, int x, int y, int height)
            : base(0, 0)
        {
            m_owner = owner;
            Position = new Point2D(x, y);
            m_expandableScrollHeight = height;
        }

        public override void Initialize()
        {
            m_gumplingTop = (GumpPic)AddControl(new GumpPic(this, 0, 0, 0, 0x0820, 0));
            m_gumplingMiddle = (GumpPicTiled)AddControl(new GumpPicTiled(this, 0, 0, 0, 0, 0, 0x0822));
            m_gumplingBottom = (GumpPic)AddControl(new GumpPic(this, 0, 0, 0, 0x0823, 0));
            m_gumplingExpander = (Button)AddControl(new Button(this, 0, 0, 0, 0x082E, 0x82F, ButtonTypes.Activate, 0, gumplingExpander_ButtonID));
            
            m_gumplingExpander.OnMouseDown = expander_OnMouseDown;
            m_gumplingExpander.OnMouseUp = expander_OnMouseUp;
            m_gumplingExpander.OnMouseOver = expander_OnMouseOver;
        }

        protected override bool m_hitTest(int x, int y)
        {
            Point2D position = new Point2D(x + OwnerX + X, y + OwnerY + Y);
            if (m_gumplingTop.HitTest(position, true) != null)
                return true;
            if (m_gumplingMiddle.HitTest(position, true) != null)
                return true;
            if (m_gumplingBottom.HitTest(position, true) != null)
                return true;
            if (m_gumplingExpander.HitTest(position, true) != null)
                return true;
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_expandableScrollHeight < m_expandableScrollHeight_Min)
                m_expandableScrollHeight = m_expandableScrollHeight_Min;
            if (m_expandableScrollHeight > m_expandableScrollHeight_Max)
                m_expandableScrollHeight = m_expandableScrollHeight_Max;

            if (m_gumplingTitleGumpIDDelta)
            {
                m_gumplingTitleGumpIDDelta = false;
                if (m_gumplingTitle != null)
                    m_gumplingTitle.Dispose();
                m_gumplingTitle = (GumpPic)AddControl(new GumpPic(this, 0, 0, 0, m_gumplingTitleGumpID, 0));
            }

            if (!m_gumplingTop.IsInitialized)
            {
                Visible = false;
            }
            else 
            {
                Visible = true;
                m_gumplingTop.X = 0;
                m_gumplingTop.Y = 0;

                m_gumplingMiddle.X = 17;
                m_gumplingMiddle.Y = gumplingMidY;
                m_gumplingMiddle.Width = 263;
                m_gumplingMiddle.Height = gumplingMidHeight;

                m_gumplingBottom.X = 17;
                m_gumplingBottom.Y = gumplingBottomY;

                m_gumplingExpander.X = gumplingExpanderX;
                m_gumplingExpander.Y = gumplingExpanderY;

                if (m_gumplingTitle != null && m_gumplingTitle.IsInitialized)
                {
                    m_gumplingTitle.X = (m_gumplingTop.Width - m_gumplingTitle.Width) / 2;
                    m_gumplingTitle.Y = (m_gumplingTop.Height - m_gumplingTitle.Height) / 2;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void ActivateByButton(int buttonID)
        {
            // this is necessary to override the default behavior for buttons, which is to send a msg to the server.
        }

        void expander_OnMouseDown(int x, int y, MouseButton button)
        {
            y += m_gumplingExpander.Y + OwnerY;
            if (button == MouseButton.Left)
            {
                m_isExpanding = true;
                m_isExpanding_InitialHeight = m_expandableScrollHeight;
                m_isExpanding_InitialX = x;
                m_isExpanding_InitialY = y;
            }
        }

        void expander_OnMouseUp(int x, int y, MouseButton button)
        {
            y += m_gumplingExpander.Y + OwnerY;
            if (m_isExpanding)
            {
                m_isExpanding = false;
                m_expandableScrollHeight = m_isExpanding_InitialHeight + (y - m_isExpanding_InitialY);
            }
        }

        void expander_OnMouseOver(int x, int y)
        {
            y += m_gumplingExpander.Y + OwnerY;
            if (m_isExpanding && (y != m_isExpanding_InitialY))
            {
                m_expandableScrollHeight = m_isExpanding_InitialHeight + (y - m_isExpanding_InitialY);
            }
        }

        bool m_gumplingTitleGumpIDDelta = false;
        int m_gumplingTitleGumpID;
        GumpPic m_gumplingTitle;
        public int TitleGumpID { set { m_gumplingTitleGumpID = value; m_gumplingTitleGumpIDDelta = true; } }
    }
}
