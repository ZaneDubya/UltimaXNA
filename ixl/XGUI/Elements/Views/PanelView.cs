using InterXLib.Display;
using InterXLib.XGUI.Elements;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI.Elements.Views
{
    class PanelView : AElementView
    {
        new Panel Model
        {
            get
            {
                return (Panel)base.Model;
            }
        }

        public PanelView(Panel panel, GUIManager manager)
            : base(panel, manager)
        {

        }

        Rendering.BorderedRenderer m_PanelRenderer, m_RecessedRenderer, m_ShadowRenderer;
        Rendering.HorizontalSlidingRenderer m_HeaderRenderer;

        public override Rectangle BuildChildArea(Point area)
        {
            return new Rectangle(4, Model.DrawHeader ? 30 : 4, area.X - 8, area.Y - (Model.DrawHeader ? 34 : 8));
        }

        protected override void InternalDraw(YSpriteBatch spritebatch, double frameTime)
        {
            if (Model.DrawHeader)
                Model.FontName = "CenturyGothic11Bold";
            else
                Model.FontName = "CenturyGothic11";

            if (Model.DrawAsRecessed)
            {
                m_RecessedRenderer.Render(spritebatch, Model.ScreenArea);
            }
            else
            {
                if (Model.Parent.GetType() != typeof(Panel))
                {
                    int x = 6;
                    int y = 6;
                    Rectangle shadow_area = new Rectangle(Model.ScreenArea.X - x, Model.ScreenArea.Y - y, Model.ScreenArea.Width + x * 2, Model.ScreenArea.Height + y * 2);
                    m_ShadowRenderer.Render(spritebatch, shadow_area);
                }

                m_PanelRenderer.Render(spritebatch, Model.ScreenArea);

                if (Model.DrawHeader)
                    m_HeaderRenderer.Render(spritebatch, Model.ScreenArea);

                if (Model.Caption != null && Model.Caption != string.Empty)
                {
                    Vector2 position = new Vector2(
                        Model.ScreenArea.X + Model.ChildArea.X + (Model.DrawHeader && Model.HeaderIcon != null ? 28 : 4), 
                        Model.ScreenArea.Y + 5);
                    spritebatch.DrawString(Font, Model.Caption, position, 18f, Vector2.Zero, Color.White);
                }
            }
        }

        protected override void LoadRenderers()
        {
            m_PanelRenderer = LoadRenderer<Rendering.BorderedRenderer>("XGUI", "panel");
            m_RecessedRenderer = LoadRenderer<Rendering.BorderedRenderer>("XGUI", "recessed");
            m_HeaderRenderer = LoadRenderer<Rendering.HorizontalSlidingRenderer>("XGUI", "panel_header");
            m_ShadowRenderer = LoadRenderer<Rendering.BorderedRenderer>("XGUI", "panel_shadow");
        }
    }
}
