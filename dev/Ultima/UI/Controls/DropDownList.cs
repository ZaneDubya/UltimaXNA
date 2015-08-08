/***************************************************************************
 *   DropDownList.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    class DropDownList : AControl
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

        UserInterfaceService m_UserInterface;
        IFont m_Font;

        public DropDownList(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_Font = ServiceRegistry.GetService<IResourceProvider>().GetAsciiFont(1);
        }

        public DropDownList(AControl parent, int x, int y, int width, string[] items, int itemsVisible, int index, bool canBeNull)
            : this(parent)
        {
            buildGumpling(x, y, width, items, itemsVisible, index, canBeNull);
        }

        void buildGumpling(int x, int y, int width, string[] items, int itemsVisible, int index, bool canBeNull)
        {
            Position = new Point(x, y);
            m_items = new List<string>(items);
            m_width = width;
            Index = index;
            m_visibleItems = itemsVisible;
            m_canBeNull = canBeNull;

            m_resize = (ResizePic)AddControl(new ResizePic(this, 0, 0, 3000, m_width, m_Font.Height + 8), 0);
            m_resize.MouseClickEvent += onClickClosedList;
            m_resize.MouseOverEvent += onMouseOverClosedList;
            m_resize.MouseOutEvent += onMouseOutClosedList;
            m_label = (TextLabelAscii)AddControl(new TextLabelAscii(this, 4, 5, hue_Text, 1, string.Empty), 0);
            AddControl(new GumpPic(this, width - 22, 5, 2086, 0), 0);
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (Index < 0 || Index >= m_items.Count)
                Index = -1;

            if (m_listOpen)
            {
                // if we have moused off the open list, close it. We check to see if the mouse is over:
                // the resizepic for the closed list (because it takes one update cycle to open the list)
                // the resizepic for the open list, and the scroll bar if it is loaded.
                if (m_UserInterface.MouseOverControl != m_openResizePic &&
                    m_UserInterface.MouseOverControl != m_resize &&
                    (m_openScrollBar == null ? false : m_UserInterface.MouseOverControl != m_openScrollBar))
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
            base.Update(totalMS, frameMS);
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

        void onClickClosedList(AControl control, int x, int y, MouseButton button)
        {
            m_listOpen = true;
            m_openResizePic = new ResizePic(Parent, X, Y, 3000, m_width, m_Font.Height * m_visibleItems + 8);
            m_openResizePic.MouseClickEvent += onClickOpenList;
            m_openResizePic.MouseOverEvent += onMouseOverOpenList;
            m_openResizePic.MouseOutEvent += onMouseOutOpenList;
            ((Gump)Parent).AddControl(m_openResizePic, this.Page);

            if (m_visibleItems > m_items.Count)
            {
                m_visibleItems = m_items.Count;
            }

            // only show the scrollbar if we need to scroll
            if (m_visibleItems < m_items.Count)
            {
                m_openScrollBar = new ScrollBar(Parent, X + m_width - 20, Y + 4, m_Font.Height * m_visibleItems, (m_canBeNull ? -1 : 0), m_items.Count - m_visibleItems, Index);
                ((Gump)Parent).AddControl(m_openScrollBar, this.Page);
            }
            m_openLabels = new TextLabelAscii[m_visibleItems];
            for (int i = 0; i < m_visibleItems; i++)
            {
                m_openLabels[i] = new TextLabelAscii(Parent, X + 4, Y + 5 + m_Font.Height * i, 1106, 1, string.Empty);
                ((Gump)Parent).AddControl(m_openLabels[i], this.Page);
            }
        }

        void onMouseOverClosedList(AControl control, int x, int y)
        {
            m_label.Hue = hue_TextSelected;
        }

        void onMouseOutClosedList(AControl control, int x, int y)
        {
            m_label.Hue = hue_Text;
        }

        void onClickOpenList(AControl control, int x, int y, MouseButton button)
        {
            int indexOver = getOpenListIndexFromPoint(x, y);
            if (indexOver != -1)
                Index = indexOver + (m_openScrollBar == null ? 0 : m_openScrollBar.Value);
            closeOpenList();
        }

        void onMouseOverOpenList(AControl control, int x, int y)
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

        void onMouseOutOpenList(AControl control, int x, int y)
        {
            for (int i = 0; i < m_openLabels.Length; i++)
                m_openLabels[i].Hue = hue_Text;
        }

        int getOpenListIndexFromPoint(int x, int y)
        {
            Rectangle r = new Rectangle(4, 5, m_width - 20, m_Font.Height);
            for (int i = 0; i < m_openLabels.Length; i++)
            {
                if (r.Contains(new Point(x, y)))
                    return i;
                r.Y += m_Font.Height;
            }
            return -1;
        }
    }
}
