using InterXLib.Patterns.MVC;
using UltimaXNA.Ultima.World.Views;
using UltimaXNA.Ultima.World.Controllers;

namespace UltimaXNA.Ultima.World
{
    class WorldView : AView
    {
        public IsometricRenderer Isometric
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
        }

        public override void Draw(double frameTime)
        {
            Isometric.Draw(Model.Map, EntityManager.GetPlayerObject().Position, Model.Input.MousePick);
        }
    }
}
