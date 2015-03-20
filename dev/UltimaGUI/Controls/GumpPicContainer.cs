/***************************************************************************
 *   GumpPicContainer.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Entity;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class GumpPicContainer : GumpPic
    {
        Container m_containerItem;
        public Container Item { get { return m_containerItem; } }

        public GumpPicContainer(Control owner, int page, int x, int y, int gumpID, int hue, Container containerItem)
            : base(owner, page, x, y, gumpID, hue)
        {
            m_containerItem = containerItem;
        }
    }
}
