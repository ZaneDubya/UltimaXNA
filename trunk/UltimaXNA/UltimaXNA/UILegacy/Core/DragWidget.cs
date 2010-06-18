using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Input;

namespace UltimaXNA.UILegacy
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

        void mouseDown(int x, int y, MouseButtons button)
        {
            x += _toMove.X;
            y += _toMove.Y;
            if (button == MouseButtons.LeftButton && _toMove.IsMovable)
            {
                // move!
                isMoving = true;
                moveOriginalX = x;
                moveOriginalY = y;
            }
        }

        void mouseUp(int x, int y, MouseButtons button)
        {
            x += _toMove.X;
            y += _toMove.Y;
            if (button == MouseButtons.LeftButton)
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
