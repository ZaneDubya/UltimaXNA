/***************************************************************************
 *   DropDownList.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Interface.Input;
using UltimaXNA.Interface.GUI;

namespace UltimaXNA.UltimaGUI.Gumplings
{
    class DropDownList : Control
    {
        public int Index;

        int _width;
        List<string> _items;
        int _visibleItems;
        bool _canBeNull;

        ResizePic _resize;
        TextLabelAscii _label;

        bool _listOpen = false;
        ResizePic _openResizePic;
        ScrollBar _openScrollBar;
        TextLabelAscii[] _openLabels;

        const int hue_Text = 1107;
        const int hue_TextSelected = 588;

        public DropDownList(Control owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
        }

        public DropDownList(Control owner, int page, int x, int y, int width, int index, int itemsVisible, string[] items, bool canBeNull)
            : this(owner, page)
        {
            buildGumpling(x, y, width, index, itemsVisible, items, canBeNull);
        }

        void buildGumpling(int x, int y, int width, int index, int itemsVisible, string[] items, bool canBeNull)
        {
            Position = new Point2D(x, y);
            _items = new List<string>(items);
            _width = width;
            Index = index;
            _visibleItems = itemsVisible;
            _canBeNull = canBeNull;

            _resize = new ResizePic(_owner, Page, X, Y, 3000, _width, UltimaData.ASCIIText.Fonts[1].Height + 8);
            _resize.OnMouseClick = onClickClosedList;
            _resize.OnMouseOver = onMouseOverClosedList;
            _resize.OnMouseOut = onMouseOutClosedList;
            ((Gump)_owner).AddControl(_resize);
            _label = new TextLabelAscii(_owner, Page, X + 4, Y + 5, hue_Text, 1, string.Empty);
            ((Gump)_owner).AddControl(_label);
            ((Gump)_owner).AddControl(new GumpPic(_owner, Page, X + width - 22, Y + 5, 2086, 0));
        }

        public override void Update(GameTime gameTime)
        {
            if (_listOpen)
            {
                // if we have moused off the open list, close it. We check to see if the mouse is over:
                // the resizepic for the closed list (because it takes one update cycle to open the list)
                // the resizepic for the open list, and the scroll bar if it is loaded.
                if (UserInterface.MouseOverControl != _openResizePic &&
                    UserInterface.MouseOverControl != _resize &&
                    (_openScrollBar == null ? false : UserInterface.MouseOverControl != _openScrollBar))
                {
                    closeOpenList();
                }
                else
                {
                    // update the visible items
                    int itemOffset = (_openScrollBar == null ? 0 : _openScrollBar.Value);
                    for (int i = 0; i < _visibleItems; i++)
                    {
                        _openLabels[i].Text = (i + itemOffset < 0) ? string.Empty : _items[i + itemOffset];
                    }
                }
            }
            else
            {
                if (Index == -1)
                    _label.Text = "Click here";
                else
                    _label.Text = _items[Index];
            }
            base.Update(gameTime);
        }

        void closeOpenList()
        {
            _listOpen = false;
            _openResizePic.Dispose();
            if (_openScrollBar != null)
                _openScrollBar.Dispose();
            for (int i = 0; i < _visibleItems; i++)
                _openLabels[i].Dispose();
        }

        void onClickClosedList(int x, int y, MouseButton button)
        {
            _listOpen = true;
            _openResizePic = new ResizePic(_owner, Page, X, Y, 3000, _width, UltimaData.ASCIIText.Fonts[1].Height * _visibleItems + 8);
            _openResizePic.OnMouseClick = onClickOpenList;
            _openResizePic.OnMouseOver = onMouseOverOpenList;
            _openResizePic.OnMouseOut = onMouseOutOpenList;
            ((Gump)_owner).AddControl(_openResizePic);
            // only show the scrollbar if we need to scroll
            if (_visibleItems < _items.Count)
            {
                _openScrollBar = new ScrollBar(_owner, Page, X + _width - 20, Y + 4, UltimaData.ASCIIText.Fonts[1].Height * _visibleItems, (_canBeNull ? -1 : 0), _items.Count - _visibleItems, Index);
                ((Gump)_owner).AddControl(_openScrollBar);
            }
            _openLabels = new TextLabelAscii[_visibleItems];
            for (int i = 0; i < _visibleItems; i++)
            {
                _openLabels[i] = new TextLabelAscii(_owner, Page, X + 4, Y + 5 + UltimaData.ASCIIText.Fonts[1].Height * i, 1107, 1, string.Empty);
                ((Gump)_owner).AddControl(_openLabels[i]);
            }
        }

        void onMouseOverClosedList(int x, int y)
        {
            _label.Hue = hue_TextSelected;
        }

        void onMouseOutClosedList(int x, int y)
        {
            _label.Hue = hue_Text;
        }

        void onClickOpenList(int x, int y, MouseButton button)
        {
            int indexOver = getOpenListIndexFromPoint(x, y);
            if (indexOver != -1)
                Index = indexOver + (_openScrollBar == null ? 0 : _openScrollBar.Value);
            closeOpenList();
        }

        void onMouseOverOpenList(int x, int y)
        {
            int indexOver = getOpenListIndexFromPoint(x, y);
            for (int i = 0; i < _openLabels.Length; i++)
            {
                if (i == indexOver)
                    _openLabels[i].Hue = hue_TextSelected;
                else
                    _openLabels[i].Hue = hue_Text;
            }
        }

        void onMouseOutOpenList(int x, int y)
        {
            for (int i = 0; i < _openLabels.Length; i++)
                _openLabels[i].Hue = hue_Text;
        }

        int getOpenListIndexFromPoint(int x, int y)
        {
            Rectangle r = new Rectangle(4, 5, _width - 20, UltimaData.ASCIIText.Fonts[1].Height);
            for (int i = 0; i < _openLabels.Length; i++)
            {
                if (r.Contains(new Point(x, y)))
                    return i;
                r.Y += UltimaData.ASCIIText.Fonts[1].Height;
            }
            return -1;
        }
    }
}
