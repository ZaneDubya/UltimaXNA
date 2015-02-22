using InterXLib.Display;
using InterXLib.XGUI.Elements;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI.Elements.Views
{
    class ButtonView : AElementView
    {
        new Button Model
        {
            get
            {
                return (Button)base.Model;
            }
        }

        private bool AsIcon = false;
        private string m_IconButtonNormalName, m_IconButtonHoverName, m_IconButtonDownName;

        public ButtonView(Button button, GUIManager manager, bool as_icon = false, 
            string icon_normal = null, string icon_hover = null, string icon_down = null)
            : base(button, manager)
        {
            AsIcon = as_icon;
            if (AsIcon)
            {
                m_IconButtonNormalName = icon_normal;
                m_IconButtonHoverName = icon_hover;
                m_IconButtonDownName = icon_down;
            }
        }

        Rendering.ARenderer m_Normal, m_Hover, m_Down;
        Rendering.ARenderer m_Icon;
        WrappedText m_WrappedText;

        protected override void InternalBeforeDraw()
        {
            if (!AsIcon && Model.IsMouseDown)
                Model.LocalArea = new Rectangle(Model.LocalArea.X, Model.LocalArea.Y + 1, Model.LocalArea.Width, Model.LocalArea.Height);
        }

        protected override void InternalAfterDraw()
        {
            if (!AsIcon && Model.IsMouseDown)
                Model.LocalArea = new Rectangle(Model.LocalArea.X, Model.LocalArea.Y - 1, Model.LocalArea.Width, Model.LocalArea.Height);
        }

        protected override void InternalDraw(YSpriteBatch spritebatch, double frameTime)
        {
            Color color = Model.IsEnabled ? Color.White : new Color(160, 160, 160, 255);

            Rendering.ARenderer renderer = Model.IsMouseOver ? (Model.IsMouseDown ? m_Down : m_Hover) : m_Normal;
            renderer.Render(spritebatch, Model.ScreenArea, color);
            if (Model.DrawIcon)
            {
                Rectangle icon_area = Model.ScreenArea;
                icon_area.X += 4;
                icon_area.Y += 4;
                icon_area.Width = 36;
                icon_area.Height -= 8;
                m_Icon.Render(spritebatch, icon_area);
            }
            if (Model.Caption != null && Model.Caption != string.Empty)
            {
                if (m_WrappedText == null)
                    m_WrappedText = new WrappedText();
                Rectangle area = Model.ScreenArea;
                area.X += (Model.DrawIcon ? 44 : 4);
                // area.Y += 4;
                area.Width -= (Model.DrawIcon ? 48 : 8);
                // area.Height -= 8;

                m_WrappedText.Draw(spritebatch, Font, area, Model.Caption, Model.FontSize, FontJustification.Center | FontJustification.CenterVertically, color);
            }
        }

        protected override void LoadRenderers()
        {
            if (!AsIcon)
            {
                m_Normal = LoadRenderer<Rendering.BorderedRenderer>("XGUI", "button");
                m_Hover = LoadRenderer<Rendering.BorderedRenderer>("XGUI", "button_over");
                m_Down = LoadRenderer<Rendering.BorderedRenderer>("XGUI", "button_down");
                m_Icon = LoadRenderer<Rendering.BorderedRenderer>("XGUI", "recessed");
            }
            else
            {
                m_Normal = LoadRenderer<Rendering.IconRenderer>("XGUI", m_IconButtonNormalName);
                m_Down = LoadRenderer<Rendering.IconRenderer>("XGUI", m_IconButtonDownName);
                if (m_IconButtonHoverName == null)
                    m_Hover = m_Down;
                else
                    m_Hover = LoadRenderer<Rendering.IconRenderer>("XGUI", m_IconButtonHoverName);
            }
        }
    }
}
