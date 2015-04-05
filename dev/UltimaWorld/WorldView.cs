using InterXLib.Display;
using InterXLib.Patterns.MVC;
using UltimaXNA.UltimaWorld.Views;
using UltimaXNA.UltimaWorld.Controllers;

namespace UltimaXNA.UltimaWorld
{
    class WorldView : AView
    {
        public IsometricRenderer Isometric
        {
            get;
            private set;
        }

        public MousePicking MousePick
        {
            get;
            private set;
        }

        protected new WorldModel Model
        {
            get { return (WorldModel)base.Model; }
        }

        public WorldView(WorldModel model)
            : base(model)
        {
            Isometric = new IsometricRenderer();
            Isometric.Initialize(Model.Engine);
            Isometric.LightDirection = -0.6f;
            MousePick = new MousePicking();
        }

        public override void Draw(YSpriteBatch spritebatch, double frameTime)
        {
            Isometric.Draw(Model.Map, EntityManager.GetPlayerObject().Position, MousePick);
        }
    }
}
