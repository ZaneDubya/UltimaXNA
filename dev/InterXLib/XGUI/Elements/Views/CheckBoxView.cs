using InterXLib.Display;
using InterXLib.XGUI.Elements;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI.Elements.Views
{
    class CheckBoxView : AElementView
    {
        new CheckBox Model
        {
            get
            {
                return (CheckBox)base.Model;
            }
        }

        public CheckBoxView(CheckBox checkbox, GUIManager manager)
            : base(checkbox, manager)
        {

        }

        Rendering.ARenderer m_Normal, m_Checked;
        WrappedText m_WrappedText;

        protected override void InternalBeforeDraw()
        {
            if (Model.IsMouseDown)
                Model.LocalArea = new Rectangle(Model.LocalArea.X, Model.LocalArea.Y + 1, Model.LocalArea.Width, Model.LocalArea.Height);
        }

        protected override void InternalAfterDraw()
        {
            if (Model.IsMouseDown)
                Model.LocalArea = new Rectangle(Model.LocalArea.X, Model.LocalArea.Y - 1, Model.LocalArea.Width, Model.LocalArea.Height);
        }

        protected override void InternalDraw(YSpriteBatch spritebatch, double frameTime)
        {
            Color color = Model.IsEnabled ? Color.White : new Color(160, 160, 160, 255);

            Rendering.ARenderer renderer = Model.IsChecked ? m_Checked : m_Normal;
            renderer.Render(spritebatch, Model.ScreenArea, color);

            if (Model.Caption != null && Model.Caption != string.Empty)
            {
                if (m_WrappedText == null)
                    m_WrappedText = new WrappedText();
                Rectangle area = Model.ScreenArea;
                area.X += 16;
                area.Width -= 16;

                m_WrappedText.Draw(spritebatch, Font, area, Model.Caption, Model.FontSize, FontJustification.Left | FontJustification.Bottom, color);
            }
        }

        protected override void LoadRenderers()
        {
            m_Normal = LoadRenderer<Rendering.IconRenderer>("XGUI", "checkbox_icon");
            m_Checked = LoadRenderer<Rendering.IconRenderer>("XGUI", "checkbox_icon_checked");
        }
    }
}
