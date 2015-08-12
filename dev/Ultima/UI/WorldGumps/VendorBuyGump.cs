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
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.Network.Server;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class VendorBuyGump : Gump
    {
        private ExpandableScroll m_Background;
        private IScrollBar m_ScrollBar;
        private HtmlGumpling m_TotalCost;

        private int m_VendorSerial;
        private VendorItemInfo[] m_Items;
        private RenderedTextList m_ShopContents;

        public VendorBuyGump(AEntity vendorBackpack, VendorBuyListPacket packet)
            : base(0, 0)
        {
            // sanity checking: don't show buy gumps for empty containers.
            if (!(vendorBackpack is Container) || ((vendorBackpack as Container).Contents.Count <= 0) || (packet.Items.Count <= 0))
            {
                Dispose();
                return;
            }

            IsMoveable = true;
            // note: original gumplings start at index 0x0870.
            AddControl(m_Background = new ExpandableScroll(this, 0, 0, 360, false));
            AddControl(new HtmlGumpling(this, 0, 6, 300, 20, 0, 0, " <center><span color='#004' style='font-family:uni0;'>Shop Inventory"));

            m_ScrollBar = (IScrollBar)AddControl(new ScrollFlag(this));
            AddControl(m_ShopContents = new RenderedTextList(this, 22, 32, 250, 260, m_ScrollBar));
            BuildShopContents(vendorBackpack, packet);

            AddControl(m_TotalCost = new HtmlGumpling(this, 44, 334, 200, 30, 0, 0, string.Empty));
            UpdateCost();

            Button okButton = (Button)AddControl(new Button(this, 220, 333, 0x907, 0x908, ButtonTypes.Activate, 0, 0));
            okButton.GumpOverID = 0x909;
            okButton.MouseClickEvent += okButton_MouseClickEvent;
        }

        void okButton_MouseClickEvent(AControl control, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;

            List<Tuple<int, short>> itemsToBuy = new List<Tuple<int, short>>();
            for (int i = 0; i < m_Items.Length; i++)
            {
                if (m_Items[i].AmountToBuy > 0)
                {
                    itemsToBuy.Add(new Tuple<int, short>(m_Items[i].Item.Serial, (short)m_Items[i].AmountToBuy));
                }
            }

            if (itemsToBuy.Count == 0)
                return;

            INetworkClient network = ServiceRegistry.GetService<INetworkClient>();
            network.Send(new BuyItemsPacket(m_VendorSerial, itemsToBuy.ToArray()));
            this.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            m_ShopContents.Height = Height - 69;
            base.Update(totalMS, frameMS);
        }

        private void BuildShopContents(AEntity vendorBackpack, VendorBuyListPacket packet)
        {

            if (!(vendorBackpack is Container))
            {
                m_ShopContents.AddEntry("<span color='#800'>Err: vendorBackpack is not Container.");
                return;
            }

            Container contents = (vendorBackpack as Container);
            AEntity vendor = contents.Parent;
            if (vendor == null || !(vendor is Mobile))
            {
                m_ShopContents.AddEntry("<span color='#800'>Err: vendorBackpack item does not belong to a vendor Mobile.");
                return;
            }
            m_VendorSerial = vendor.Serial;

            m_Items = new VendorItemInfo[packet.Items.Count];

            for (int i = 0; i < packet.Items.Count; i++)
            {
                Item item = contents.Contents[packet.Items.Count - 1 - i];
                if (item.Amount > 0)
                {
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
                        // get the resource provider
                        IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                        description = Utility.CapitalizeAllWords(provider.GetString(clilocDescription));
                    }

                    string html = string.Format(c_Format, description, price.ToString(), item.DisplayItemID, item.Amount, i);
                    m_ShopContents.AddEntry(html);

                    m_Items[i] = new VendorItemInfo(item, description, price, item.Amount);
                }
            }

            // list starts displaying first item.
            m_ScrollBar.Value = 0;
        }

        private const string c_Format = 
            "<right><a href='add={4}'><gumpimg src='0x9CF'/></a><span width='4'/><a href='remove={4}'><gumpimg src='0x9CE'/></a></right>" +
            "<itemimg src='{2}' style='layer: background; top: 0; left: 0;'/><span color='#400'>{0}<br/>{1}gp, {3} available.</span>";

        public override void ActivateByHREF(string href)
        {
            string[] hrefs = href.Split('=');
            bool isAdd;
            int index;
            if (hrefs[0] == "add")
            {
                isAdd = true;
            }
            else if (hrefs[0] == "remove")
            {
                isAdd = false;
            }
            else
            {
                Tracer.Error("Bad HREF in VendorBuyGump: {0}", href);
                return;
            }

            if (!(int.TryParse(hrefs[1], out index)))
            {
                Tracer.Error("Unknown vendor item index in VendorBuyGump: {0}", href);
                return;
            }

            if (isAdd)
            {
                if (m_Items[index].AmountToBuy < m_Items[index].AmountTotal)
                    m_Items[index].AmountToBuy++;
            }
            else
            {
                if (m_Items[index].AmountToBuy > 0)
                    m_Items[index].AmountToBuy--;
            }

            m_ShopContents.UpdateEntry(index, string.Format(c_Format, 
                m_Items[index].Description, 
                m_Items[index].Price.ToString(), 
                m_Items[index].Item.DisplayItemID,
                m_Items[index].AmountTotal - m_Items[index].AmountToBuy, index));
            UpdateCost();
        }

        private void UpdateCost()
        {
            int totalCost = 0;
            if (m_Items != null)
            {
                for (int i = 0; i < m_Items.Length; i++)
                {
                    totalCost += m_Items[i].AmountToBuy * m_Items[i].Price;
                }
            }
            m_TotalCost.Text = string.Format("<span style='font-family:uni0;' color='#008'>Total: </span><span color='#400'>{0}gp</span>", totalCost);
        }

        private class VendorItemInfo
        {
            public readonly Item Item;
            public readonly string Description;
            public readonly int Price;
            public readonly int AmountTotal;
            public int AmountToBuy;

            public VendorItemInfo(Item item, string description, int price, int amount)
            {
                Item = item;
                Description = description;
                Price = price;
                AmountTotal = amount;
                AmountToBuy = 0;
            }
        }
    }
}
