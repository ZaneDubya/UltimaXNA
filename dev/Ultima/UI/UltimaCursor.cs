/***************************************************************************
 *   UltimaCursor.cs
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
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources;
#endregion

namespace UltimaXNA.Ultima.UI
{
    class UltimaCursor : ICursor
    {
        private HuedTexture m_CursorSprite = null;
        private int m_CursorSpriteArtIndex = -1;
        protected Tooltip m_Tooltip = null;

        public int CursorSpriteArtIndex
        {
            get { return m_CursorSpriteArtIndex; }
            set
            {
                if (value != m_CursorSpriteArtIndex)
                {
                    m_CursorSpriteArtIndex = value;

                    IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                    Texture2D art = provider.GetItemTexture(m_CursorSpriteArtIndex);
                    if (art == null)
                    {
                        // shouldn't we have a debug texture to show that we are missing this cursor art? !!!
                        m_CursorSprite = null;
                    }
                    else
                    {
                        Rectangle sourceRect = new Rectangle(1, 1, art.Width - 2, art.Height - 2);
                        m_CursorSprite = new HuedTexture(art, Point.Zero, sourceRect, 0);
                    }
                }
            }
        }

        public Point CursorOffset
        {
            get;
            protected set;
        }

        public int CursorHue
        {
            get;
            protected set;
        }

        private UserInterfaceService m_UserInterface;

        public UltimaCursor()
        {
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
        }

        public virtual void Dispose()
        {
            m_UserInterface = null;
        }

        public virtual void Update()
        {

        }

        protected virtual void BeforeDraw(SpriteBatchUI spritebatch, Point position)
        {
            // Over the interface or not in world. Display a default cursor.
            int artworkIndex = 8305;

            if (WorldModel.IsInWorld && WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode)
            {
                // if in warmode, show the red-hued cursor.
                artworkIndex -= 23;
            }

            CursorSpriteArtIndex = artworkIndex;
            CursorOffset = new Point(-1, 1);
        }

        public void Draw(SpriteBatchUI spritebatch, Point position)
        {
            BeforeDraw(spritebatch, position);

            if (m_CursorSprite != null)
            {
                m_CursorSprite.Hue = CursorHue;
                m_CursorSprite.Offset = CursorOffset;
                m_CursorSprite.Draw(spritebatch, position);
            }

            DrawTooltip(spritebatch, position);
        }

        protected virtual void DrawTooltip(SpriteBatchUI spritebatch, Point position)
        {
            if (m_UserInterface.IsMouseOverUI && m_UserInterface.MouseOverControl != null && m_UserInterface.MouseOverControl.HasTooltip)
            {
                if (m_Tooltip != null && m_Tooltip.Caption != m_UserInterface.MouseOverControl.Tooltip)
                {
                    m_Tooltip.Dispose();
                    m_Tooltip = null;
                }
                if (m_Tooltip == null)
                {
                    m_Tooltip = new Tooltip(m_UserInterface.MouseOverControl.Tooltip);
                }
                m_Tooltip.Draw(spritebatch, position.X, position.Y + 24);
            }
            else
            {
                if (m_Tooltip != null)
                {
                    m_Tooltip.Dispose();
                    m_Tooltip = null;
                }
            }
        }
    }
}
