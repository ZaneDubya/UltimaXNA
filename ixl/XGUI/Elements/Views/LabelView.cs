using InterXLib.Display;
using InterXLib.XGUI.Elements;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI.Elements.Views
{
    class LabelView : AElementView
    {
        new Label Model
        {
            get
            {
                return (Label)base.Model;
            }
        }

        public LabelView(Label panel, GUIManager manager)
            : base(panel, manager)
        {

        }

        public override Rectangle BuildChildArea(Point area)
        {
            return new Rectangle(0, 0, area.X, area.Y);
        }

        private WrappedText m_WrappedText = null;

        protected override void InternalDraw(YSpriteBatch spritebatch, double frameTime)
        {
            if (m_WrappedText == null)
                m_WrappedText = new WrappedText();
            m_WrappedText.Draw(spritebatch, Font, Model.ScreenArea, Model.Caption, Model.FontSize, Model.Justification, Model.Color);
        }

        protected override void LoadRenderers()
        {

        }
    }
}
