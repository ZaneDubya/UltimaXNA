/***************************************************************************
 *   UltimaCursor.cs
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
using UltimaXNA.Ultima.ClientVars;
using UltimaXNA.Ultima.World;

#endregion

namespace UltimaXNA.Ultima.UI
{
    class UltimaCursor
    {
        private HuedTexture m_CursorSprite = null;
        private int m_CursorSpriteArtIndex = -1;

        public int CursorSpriteArtIndex
        {
            get { return m_CursorSpriteArtIndex; }
            set
            {
                if (value != m_CursorSpriteArtIndex)
                {
                    m_CursorSpriteArtIndex = value;

                    Texture2D art = IO.ArtData.GetStaticTexture(m_CursorSpriteArtIndex);
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

        private Point m_CursorOffset = Point.Zero;
        public Point CursorOffset
        {
            set { m_CursorOffset = value; }
            get { return m_CursorOffset; }
        }

        private int m_CursorHue = 0;
        public int CursorHue
        {
            set { m_CursorHue = value; }
            get { return m_CursorHue; }
        }

        public virtual void Dispose()
        {

        }

        public virtual void Update()
        {

        }

        protected virtual void BeforeDraw(SpriteBatchUI spritebatch, Point position)
        {
            int artworkIndex = 8305;

            if (EngineVars.InWorld && EntityManager.GetPlayerObject().Flags.IsWarMode)
            {
                // Over the interface or not in world. Display a default cursor.
                artworkIndex -= 23;
            }

            CursorSpriteArtIndex = artworkIndex;
            CursorOffset = new Point(1, 1);
        }

        public virtual void Draw(SpriteBatchUI spritebatch, Point position)
        {
            BeforeDraw(spritebatch, position);

            if (m_CursorSprite != null)
            {
                m_CursorSprite.Hue = m_CursorHue;
                m_CursorSprite.Offset = m_CursorOffset;
                m_CursorSprite.Draw(spritebatch, position);
            }
        }
    }
}
