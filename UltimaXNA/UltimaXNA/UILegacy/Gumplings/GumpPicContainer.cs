using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;

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

        protected override void itemDrop(UltimaXNA.Entities.Item item, int x, int y)
        {
            Interaction.DropItemToContainer(item, _containerItem, x, y);
        }
    }
}
