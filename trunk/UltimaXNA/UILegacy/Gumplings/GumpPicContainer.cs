/***************************************************************************
 *   GumpPicContainer.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Entity;

namespace UltimaXNA.UILegacy.Gumplings
{
    class GumpPicContainer : GumpPic
    {
        Container _containerItem;

        public GumpPicContainer(Control owner, int page, int x, int y, int gumpID, int hue, Container containerItem)
            : base(owner, page, x, y, gumpID, hue)
        {
            _containerItem = containerItem;
        }

        protected override void itemDrop(Item item, int x, int y)
        {
            Interaction.DropItemToContainer(item, _containerItem, x, y);
        }
    }
}
