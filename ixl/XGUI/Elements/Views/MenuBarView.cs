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
        private HorizontalSlidingDoorRenderer _default, _hover, _pressed;
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
            _default = LoadRenderer<HorizontalSlidingDoorRenderer>("XGUI", "menubar");
            _hover = LoadRenderer<HorizontalSlidingDoorRenderer>("XGUI", "menubar_hover");
            _pressed = LoadRenderer<HorizontalSlidingDoorRenderer>("XGUI", "menubar_pushed");
        }

        protected override void InternalDraw(YSpriteBatch spritebatch, double frameTime)
        {
            Rectangle area = Model.ScreenArea;
            _default.Render(spritebatch, area);

            area.X += _hover.Buffer;

            foreach (MenuElement e in Model.Children)
            {
                bool elementDown = false;
                Vector2 v = Font.MeasureString(e.Label, m_FontSize);

                if (Model.IsMouseOver)
                {
                    Rectangle button = new Rectangle(area.X, area.Y, (int)v.X + _hover.Buffer * 2, m_MenuHeight);
                    if (e.Enabled && button.Contains(Model.MousePosition))
                        if (Model.IsMouseDown)
                        {
                            _pressed.Render(spritebatch, button);
                            elementDown = true;
                        }
                        else
                            _hover.Render(spritebatch, button);
                }
                if (!elementDown)
                    area.Y--;
                Color font_color = e.Enabled ? Color.White : Color.Gray;
                spritebatch.DrawString(Font, e.Label, new Vector2(area.X + _hover.Buffer, area.Y + (m_MenuHeight - v.Y) / 2f), m_FontSize, color: font_color);
                if (!elementDown)
                    area.Y++;
                area.X += (_hover.Buffer * 2 + (int)v.X);
                area.Width -= (_hover.Buffer * 2 + (int)v.X);
            }
        }

        public MenuElement GetMenuElementUnderMouse()
        {
            if (!Model.IsMouseOver)
                return null;

            Rectangle area = Model.ScreenArea;
            area.X += _hover.Buffer;

            foreach (MenuElement e in Model.Children)
            {
                Vector2 v = Font.MeasureString(e.Label, m_FontSize);
                Rectangle button = new Rectangle(area.X, area.Y, (int)v.X + _hover.Buffer * 2, m_MenuHeight);
                if (e.Enabled && button.Contains(Model.MousePosition))
                    return e;
                area.X += (_hover.Buffer * 2 + (int)v.X);
                area.Width -= (_hover.Buffer * 2 + (int)v.X);
            }

            return null;
        }
    }
}
