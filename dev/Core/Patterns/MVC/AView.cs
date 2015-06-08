
namespace UltimaXNA.Core.Patterns.MVC
{
    /// <summary>
    /// Abstract Model - polls the state of a model, and renders it for the player.
    /// </summary>
    public abstract class AView
    {
        protected AModel Model;

        public AView(AModel parent_model)
        {
            Model = parent_model;
        }

        public virtual void Draw(double frameMS)
        {

        }
    }
}
