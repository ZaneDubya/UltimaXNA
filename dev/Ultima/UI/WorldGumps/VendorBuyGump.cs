/***************************************************************************
 *   VendorBuyGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.Network.Server;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
#endregion


namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class VendorBuyGump : Gump
    {
        private ExpandableScroll m_Background;
        private IScrollBar m_ScrollBar;
        private RenderedTextList m_ShopContents;

        public VendorBuyGump(AEntity vendorBackpack, VendorBuyListPacket packet)
            : base(0, 0)
        {
            IsMoveable = true;
            // note: original gumplings start at index 0x0870.
            AddControl(m_Background = new ExpandableScroll(this, 0, 0, 360, false));
            AddControl(new HtmlGumpling(this, 0, 6, 300, 20, 0, 0, " <center><span color='#004' style='font-family:uni0;'>Shop Inventory"));

            m_ScrollBar = (IScrollBar)AddControl(new ScrollFlag(this));
            AddControl(m_ShopContents = new RenderedTextList(this, 22, 36, 250, 260, m_ScrollBar));

            BuildShopContentsHtml(vendorBackpack, packet);
        }

        public override void Update(double totalMS, double frameMS)
        {
            m_ShopContents.Height = Height - 69;
            base.Update(totalMS, frameMS);
        }

        private void BuildShopContentsHtml(AEntity vendorBackpack, VendorBuyListPacket packet)
        {
            if (!(vendorBackpack is Container))
            {
                m_ShopContents.AddEntry("<span color='#800'>Err: vendorBackpack is not Container.");
                return;
            }

            Container contents = (vendorBackpack as Container);
            if (contents.Contents.Count != packet.Items.Count)
            {
                m_ShopContents.AddEntry("<span color='#800'>Err: vendorBackpack item count and packet item count do not match.");
                return;
            }

            for (int i = 0; i < packet.Items.Count; i++)
            {
                Item item = contents.Contents[packet.Items.Count - 1 - i];
                string cliLocAsString = packet.Items[i].Description;
                int price = packet.Items[i].Price;

                int clilocDescription;
                string description;
                if (!(int.TryParse(cliLocAsString, out clilocDescription)))
                {
                    description = cliLocAsString;
                }
                else
                {
                    description = Utility.CapitalizeAllWords(Ultima.IO.StringData.Entry(clilocDescription));
                }

                string html = string.Format("<right><a href='add={4}'><gumpimg src='0x9CF'/></a><span width='4'/><a href='remove={4}'><gumpimg src='0x9CE'/></a></right><span width='54'/><span color='#400'>{0}, {1}gp<br/><itemimg src='{2}' width='52' height='44' style='top: -18;'/>{3} available.</span><br/>", 
                    description, price.ToString(), item.DisplayItemID, item.Amount, i);
                m_ShopContents.AddEntry(html);
            }

            // list starts displaying first item.
            m_ScrollBar.Value = 0;
        }

        public override void ActivateByHREF(string href)
        {

        }
    }
}
