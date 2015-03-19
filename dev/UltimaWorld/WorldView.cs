using InterXLib.Display;
using InterXLib.Patterns.MVC;

namespace UltimaXNA.UltimaWorld
{
    class WorldView : AView
    {
        protected new WorldModel Model
        {
            get { return (WorldModel)base.Model; }
        }

        public WorldView(WorldModel model)
            : base(model)
        {

        }

        public override void Draw(YSpriteBatch spritebatch, double frameTime)
        {
            View.IsometricRenderer.Draw(Model.Map);
            UltimaEngine.UserInterface.Draw(frameTime);
        }
    }
}
