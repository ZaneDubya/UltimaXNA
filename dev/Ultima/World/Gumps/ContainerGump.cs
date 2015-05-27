/***************************************************************************
 *   ContainerGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Ultima.Entities.Items.Containers;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.World.Gumps
{
    class ContainerGump : Gump
    {
        ContainerData m_data;
        Container m_item;
        HtmlGump m_tickerText;
        int m_updateTicker = 0;
        public Serial ContainerSerial { get { return m_item.Serial; } }

        public ContainerGump(AEntity containerItem, int gumpID)
            : base(containerItem.Serial, 0)
        {
            m_data = IO.ContainerData.GetData(gumpID);
            m_item = (Container)containerItem;
            IsMovable = true;

            AddControl(new GumpPicContainer(this, 0, 0, 0, m_data.GumpID, 0, m_item));

            m_tickerText = (HtmlGump)AddControl(new HtmlGump(this, 0, 50, 50, 0, 0, 0, 0, string.Empty));
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            if (m_updateTicker != m_item.UpdateTicker)
            {
                m_updateTicker = m_item.UpdateTicker;
                m_tickerText.Text = string.Format("Update#{0}", m_updateTicker);
                // delete any items in our pack that are no longer in the container.
                List<AControl> ControlsToRemove = new List<AControl>();
                foreach (AControl c in Children)
                {
                    if (c is ItemGumpling && !m_item.Contents.Contains(((ItemGumpling)c).Item))
                    {
                        ControlsToRemove.Add(c);
                    }
                }
                foreach (AControl c in ControlsToRemove)
                    Children.Remove(c);
                // add any items in the container that are not in our pack.
                foreach (Item item in m_item.Contents)
                {
                    bool controlForThisItem = false;
                    foreach (AControl c in Children)
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
