using InterXLib.Display;

namespace InterXLib.XGUI
{
    public class AScreenView : AElementView
    {
        public AScreenView(AScreen screen, GUIManager manager)
            : base(screen, manager)
        {

        }

        protected override void InternalDraw(YSpriteBatch spritebatch, double frameTime)
        {
            // nothing to do here.
        }

        protected override void LoadRenderers()
        {
            // none needed.
        }
    }
}
