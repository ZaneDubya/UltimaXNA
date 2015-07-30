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

        private int m_PropertyListHash = 0;
        private AEntity m_Entity;

        public Tooltip(string caption)
        {
            m_Entity = null;
            Caption = caption;
        }

        public Tooltip(AEntity entity)
        {
            m_Entity = entity;
            m_PropertyListHash = m_Entity.PropertyList.Hash;
            Caption = m_Entity.PropertyList.Properties;
        }

        public void Dispose()
        {
            Caption = null;
        }

        public void Draw(SpriteBatchUI spriteBatch, int x, int y)
        {
            // determine if properties need to be updated.
            if (m_Entity != null && m_PropertyListHash != m_Entity.PropertyList.Hash)
            {
                m_PropertyListHash = m_Entity.PropertyList.Hash;
                Caption = m_Entity.PropertyList.Properties;
            }

            // update text if necessary.
            if (m_RenderedText == null)
            {
                m_RenderedText = new RenderedText("<center>" + Caption, 300);
            }
            else if (m_RenderedText.Text != "<center>" + Caption)
            {
                m_RenderedText = null;
                m_RenderedText = new RenderedText("<center>" + Caption, 300);
            }

            // draw checkered trans underneath.
            spriteBatch.Draw2DTiled(CheckerTrans.CheckeredTransTexture, new Rectangle(x - 4, y - 4, m_RenderedText.Width + 8, m_RenderedText.Height + 8), Vector3.Zero);
            // draw tooltip contents
            m_RenderedText.Draw(spriteBatch, new Point(x, y));
        }

        internal void UpdateEntity(AEntity entity)
        {
            if (m_Entity == null || m_Entity != entity || m_PropertyListHash != m_Entity.PropertyList.Hash)
            {
                m_Entity = entity;
                m_PropertyListHash = m_Entity.PropertyList.Hash;
                Caption = m_Entity.PropertyList.Properties;
            }
        }

        internal void UpdateCaption(string caption)
        {
            m_Entity = null;
            Caption = caption;
        }
    }
}
