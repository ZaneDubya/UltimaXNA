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
        Control _toMove;
        bool isMoving = false; int moveOriginalX, moveOriginalY;

        public DragWidget(Control inputFrom, Control toMove)
        {
            _toMove = toMove;

            inputFrom.OnMouseDown += mouseDown;
            inputFrom.OnMouseUp += mouseUp;
            inputFrom.OnMouseOver += mouseOver;
        }

        void mouseDown(int x, int y, MouseButton button)
        {
            x += _toMove.X;
            y += _toMove.Y;
            if (button == MouseButton.Left && _toMove.IsMovable)
            {
                // move!
                isMoving = true;
                moveOriginalX = x;
                moveOriginalY = y;
            }
        }

        void mouseUp(int x, int y, MouseButton button)
        {
            x += _toMove.X;
            y += _toMove.Y;
            if (button == MouseButton.Left)
            {
                if (isMoving == true)
                {
                    isMoving = false;
                    _toMove.X += (x - moveOriginalX);
                    _toMove.Y += (y - moveOriginalY);
                }
            }
        }

        void mouseOver(int x, int y)
        {
            x += _toMove.X;
            y += _toMove.Y;
            if (isMoving == true)
            {
                _toMove.X += (x - moveOriginalX);
                _toMove.Y += (y - moveOriginalY);
                moveOriginalX = x;
                moveOriginalY = y;
            }
        }
    }
}
