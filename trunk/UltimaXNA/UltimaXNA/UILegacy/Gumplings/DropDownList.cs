using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;

namespace UltimaXNA.UILegacy.Gumplings
{
    class DropDownList : Control
    {
        int _width;
        List<string> _items;
        int Index;
        int _visibleItems;

        ResizePic _resize;
        TextLabelAscii _label;

        bool _listOpen = false;
        ResizePic _openResizePic;
        ScrollBar _openScrollBar;
        TextLabelAscii[] _openLabels;

        public DropDownList(Control owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
            _controls = new List<Control>();
        }

        public DropDownList(Control owner, int page, int x, int y, int width, int index, int itemsVisible, string[] items)
            : this(owner, page)
        {
            buildGumpling(x, y, width, index, itemsVisible, items);
        }

        void buildGumpling(int x, int y, int width, int index, int itemsVisible, string[] items)
        {
            Position = new Vector2(x, y);
            _items = new List<string>(items);
            _width = width;
            Index = index;
            _visibleItems = itemsVisible;

            _resize = new ResizePic(_owner, Page, X, Y, 3000, _width, Data.ASCIIText.Fonts[1].Height + 8);
            _resize.OnMouseClick = onClickClosedList;
            ((Gump)_owner).AddGumpling(_resize);
            _label = new TextLabelAscii(this, Page, X + 4, Y + 5, 1107, 1, string.Empty);
            ((Gump)_owner).AddGumpling(_label);
        }

        public override void Update(GameTime gameTime)
        {
            if (_listOpen)
            {
                // if we have moused off the open list, close it without ceremony.
                if (_manager.MouseOverControl != _openResizePic && _manager.MouseOverControl != _openScrollBar && _manager.MouseOverControl != _resize)
                {
                    closeOpenList();
                }
                else
                {
                    // update the visible items
                    for (int i = 0; i < _visibleItems; i++)
                    {
                        _openLabels[i].Text = (i + _openScrollBar.Value < 0) ? string.Empty : _items[i + _openScrollBar.Value];
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
            _openScrollBar.Dispose();
            for (int i = 0; i < _visibleItems; i++)
                _openLabels[i].Dispose();
        }

        void onClickClosedList(int x, int y, MouseButtons button)
        {
            _listOpen = true;
            _openResizePic = new ResizePic(_owner, Page, X, Y, 3000, _width, Data.ASCIIText.Fonts[1].Height * 8 + 8);
            _openResizePic.OnMouseClick = onClickOpenList;
            ((Gump)_owner).AddGumpling(_openResizePic);
            _openScrollBar = new ScrollBar(_owner, Page, X + _width - 20, Y + 4, Data.ASCIIText.Fonts[1].Height * 8, -1, _items.Count - _visibleItems, Index);
            ((Gump)_owner).AddGumpling(_openScrollBar);
            _openLabels = new TextLabelAscii[_visibleItems];
            for (int i = 0; i < _visibleItems; i++)
            {
                _openLabels[i] = new TextLabelAscii(_owner, Page, X + 4, Y + 5 + Data.ASCIIText.Fonts[1].Height * i, 1107, 1, string.Empty);
                ((Gump)_owner).AddGumpling(_openLabels[i]);
            }
        }

        void onClickOpenList(int x, int y, MouseButtons button)
        {
            closeOpenList();
        }
    }
}
