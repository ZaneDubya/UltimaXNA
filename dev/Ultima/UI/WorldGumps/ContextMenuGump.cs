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
using System.Collections.Generic;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Core.UI.Fonts;
using UltimaXNA.Ultima.IO.Fonts;
using System;
using System.Text;
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

            AFont font = TextUni.GetUniFont(1);

            m_Background = (ResizePic)AddControl(new ResizePic(this, 0, 0, 0x0A3C, 50, font.Height * m_Data.Count + 20));

            StringBuilder htmlContextItems = new StringBuilder();
            for (int i = 0; i < m_Data.Count; i++)
            {
                htmlContextItems.Append(string.Format("<a href='{0}' style='font-decoration:none;'>{1}</a>\n", m_Data[i].ResponseCode, m_Data[i].Caption));
            }
            m_MenuItems = (HtmlGumpling)AddControl(new HtmlGumpling(this, 10, 10, 200, font.Height * m_Data.Count, 0, 0, htmlContextItems.ToString()));
        }

        protected override void OnInitialize()
        {
            m_Background.Width = m_MenuItems.Width + 20;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }
    }
}
