using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Graphics;

namespace UltimaXNA.GUI
{
    public class Sprite
    {
        Texture2D m_Texture = null;
        Point2D m_Offset;
        Rectangle m_SourceRect = Rectangle.Empty;
        int m_Hue = 0;

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
