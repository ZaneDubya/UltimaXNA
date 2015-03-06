using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Rendering;

namespace UltimaXNA.UltimaGUI
{
    public class Sprite
    {
        private Texture2D m_Texture = null;
        private Rectangle m_SourceRect = Rectangle.Empty;

        private Point2D m_Offset;
        public Point2D Offset
        {
            set { m_Offset = value; }
            get { return m_Offset; }
        }
        
        private int m_Hue = 0;
        public int Hue
        {
            set { m_Hue = value; }
            get { return m_Hue; }
        }

        public Sprite(Texture2D texture, Point2D offset, Rectangle source, int hue)
        {
            m_Texture = texture;
            m_Offset = offset;
            m_SourceRect = source;
            m_Hue = hue;
        }

        public void Draw(SpriteBatchUI sb, Point2D position)
        {
            sb.Draw2D(m_Texture, position - m_Offset, m_SourceRect, m_Hue, false, false);
        }
    }
}
