/***************************************************************************
 *   ContainerGump.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using UltimaXNA.Entity;
using UltimaXNA.Interface.GUI;
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaGUI.Gumplings;

namespace UltimaXNA.UltimaGUI.ClientsideGumps
{
    class ContainerGump : Gump
    {
        ContainerData _data;
        Container _item;
        HtmlGump _tickerText;
        int _updateTicker = 0;
        public Serial ContainerSerial { get { return _item.Serial; } }

        public ContainerGump(BaseEntity containerItem, int gumpID)
            : base(containerItem.Serial, 0)
        {
            _data = UltimaData.ContainerData.GetData(gumpID);
            _item = (Container)containerItem;
            IsMovable = true;

            AddControl(new GumpPicContainer(this, 0, 0, 0, _data.GumpID, 0, _item));
            LastControl.MakeDragger(this);
            LastControl.MakeCloseTarget(this);

            _tickerText = (HtmlGump)AddControl(new HtmlGump(this, 0, 50, 50, 0, 0, 0, 0, string.Empty));
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            if (_updateTicker != _item.UpdateTicker)
            {
                _updateTicker = _item.UpdateTicker;
                _tickerText.Text = string.Format("Update#{0}", _updateTicker);
                // delete any items in our pack that are no longer in the container.
                List<Control> gumplingsToRemove = new List<Control>();
                foreach (Control c in Controls)
                {
                    if (c is ItemGumpling && !_item.Contents.Contains(((ItemGumpling)c).Item))
                    {
                        gumplingsToRemove.Add(c);
                    }
                }
                foreach (Control c in gumplingsToRemove)
                    Controls.Remove(c);
                // add any items in the container that are not in our pack.
                foreach (Item item in _item.Contents)
                {
                    bool controlForThisItem = false;
                    foreach (Control c in Controls)
                    {
                        if (c is ItemGumpling && ((ItemGumpling)c).Item == item)
                        {
                            controlForThisItem = true;
                            break;
                        }
                    }
                    if (!controlForThisItem)
                    {
                        AddControl(new ItemGumpling(this, item));
                    }
                }
            }
        }
    }
}
