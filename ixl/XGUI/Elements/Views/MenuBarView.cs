using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.XGUI.Rendering;
using Microsoft.Xna.Framework;
using InterXLib.Display;

namespace InterXLib.XGUI.Elements.Views
{
    class MenuBarView : AElementView
    {
        private HorizontalSlidingDoorRenderer m_default, m_hover, m_pressed;
        private float m_FontSize = 14f;
        private int m_MenuHeight = 24;

        new MenuBar Model
        {
            get
            {
                return (MenuBar)base.Model;
            }
        }

        public MenuBarView(MenuBar menubar, GUIManager manager)
            : base(menubar, manager)
        {

        }

        protected override void LoadRenderers()
        {
            m_default = LoadRenderer<HorizontalSlidingDoorRenderer>("XGUI", "menubar");
            m_hover = LoadRenderer<HorizontalSlidingDoorRenderer>("XGUI", "menubar_hover");
            m_pressed = LoadRenderer<HorizontalSlidingDoorRenderer>("XGUI", "menubar_pushed");
        }

        protected override void InternalDraw(YSpriteBatch spritebatch, double frameTime)
        {
            Rectangle area = Model.ScreenArea;
            m_default.Render(spritebatch, area);

            area.X += m_hover.Buffer;

            foreach (MenuElement e in Model.Children)
            {
                bool elementDown = false;
                Vector2 v = Font.MeasureString(e.Label, m_FontSize);

                if (Model.IsMouseOver)
                {
                    Rectangle button = new Rectangle(area.X, area.Y, (int)v.X + m_hover.Buffer * 2, m_MenuHeight);
                    if (e.Enabled && button.Contains(Model.MousePosition))
                        if (Model.IsMouseDown)
                        {
                            m_pressed.Render(spritebatch, button);
                            elementDown = true;
                        }
                        else
                            m_hover.Render(spritebatch, button);
                }
                if (!elementDown)
                    area.Y--;
                Color font_color = e.Enabled ? Color.White : Color.Gray;
                spritebatch.DrawString(Font, e.Label, new Vector2(area.X + m_hover.Buffer, area.Y + (m_MenuHeight - v.Y) / 2f), m_FontSize, color: font_color);
                if (!elementDown)
                    area.Y++;
                area.X += (m_hover.Buffer * 2 + (int)v.X);
                area.Width -= (m_hover.Buffer * 2 + (int)v.X);
            }
        }

        public MenuElement GetMenuElementUnderMouse()
        {
            if (!Model.IsMouseOver)
                return null;

            Rectangle area = Model.ScreenArea;
            area.X += m_hover.Buffer;

            foreach (MenuElement e in Model.Children)
            {
                Vector2 v = Font.MeasureString(e.Label, m_FontSize);
                Rectangle button = new Rectangle(area.X, area.Y, (int)v.X + m_hover.Buffer * 2, m_MenuHeight);
                if (e.Enabled && button.Contains(Model.MousePosition))
                    return e;
                area.X += (m_hover.Buffer * 2 + (int)v.X);
                area.Width -= (m_hover.Buffer * 2 + (int)v.X);
            }

            return null;
        }
    }
}
