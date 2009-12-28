using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;

namespace UltimaXNA.UILegacy.Gumplings
{
    class GumpPicContainer : GumpPic
    {
        public GumpPicContainer(Control owner, int page, int x, int y, int gumpID, int hue)
            : base(owner, page, x, y, gumpID, hue)
        {
            HandlesMouseInput = true;
        }

        protected override bool _hitTest(int x, int y)
        {
            Color[] pixelData;
            pixelData = new Color[1];
            _texture.GetData<Color>(0, new Rectangle(x, y, 1, 1), pixelData, 0, 1);
            if (pixelData[0].A > 0)
                return true;
            else
                return false;
        }

        bool isMoving = false; int moveOriginalX, moveOriginalY;

        protected override void _mouseDown(int x, int y, MouseButtons button)
        {
            x += _owner.X;
            y += _owner.Y;
            if (button == MouseButtons.LeftButton)
            {
                // move!
                isMoving = true;
                moveOriginalX = x;
                moveOriginalY = y;
            }
        }

        protected override void _mouseUp(int x, int y, MouseButtons button)
        {
            x += _owner.X;
            y += _owner.Y;
            if (button == MouseButtons.LeftButton)
            {
                if (isMoving == true)
                {
                    isMoving = false;
                    _owner.X += (x - moveOriginalX);
                    _owner.Y += (y - moveOriginalY);
                }
            }
        }

        protected override void _mouseOver(int x, int y)
        {
            x += _owner.X;
            y += _owner.Y;
            if (isMoving == true)
            {
                _owner.X += (x - moveOriginalX);
                _owner.Y += (y - moveOriginalY);
                moveOriginalX = x;
                moveOriginalY = y;
            }
        }

        protected override void _mouseClick(int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.RightButton)
            {
                _owner.Dispose();
            }
        }
    }
}
