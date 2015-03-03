/***************************************************************************
 *   DragWidget.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using InterXLib.Input.Windows;

namespace UltimaXNA.UltimaGUI
{
    class DragWidget
    {
        Control m_toMove;
        bool isMoving = false; int moveOriginalX, moveOriginalY;

        public DragWidget(Control inputFrom, Control toMove)
        {
            m_toMove = toMove;

            inputFrom.OnMouseDown += mouseDown;
            inputFrom.OnMouseUp += mouseUp;
            inputFrom.OnMouseOver += mouseOver;
        }

        void mouseDown(int x, int y, MouseButton button)
        {
            x += m_toMove.X;
            y += m_toMove.Y;
            if (button == MouseButton.Left && m_toMove.IsMovable)
            {
                // move!
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
                m_toMove.X += (x - moveOriginalX);
                m_toMove.Y += (y - moveOriginalY);
                moveOriginalX = x;
                moveOriginalY = y;
            }
        }
    }
}
