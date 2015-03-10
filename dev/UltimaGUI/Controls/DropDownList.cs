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
using InterXLib.Input.Windows;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaData.Fonts;

namespace UltimaXNA.UltimaGUI.Controls
{
    class DropDownList : Control
    {
        public int Index;

        int m_width;
        List<string> m_items;
        int m_visibleItems;
        bool m_canBeNull;

        ResizePic m_resize;
        TextLabelAscii m_label;

        bool m_listOpen = false;
        ResizePic m_openResizePic;
        ScrollBar m_openScrollBar;
        TextLabelAscii[] m_openLabels;

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
            Position = new Point(x, y);
            m_items = new List<string>(items);
            m_width = width;
            Index = index;
            m_visibleItems = itemsVisible;
            m_canBeNull = canBeNull;

            m_resize = new ResizePic(m_owner, Page, X, Y, 3000, m_width, UltimaData.Fonts.ASCIIText.Fonts[1].Height + 8);
            m_resize.OnMouseClick = onClickClosedList;
            m_resize.OnMouseOver = onMouseOverClosedList;
            m_resize.OnMouseOut = onMouseOutClosedList;
            ((Gump)m_owner).AddControl(m_resize);
            m_label = new TextLabelAscii(m_owner, Page, X + 4, Y + 5, hue_Text, 1, string.Empty);
            ((Gump)m_owner).AddControl(m_label);
            ((Gump)m_owner).AddControl(new GumpPic(m_owner, Page, X + width - 22, Y + 5, 2086, 0));
        }

        public override void Update(GameTime gameTime)
        {
            if (m_listOpen)
            {
                // if we have moused off the open list, close it. We check to see if the mouse is over:
                // the resizepic for the closed list (because it takes one update cycle to open the list)
                // the resizepic for the open list, and the scroll bar if it is loaded.
                if (UserInterface.MouseOverControl != m_openResizePic &&
                    UserInterface.MouseOverControl != m_resize &&
                    (m_openScrollBar == null ? false : UserInterface.MouseOverControl != m_openScrollBar))
                {
                    closeOpenList();
                }
                else
                {
                    // update the visible items
                    int itemOffset = (m_openScrollBar == null ? 0 : m_openScrollBar.Value);
                    for (int i = 0; i < m_visibleItems; i++)
                    {
                        m_openLabels[i].Text = (i + itemOffset < 0) ? string.Empty : m_items[i + itemOffset];
                    }
                }
            }
            else
            {
                if (Index == -1)
                    m_label.Text = "Click here";
                else
                    m_label.Text = m_items[Index];
            }
            base.Update(gameTime);
        }

        void closeOpenList()
        {
            m_listOpen = false;
            m_openResizePic.Dispose();
            if (m_openScrollBar != null)
                m_openScrollBar.Dispose();
            for (int i = 0; i < m_visibleItems; i++)
                m_openLabels[i].Dispose();
        }

        void onClickClosedList(int x, int y, MouseButton button)
        {
            m_listOpen = true;
            m_openResizePic = new ResizePic(m_owner, Page, X, Y, 3000, m_width, ASCIIText.Fonts[1].Height * m_visibleItems + 8);
            m_openResizePic.OnMouseClick = onClickOpenList;
            m_openResizePic.OnMouseOver = onMouseOverOpenList;
            m_openResizePic.OnMouseOut = onMouseOutOpenList;
            ((Gump)m_owner).AddControl(m_openResizePic);
            // only show the scrollbar if we need to scroll
            if (m_visibleItems < m_items.Count)
            {
                m_openScrollBar = new ScrollBar(m_owner, Page, X + m_width - 20, Y + 4, ASCIIText.Fonts[1].Height * m_visibleItems, (m_canBeNull ? -1 : 0), m_items.Count - m_visibleItems, Index);
                ((Gump)m_owner).AddControl(m_openScrollBar);
            }
            m_openLabels = new TextLabelAscii[m_visibleItems];
            for (int i = 0; i < m_visibleItems; i++)
            {
                m_openLabels[i] = new TextLabelAscii(m_owner, Page, X + 4, Y + 5 + ASCIIText.Fonts[1].Height * i, 1107, 1, string.Empty);
                ((Gump)m_owner).AddControl(m_openLabels[i]);
            }
        }

        void onMouseOverClosedList(int x, int y)
        {
            m_label.Hue = hue_TextSelected;
        }

        void onMouseOutClosedList(int x, int y)
        {
            m_label.Hue = hue_Text;
        }

        void onClickOpenList(int x, int y, MouseButton button)
        {
            int indexOver = getOpenListIndexFromPoint(x, y);
            if (indexOver != -1)
                Index = indexOver + (m_openScrollBar == null ? 0 : m_openScrollBar.Value);
            closeOpenList();
        }

        void onMouseOverOpenList(int x, int y)
        {
            int indexOver = getOpenListIndexFromPoint(x, y);
            for (int i = 0; i < m_openLabels.Length; i++)
            {
                if (i == indexOver)
                    m_openLabels[i].Hue = hue_TextSelected;
                else
                    m_openLabels[i].Hue = hue_Text;
            }
        }

        void onMouseOutOpenList(int x, int y)
        {
            for (int i = 0; i < m_openLabels.Length; i++)
                m_openLabels[i].Hue = hue_Text;
        }

        int getOpenListIndexFromPoint(int x, int y)
        {
            Rectangle r = new Rectangle(4, 5, m_width - 20, ASCIIText.Fonts[1].Height);
            for (int i = 0; i < m_openLabels.Length; i++)
            {
                if (r.Contains(new Point(x, y)))
                    return i;
                r.Y += ASCIIText.Fonts[1].Height;
            }
            return -1;
        }
    }
}
