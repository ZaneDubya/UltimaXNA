using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Data;
using UltimaXNA.Entities;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.Clientside
{
    class ContainerGump : Gump
    {
        ContainerData _data;
        ContainerItem _item;

        public ContainerGump(Entity containerItem, int gumpID)
            : base(containerItem.Serial, 0)
        {
            _data = Data.ContainerData.GetData(gumpID);
            _item = (ContainerItem)containerItem;

            AddGumpling(new Gumplings.GumpPicContainer(this, 0, 0, 0, _data.GumpID, 0));
        }
    }
}
