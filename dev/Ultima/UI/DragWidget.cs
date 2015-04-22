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
        AControl m_toMove;
        bool isMoving = false; int moveOriginalX, moveOriginalY;

        public DragWidget(AControl inputFrom, AControl toMove)
        {
            m_toMove = toMove;

            inputFrom.MouseDownEvent += mouseDown;
            inputFrom.MouseUpEvent += mouseUp;
            inputFrom.MouseOverEvent += mouseOver;
        }

        void mouseDown(int x, int y, MouseButton button)
        {
            if (!m_toMove.IsMovable)
                return;

            x += m_toMove.X;
            y += m_toMove.Y;
            if (button == MouseButton.Left && m_toMove.IsMovable)
            {
                ClipMouse(ref x, ref y);
                isMoving = true;
                moveOriginalX = x;
                moveOriginalY = y;
            }
        }

        void mouseUp(int x, int y, MouseButton button)
        {
            x += m_toMove.X;
            y += m_toMove.Y;
            if (button == MouseButton.Left)
            {
                if (isMoving == true)
                {
                    UnclipMouse();
                    isMoving = false;
                    m_toMove.X += (x - moveOriginalX);
                    m_toMove.Y += (y - moveOriginalY);
                }
            }
        }

        void mouseOver(int x, int y)
        {
            x += m_toMove.X;
            y += m_toMove.Y;
            if (isMoving == true)
            {
                ClipMouse(ref x, ref y);
                m_toMove.X += (x - moveOriginalX);
                m_toMove.Y += (y - moveOriginalY);
                moveOriginalX = x;
                moveOriginalY = y;
            }
        }

        private void ClipMouse(ref int x, ref int y)
        {
            UltimaEngine engine = UltimaServices.GetService<UltimaEngine>();
            Rectangle rect = engine.Window.ClientBounds;

            if (x < -8)
                x = -8;
            if (y < -8)
                y = -8;
            if (x >= rect.Width + 8)
                x = rect.Width + 8;
            if (y >= rect.Height + 8)
                y = rect.Height + 8;
        }

        private void UnclipMouse()
        {

        }
    }
}
