using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaGUI
{
    class UltimaCursor
    {
        private Sprite m_CursorSprite = null;
        private int m_CursorSpriteArtIndex = -1;

        public int CursorSpriteArtIndex
        {
            get { return m_CursorSpriteArtIndex; }
            set
            {
                if (value != m_CursorSpriteArtIndex)
                {
                    m_CursorSpriteArtIndex = value;

                    Texture2D art = UltimaData.ArtData.GetStaticTexture(m_CursorSpriteArtIndex);
                    if (art == null)
                    {
                        // shouldn't we have a debug texture to show that we are missing this cursor art? !!!
                        m_CursorSprite = null;
                    }
                    else
                    {
                        Rectangle sourceRect = new Rectangle(1, 1, art.Width - 2, art.Height - 2);
                        m_CursorSprite = new Sprite(art, Point2D.Zero, sourceRect, 0);
                    }
                }
            }
        }

        private Point2D m_CursorOffset = Point2D.Zero;
        public Point2D CursorOffset
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
            // Over the interface or not in world. Display a default cursor.
            CursorOffset = new Point2D(1, 1);
            CursorSpriteArtIndex = 8305;
        }

        public void Draw(SpriteBatchUI spritebatch, Point2D position)
        {
            BeforeDraw(spritebatch, position);

            if (m_CursorSprite != null)
            {
                m_CursorSprite.Offset = m_CursorOffset;
                m_CursorSprite.Hue = m_CursorHue;
                m_CursorSprite.Draw(spritebatch, position);
            }
        }

        protected virtual void BeforeDraw(SpriteBatchUI spritebatch, Point2D position)
        {

        }
    }
}
