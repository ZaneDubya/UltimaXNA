using InterXLib.Display;
using InterXLib.XGUI.Elements;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI.Elements.Views
{
    class TabBarView: AElementView
    {
        new TabBar Model
        {
            get
            {
                return (TabBar)base.Model;
            }
        }

        public TabBarView(TabBar panel, GUIManager manager)
            : base(panel, manager)
        {

        }

        public Rectangle[] TabAreas;

        protected override void InternalDraw(YSpriteBatch spritebatch, double frameTime)
        {
            if (Model.TabCount == 0)
            {
                m_TabAreaRenderer.DrawTabs = false;
                m_TabAreaRenderer.Render(spritebatch, Model.ScreenArea);
            }
            else
            {
                int tab_space_begin = 8;
                int tab_space_end = 0;

                // draw the background of the tab space
                m_TabAreaUnderRenderer.Render(spritebatch, Model.ScreenArea);

                // get the width of the tabs
                TabAreas = new Rectangle[Model.TabCount];
                int x = tab_space_begin + 5;
                for (int i = 0; i < Model.TabCount; i++)
                {
                    string caption = Model.GetTabCaption(i);
                    Vector2 caption_size = Font.MeasureString(caption, 18f);
                    TabAreas[i] = new Rectangle(x, Model.ScreenArea.Y + 8, (int)caption_size.X + 16, 28);
                    x += TabAreas[i].Width;
                }

                m_TabAreaRenderer.DrawTabs = true;
                m_TabAreaRenderer.TabSpaceBegin = tab_space_begin;
                m_TabAreaRenderer.TabSpaceWidth = x - 4;
                m_TabAreaRenderer.Render(spritebatch, Model.ScreenArea);

                int up = 0, down = 20, hover = 30;

                for (int i = 0; i < Model.TabCount; i++)
                {
                    Color text_color;
                    Rendering.HorizontalSlidingRenderer renderer;
                    if (Model.SelectedTab == i)
                    {
                        renderer = m_TabSelectedRenderer;
                        text_color = new Color(up, up, up, 255);
                    }
                    else if (Model.HoverTab == i)
                    {
                        renderer = m_TabUnselectedHoverRenderer;
                        text_color = new Color(hover, hover, hover, 255);
                    }
                    else
                    {
                        renderer = m_TabUnselectedRenderer;
                        text_color = new Color(down, down, down, 255);
                    }
                    renderer.Render(spritebatch, TabAreas[i]);
                    spritebatch.DrawString(Font, Model.GetTabCaption(i), new Vector2(TabAreas[i].X + 8, TabAreas[i].Y + 4), 18f,
                        color: text_color);
                }
                tab_space_end = x + 16;
                if (Model.Caption != null)
                {
                    spritebatch.DrawString(Font, Model.Caption, new Vector2(tab_space_end, 11), 18f, color: Color.Black);
                }
            }

        }

        Rendering.HorizontalSlidingRenderer m_TabAreaUnderRenderer, m_TabSelectedRenderer;
        Rendering.HorizontalSlidingRenderer m_TabUnselectedRenderer, m_TabUnselectedHoverRenderer;
        Rendering.TabBarRenderer m_TabAreaRenderer;

        protected override void LoadRenderers()
        {
            m_TabAreaRenderer = LoadRenderer<Rendering.TabBarRenderer>("XGUI", "tab_over");
            m_TabAreaUnderRenderer = LoadRenderer<Rendering.HorizontalSlidingRenderer>("XGUI", "tab_under");
            m_TabSelectedRenderer = LoadRenderer<Rendering.HorizontalSlidingRenderer>("XGUI", "tab_selected");
            m_TabUnselectedRenderer = LoadRenderer<Rendering.HorizontalSlidingRenderer>("XGUI", "tab_unselected");
            m_TabUnselectedHoverRenderer = LoadRenderer<Rendering.HorizontalSlidingRenderer>("XGUI", "tab_unselected_hover");
        }
    }
}
