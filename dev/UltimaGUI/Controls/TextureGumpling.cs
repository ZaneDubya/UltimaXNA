using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class TextureGumpling : Control
    {
        Texture2D m_Texture;
        Color[] m_TextureData;

        public TextureGumpling(Control owner, int page, int x, int y, int width, int height)
            : base(owner, page)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            HandlesMouseInput = true;
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            if (m_Texture == null)
            {
                m_Texture = new Texture2D(spriteBatch.GraphicsDevice, Width, Height);
                if (m_TextureData == null)
                    m_Texture.GetData<Color>(m_TextureData);
                else
                    m_Texture.SetData<Color>(m_TextureData);
            }

            spriteBatch.Draw2D(m_Texture, new Rectangle(X, Y, Width, Height), 0, false, false);

            base.Draw(spriteBatch);
        }

        protected override void mouseOver(int x, int y)
        {
            if (UserInterface.MouseOverControl == this)
                SetPixel(new Color(255, 255, 255, 255), x, y);
        }

        public Color GetPixel(int x, int y)
        {
            if (m_TextureData == null)
                m_TextureData = new Color[Width * Height];
            return m_TextureData[y * Width + x];
        }

        public void SetPixel(Color c, int x, int y)
        {
            if (m_TextureData == null)
                m_TextureData = new Color[Width * Height];
            m_TextureData[y * Width + x] = c;
            if (m_Texture != null)
                m_Texture.SetData<Color>(0, new Rectangle(x, y, 1, 1), new Color[1] { c }, 0, 1);
        }
    }
}
