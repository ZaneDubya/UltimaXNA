using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.TileEngine;

namespace UltimaXNA.InputOld
{
    public enum MouseButton
    {
        LeftButton,
        MiddleButton,
        RightButton,
        XButton1,
        XButton2,
        Count = XButton2
    }

    enum ClickTypes
    {
        None, SingleClick, DoubleClick, Drag
    }

    class ClickState
    {
        bool _canDrag;
        float _pickUpTime;
        bool _singleClick;
        float _singleClickTime;
        bool _doubleClick;
        Point _clickPoint;
        ClickTypes _click = ClickTypes.None;
        public MapObject Object;
        public Point ObjectClickPoint;

        public ClickTypes Click
        {
            get
            {
                return _click;
            }

            protected set
            {
                _canDrag = false;
                _singleClick = false;
                _doubleClick = false;
                _click = value;
            }
        }

        public void Reset()
        {
            Click = ClickTypes.None;
        }

        public void Press(int x, int y, MapObject mapObject, Point clickOffset)
        {
            Object = mapObject;
            ObjectClickPoint = clickOffset;
            _canDrag = true;
            _pickUpTime = ClientVars.TheTime + ClientVars.SecondsBetweenClickAndPickUp;
            _clickPoint = new Point(x, y);
        }

        public void Release(int x, int y)
        {
            _canDrag = false;
            if (_singleClick == true && ClientVars.TheTime < _singleClickTime && !hasMovedFromClickPoint(x, y, 2))
            {
                _singleClick = false;
                _doubleClick = true;
            }
            else
            {
                _singleClick = true;
                _singleClickTime = ClientVars.TheTime + ClientVars.SecondsForDoubleClick;
            }
        }

        public void Update(int x, int y)
        {
            if (_canDrag && hasMovedFromClickPoint(x, y, 2))
            {
                Click = ClickTypes.Drag;
            }

            if (_canDrag && ClientVars.TheTime >= _pickUpTime)
            {
                Click = ClickTypes.Drag;
            }

            if (_singleClick && ClientVars.TheTime >= _singleClickTime)
            {
                Click = ClickTypes.SingleClick;
            }

            if (_doubleClick)
            {
                Click = ClickTypes.DoubleClick;
            }
        }

        bool hasMovedFromClickPoint(int x, int y, int distance)
        {
            if (Math.Abs(_clickPoint.X - x) + Math.Abs(_clickPoint.Y - y) > distance)
                return true;
            else
                return false;
        }
    }
}
