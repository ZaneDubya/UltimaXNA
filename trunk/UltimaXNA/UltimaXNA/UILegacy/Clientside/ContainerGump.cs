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
        Container _item;
        int _updateTicker = 0;

        public ContainerGump(Entity containerItem, int gumpID)
            : base(containerItem.Serial, 0)
        {
            _data = Data.ContainerData.GetData(gumpID);
            _item = (Container)containerItem;

            AddGumpling(new Gumplings.GumpPicContainer(this, 0, 0, 0, _data.GumpID, 0));
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            if (_updateTicker != _item.UpdateTicker)
            {
                _updateTicker = _item.UpdateTicker;
                removeAllItems();
                foreach (Item i in _item.Contents)
                {
                    AddGumpling(new ItemGumpling(this, i));
                }
            }
        }

        void removeAllItems()
        {
            _controls.RemoveAll(item => item is ItemGumpling);
        }
    }
}
