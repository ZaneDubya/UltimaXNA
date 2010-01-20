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
        HtmlGump _tickerText;
        int _updateTicker = 0;
        public Serial ContainerSerial { get { return _item.Serial; } }

        public ContainerGump(Entity containerItem, int gumpID)
            : base(containerItem.Serial, 0)
        {
            _data = Data.ContainerData.GetData(gumpID);
            _item = (Container)containerItem;

            AddGumpling(new Gumplings.GumpPicContainer(this, 0, 0, 0, _data.GumpID, 0));
            _tickerText = (HtmlGump)AddGumpling(new HtmlGump(this, 0, 50, 50, 50, 50, 0, 0, string.Empty));
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            if (_updateTicker != _item.UpdateTicker)
            {
                _updateTicker = _item.UpdateTicker;
                _tickerText.Text = _updateTicker.ToString();
                // delete any items in our pack that are no longer in the container.
                List<Control> gumplingsToRemove = new List<Control>();
                foreach (Control c in _controls)
                {
                    if (c is ItemGumpling && !_item.Contents.Contains(((ItemGumpling)c).Item))
                    {
                        gumplingsToRemove.Add(c);
                    }
                }
                foreach (Control c in gumplingsToRemove)
                    _controls.Remove(c);
                // add any items in the container that are not in our pack.
                foreach (Item item in _item.Contents)
                {
                    bool controlForThisItem = false;
                    foreach (Control c in _controls)
                    {
                        if (c is ItemGumpling && ((ItemGumpling)c).Item == item)
                        {
                            controlForThisItem = true;
                            break;
                        }
                    }
                    if (!controlForThisItem)
                    {
                        AddGumpling(new ItemGumpling(this, item));
                    }
                }
            }
        }
    }
}
