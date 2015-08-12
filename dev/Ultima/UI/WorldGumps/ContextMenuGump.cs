/***************************************************************************
 *   ContextMenuGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Text;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI.Fonts;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Resources.Fonts;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class ContextMenuGump : Gump
    {
        private readonly ContextMenuData m_Data;

        private ResizePic m_Background;
        private HtmlGumpling m_MenuItems;

        public ContextMenuGump(ContextMenuData data)
            : base(0, 0)
        {
            m_Data = data;

            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            AFont font = (AFont)provider.GetUnicodeFont(1);

            m_Background = (ResizePic)AddControl(new ResizePic(this, 0, 0, 0x0A3C, 50, font.Height * m_Data.Count + 20));

            StringBuilder htmlContextItems = new StringBuilder();
            for (int i = 0; i < m_Data.Count; i++)
            {
                htmlContextItems.Append(string.Format("<a href='{0}' color='#DDD' hovercolor='#FFF' activecolor='#BBB' style='text-decoration:none;'>{1}</a><br/>", m_Data[i].ResponseCode, m_Data[i].Caption));
            }
            m_MenuItems = (HtmlGumpling)AddControl(new HtmlGumpling(this, 10, 10, 200, font.Height * m_Data.Count, 0, 0, htmlContextItems.ToString()));
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            m_Background.Width = m_MenuItems.Width + 20;
        }

        protected override void OnMouseOut(int x, int y)
        {
            Dispose();
        }

        public override void ActivateByHREF(string href)
        {
            int contextMenuItemSelected;
            if (int.TryParse(href, out contextMenuItemSelected))
            {
                INetworkClient network = ServiceRegistry.GetService<INetworkClient>();
                network.Send(new ContextMenuResponsePacket(m_Data.Serial, (short)contextMenuItemSelected));
                this.Dispose();
            }
        }
    }
}
