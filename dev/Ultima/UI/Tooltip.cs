using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;

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

        public Tooltip(string caption)
        {
            Caption = caption;
        }

        public void Dispose()
        {
            Caption = null;
        }

        public void Draw(SpriteBatchUI spriteBatch, int x, int y)
        {
            // draw checkered trans underneath.

            if (m_RenderedText == null)
            {
                m_RenderedText = new RenderedText(Caption, 200);
            }

            spriteBatch.Draw2DTiled(Controls.CheckerTrans.CheckeredTransTexture, new Rectangle(x - 4, y - 4, m_RenderedText.Width + 8, m_RenderedText.Height + 8), Vector3.Zero);

            m_RenderedText.Draw(spriteBatch, new Point(x, y));
        }
    }
}
