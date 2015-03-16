using InterXLib.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.XGUI.Rendering
{
    class TabBarRenderer : ARenderer
    {
        private Rectangle m_TabArea, m_TabAreaEdge, m_Bar;

        private int m_TabSpaceBegin = 8, m_TabSpaceWidth = 16;
        public int TabSpaceBegin
        {
            get { return m_TabSpaceBegin; }
            set
            {
                if (value < 0)
                    value = 0;
                m_TabSpaceBegin = value;
            }
        }

        public int TabSpaceWidth
        {
            get { return m_TabSpaceWidth; }
            set
            {
                if (value < 16)
                    value = 16;
                m_TabSpaceWidth = value;
            }
        }

        public bool DrawTabs = true;

        public TabBarRenderer(Texture2D texture, Rectangle source, int tabportion_width, int tabend_width, int bar_width)
            : base(texture)
        {
            m_TabArea = new Rectangle(source.Left, source.Top, tabportion_width, source.Height);
            m_TabAreaEdge = new Rectangle(m_TabArea.Right, source.Top, tabportion_width, source.Height);
            m_Bar = new Rectangle(m_TabAreaEdge.Right, source.Top, bar_width, source.Height);
        }

        public override Rectangle BuildChildArea(Point area)
        {
            return new Rectangle(0, 0, area.X, area.Y);
        }

        public override void Render(YSpriteBatch batch, Rectangle area, Color? color = null)
        {
            Color Color = (color == null) ? Color.White : color.Value;
            Rectangle drawArea;

            int x = 0;

            if (DrawTabs)
            {
                // draw bar to the left of the tab area
                if (m_TabSpaceBegin > 0)
                {
                    drawArea = new Rectangle(area.X, area.Y, m_TabSpaceBegin, area.Height);
                    batch.GUIDrawSprite(Texture, drawArea, m_Bar, Color);
                    x += m_TabSpaceBegin;
                }

                // draw the left tab area edge
                drawArea = new Rectangle(area.X + x, area.Y, m_TabAreaEdge.Width, area.Height);
                batch.GUIDrawSprite(Texture, drawArea, m_TabAreaEdge, effects: SpriteEffects.FlipHorizontally);
                x += m_TabAreaEdge.Width;

                // draw the tab area
                drawArea = new Rectangle(area.X + x, area.Y, m_TabSpaceWidth - m_TabAreaEdge.Width * 2, area.Height);
                batch.GUIDrawSprite(Texture, drawArea, m_TabArea, Color);
                x += drawArea.Width;

                // draw the right tab area edge
                drawArea = new Rectangle(area.X + x, area.Y, m_TabAreaEdge.Width, area.Height);
                batch.GUIDrawSprite(Texture, drawArea, m_TabAreaEdge, Color);
                x += m_TabAreaEdge.Width;
            }

            // draw bar over remainder of width
            if (x < area.Width)
            {
                drawArea = new Rectangle(area.X + x, area.Y, area.Width - x, area.Height);
                batch.GUIDrawSprite(Texture, drawArea, m_Bar, Color);
            }
        }
    }
}
