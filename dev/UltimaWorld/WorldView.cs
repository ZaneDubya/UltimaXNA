using InterXLib.Display;
using InterXLib.Patterns.MVC;
using Microsoft.Xna.Framework.Graphics;

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
            Viewport old=UltimaEngine.GraphicsDeviceManager.GraphicsDevice.Viewport;
            UltimaEngine.GraphicsDeviceManager.GraphicsDevice.Viewport = new Viewport(0, 0, 800, 600);
            View.IsometricRenderer.Draw(Model.Map);
            UltimaEngine.GraphicsDeviceManager.GraphicsDevice.Viewport = old;

            UltimaEngine.UserInterface.Draw(frameTime);
        }
    }
}
