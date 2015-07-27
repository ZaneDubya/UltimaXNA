/***************************************************************************
 *   TooltipGump.cs
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
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.IO.Fonts;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class TooltipGump : Gump
    {
        private AEntity m_Entity;
        private int m_PropertyHash = int.MinValue;
        private HtmlGumpling m_PropertyListGumpling;
        private CheckerTrans m_Background;

        public TooltipGump(AEntity entity)
            : base(0, 0)
        {
            m_Entity = entity;
            m_PropertyHash = m_Entity.PropertyList.Hash;

            m_PropertyListGumpling = new HtmlGumpling(this, 0, 0, 200, 200, 0, 0, m_Entity.PropertyList.Properties);


            m_Background = new CheckerTrans(this, 0, 0, 0, 0);
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_PropertyHash != m_Entity.PropertyList.Hash)
            {
                m_PropertyHash = m_Entity.PropertyList.Hash;
                m_PropertyListGumpling.Text = m_Entity.PropertyList.Properties;
            }
            m_Background.Width = m_PropertyListGumpling.Width;
            m_Background.Height = m_PropertyListGumpling.Height;

            base.Update(totalMS, frameMS);
        }
    }
}
