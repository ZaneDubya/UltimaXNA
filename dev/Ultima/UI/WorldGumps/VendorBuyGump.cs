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
        private ExpandableScroll m_ScrollBackground;
        private HtmlGumpling m_ShopContents;

        public VendorBuyGump(AEntity vendorBackpack, VendorBuyListPacket packet)
            : base(0, 0)
        {
            IsMoveable = true;
            // note: original gumplings start at index 0x0870.
            AddControl(m_ScrollBackground = new ExpandableScroll(this, 0, 0, 320, false));
            AddControl(new HtmlGumpling(this, 0, 6, 300, 20, 0, 0, " <center><span color='#004' style='font-family:uni0;'>Shop Inventory"));
            AddControl(m_ShopContents = new HtmlGumpling(this, 28, 32, 244, 254, 0, 2, BuildShopContentsHtml(vendorBackpack, packet)));
        }

        private string BuildShopContentsHtml(AEntity vendorBackpack, VendorBuyListPacket packet)
        {
            if (!(vendorBackpack is Container))
                return "<span color='#800'>Err: vendorBackpack is not Container.";

            Container contents = (vendorBackpack as Container);
            if (contents.Contents.Count != packet.Items.Count)
                return "<span color='#800'>Err: vendorBackpack item count and packet item count do not match.";

            StringBuilder html = new StringBuilder();
            html.Append("<span color='#001'>");

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
                    description = Ultima.IO.StringData.Entry(clilocDescription);
                }

                 
                html.Append(string.Format("<itemimg src='{2}'/>{0}<right>{1}gp</right><br/>", 
                    description, price.ToString(), item.DisplayItemID));
            }

            return html.ToString();
        }
    }
}
