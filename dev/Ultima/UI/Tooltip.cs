/***************************************************************************
 *   Tooltip.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
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

namespace UltimaXNA.Ultima.UI
{
    class Tooltip
    {
        public string Caption
        {
            get;
            protected set;
        }

        private RenderedText m_RenderedText;

        private bool m_IsPropertyList = false;
        private int m_PropertyListHash = 0;
        private AEntity m_Entity;

        public Tooltip(string caption)
        {
            Caption = caption;
            m_IsPropertyList = false;
        }

        public Tooltip(AEntity entity)
        {
            m_IsPropertyList = true;
            m_Entity = entity;
            m_PropertyListHash = m_Entity.PropertyList.Hash;
            Caption = "<center>" + m_Entity.PropertyList.Properties;
        }

        public void Dispose()
        {
            Caption = null;
        }

        public void Draw(SpriteBatchUI spriteBatch, int x, int y)
        {
            // determine if properties need to be updated.
            if (m_IsPropertyList && m_PropertyListHash != m_Entity.PropertyList.Hash)
            {
                m_PropertyListHash = m_Entity.PropertyList.Hash;
                Caption = "<center>" + m_Entity.PropertyList.Properties;
            }

            // update text if necessary.
            if (m_RenderedText == null)
            {
                m_RenderedText = new RenderedText(Caption, 200);
            }
            else if (m_RenderedText.Text != Caption)
            {
                m_RenderedText = null;
                m_RenderedText = new RenderedText(Caption, 200);
            }

            // draw checkered trans underneath.
            spriteBatch.Draw2DTiled(CheckerTrans.CheckeredTransTexture, new Rectangle(x - 4, y - 4, m_RenderedText.Width + 8, m_RenderedText.Height + 8), Vector3.Zero);
            // draw tooltip contents
            m_RenderedText.Draw(spriteBatch, new Point(x, y));
        }
    }
}
