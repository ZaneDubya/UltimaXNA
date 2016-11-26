/***************************************************************************
 *   GumpPicContainer.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.World.Entities.Items.Containers;

namespace UltimaXNA.Ultima.UI.Controls
{
    class GumpPicContainer : GumpPic
    {
        readonly ContainerItem m_containerItem;
        public ContainerItem Item { get { return m_containerItem; } }

        public GumpPicContainer(AControl parent, int x, int y, int gumpID, int hue, ContainerItem containerItem)
            : base(parent, x, y, gumpID, hue)
        {
            m_containerItem = containerItem;
        }
    }
}
