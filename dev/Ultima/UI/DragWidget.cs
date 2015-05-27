/***************************************************************************
 *   DragWidget.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Input.Windows;
#endregion

namespace UltimaXNA.Ultima.UI
{
    class DragWidget
    {
        private AControl m_DragTarget;
        private bool m_IsMoving = false;
        private int m_DragOriginX, m_DragOriginY;

        public DragWidget(AControl inputFrom, AControl toMove)
        {
            m_DragTarget = toMove;

            inputFrom.MouseDownEvent += OnMouseDown;
            inputFrom.MouseUpEvent += OnMouseUp;
            inputFrom.MouseOverEvent += OnMouseOver;
        }

        private void OnMouseDown(int x, int y, MouseButton button)
        {
            if (!m_DragTarget.IsMovable)
                return;

            if (button == MouseButton.Left && m_DragTarget.IsMovable)
            {
                x += m_DragTarget.X;
                y += m_DragTarget.Y;
                ClipMouse(ref x, ref y);
                m_IsMoving = true;
                m_DragOriginX = x;
                m_DragOriginY = y;
            }
        }

        private void OnMouseUp(int x, int y, MouseButton button)
        {
            x += m_DragTarget.X;
            y += m_DragTarget.Y;
            if (button == MouseButton.Left)
            {
                if (m_IsMoving == true)
                {
                    UnclipMouse();
                    m_IsMoving = false;
                    m_DragTarget.Position = new Point(
                        m_DragTarget.X + (x - m_DragOriginX),
                        m_DragTarget.Y + (y - m_DragOriginY));
                }
            }
        }

        private void OnMouseOver(int x, int y)
        {
            x += m_DragTarget.X;
            y += m_DragTarget.Y;
            if (m_IsMoving == true)
            {
                ClipMouse(ref x, ref y);
                m_DragTarget.Position = new Point(
                    m_DragTarget.X + (x - m_DragOriginX),
                    m_DragTarget.Y + (y - m_DragOriginY));
                m_DragOriginX = x;
                m_DragOriginY = y;
            }
        }

        private void ClipMouse(ref int x, ref int y)
        {
            UltimaEngine engine = UltimaServices.GetService<UltimaEngine>();
            Rectangle window = engine.Window.ClientBounds;

            if (x < -8)
                x = -8;
            if (y < -8)
                y = -8;
            if (x >= window.Width + 8)
                x = window.Width + 8;
            if (y >= window.Height + 8)
                y = window.Height + 8;
        }

        private void UnclipMouse()
        {

        }
    }
}
